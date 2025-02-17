using Tilray.Integrations.Core.Common.Extensions;

namespace Tilray.Integrations.Services.SAPConcur.Service;

public class SAPConcurService(HttpClient client, SAPConcurSettings sapConcurSettings, ILogger<SAPConcurService> logger) : ISAPConcurService
{
    #region Private methods

    private string BuildDigestsUri(DateTime startDate, DateTime endDate) =>
        $"/api/v3.0/invoice/paymentrequestdigests" +
        $"?approvalStatus={sapConcurSettings.ApprovalStatus}" +
        $"&paymentStatus={sapConcurSettings.PaymentStatus}" +
        $"&extractedDateAfter={startDate:yyyy-MM-dd}" +
        $"&extractedDateBefore={endDate:yyyy-MM-dd}";

    private async Task<Result<T>> GetAsync<T>(string requestUri) where T : class
    {
        var response = await client.GetAsync(requestUri);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError(errorMessage);
            return Result.Fail<T>($"Failed to fetch invoice(s) with requestUri {requestUri}. Error: {errorMessage}");
        }

        var result = JsonConvert.DeserializeObject<T>(content);

        return result == null
            ? Result.Fail<T>("Failed to deserialize API response")
            : Result.Ok(result);
    }

    private async Task<Result<InvoiceDigests>> GetInvoiceDigestsAsync(DateTime startDate, DateTime endDate)
    {
        var allDigests = new InvoiceDigests { PaymentRequestDigest = [] };
        var nextPageUri = BuildDigestsUri(startDate, endDate);

        while (!string.IsNullOrEmpty(nextPageUri))
        {
            var pageResult = await GetAsync<InvoiceDigests>(nextPageUri);

            if (pageResult.IsFailed)
                return Result.Fail<InvoiceDigests>(pageResult.Errors);

            var currentPage = pageResult.Value ?? new InvoiceDigests();
            allDigests.PaymentRequestDigest.AddRange(currentPage.PaymentRequestDigest ?? []);

            nextPageUri = currentPage.NextPage ?? string.Empty;
        }

        logger.LogInformation("Total {TotalDigests} invoice digests found",
            allDigests.PaymentRequestDigest.Count());
        return Result.Ok(allDigests);
    }

    private async Task<Result<Invoice>> GetInvoiceByIdAsync(string invoiceId)
    {
        var requestUri = $"/api/v3.0/invoice/paymentrequest/{invoiceId}";
        var result = await GetAsync<Invoice>(requestUri);

        if (result.IsSuccess && result.Value != null)
            logger.LogInformation("Successfully retrieved invoice {InvoiceId}", invoiceId);
        else
            logger.LogWarning("Failed to retrieve invoice {InvoiceId}", invoiceId);

        return result;
    }

    #endregion

    #region Public methods

    public async Task<Result<IEnumerable<Invoice>>> GetInvoicesAsync()
    {
        var startDate = DateTime.UtcNow.AddMinutes(-sapConcurSettings.InvoicesFetchDurationInMinutes);
        var endDate = DateTime.UtcNow;

        logger.LogInformation("GetInvoices: Fetching invoices from {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}",
            startDate, endDate);

        var digestsResult = await GetInvoiceDigestsAsync(startDate, endDate);
        if (digestsResult.IsFailed)
        {
            logger.LogError("GetInvoices: Failed to retrieve invoice digests: {Errors}",
                Helpers.GetErrorMessage(digestsResult.Errors));
            return Result.Fail<IEnumerable<Invoice>>(digestsResult.Errors);
        }

        var digests = digestsResult.Value?.PaymentRequestDigest ?? [];
        if (digests.Count == 0)
        {
            logger.LogInformation("GetInvoices: No invoice digests found in date range");
            return Result.Ok(Enumerable.Empty<Invoice>());
        }

        logger.LogInformation("GetInvoices: Processing {DigestCount} invoice digests", digests.Count());
        var invoiceResults = await Task.WhenAll(digests.Select(async digest =>
        {
            var invoiceResult = await GetInvoiceByIdAsync(digest.ID);
            if (invoiceResult.IsSuccess && invoiceResult.Value != null)
            {
                invoiceResult.Value.LastModifiedDate = digest.LastModifiedDate;
            }
            return invoiceResult;
        }));

        var invoices = invoiceResults
            .Where(r => r.IsSuccess && r.Value != null)
            .Select(r => r.Value);

        logger.LogInformation($"GetInvoices: Successfully fetched {invoices.Count()} invoices. InvoiceIds: {Helpers.GetErrorMessage(invoices.Select(data => data.ID))}");
        return Result.Ok(invoices);
    }

    #endregion
}
