
using System.Text;
using Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders;
using Tilray.Integrations.Services.SAPConcur.Service.Models;
using Tilray.Integrations.Services.SAPConcur.Startup;

namespace Tilray.Integrations.Services.SAPConcur.Service;

public class SAPConcurService(HttpClient client, SAPConcurSettings sapConcurSettings, ILogger<SAPConcurService> logger, IMapper mapper) : ISAPConcurService

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

        var result = content.ToObject<T>();

        return result == null
            ? Result.Fail<T>("Failed to deserialize API response")
            : Result.Ok(result);
    }

    private async Task<Result> GetAsync(string requestUri)
    {
        var response = await client.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError(errorMessage);
            return Result.Fail($"Failed to fetch data with requestUri {requestUri}. Error: {errorMessage}");
        }

        return Result.Ok();
    }

    private async Task<Result> PostAsync<TRequest>(string requestUri, TRequest content)
    {
        var jsonContent = Helpers.CreateStringContent(content);
        var response = await client.PostAsync(requestUri, jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError($"POST {requestUri} failed: {errorMessage}");
            return Result.Fail($"POST {requestUri} failed: {errorMessage}");
        }

        return Result.Ok();
    }

    private async Task<Result> PutAsync<TRequest>(string requestUri, TRequest content)
    {
        var jsonContent = Helpers.CreateStringContent(content);
        var response = await client.PutAsync(requestUri, jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError($"PUT {requestUri} failed: {errorMessage}");
            return Result.Fail($"PUT {requestUri} failed: {errorMessage}");
        }

        return Result.Ok();
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

    private async Task<Result<IEnumerable<Definition>>> GetExtractDefinitionsAsync()
    {
        var result = await GetAsync<IEnumerable<Definition>>("/api/expense/extract/v1.0/");
        if (result.IsFailed)
        {
            logger.LogError("Failed to get extract definitions: {Errors}", Helpers.GetErrorMessage(result.Errors));
            return Result.Fail<IEnumerable<Definition>>(result.Errors);
        }

        var filteredDefinitions = result.Value?.Where(d => d.Name == sapConcurSettings.ExtractDefinitionName);
        if (filteredDefinitions?.Any() != true)
        {
            logger.LogError("No extract definition found with name {DefinitionName}", sapConcurSettings.ExtractDefinitionName);
            return Result.Ok<IEnumerable<Definition>>([]);
        }

        logger.LogInformation("Found the extract definition with name {DefinitionName}", sapConcurSettings.ExtractDefinitionName);
        return Result.Ok(filteredDefinitions);
    }

    private async Task<Result<IEnumerable<Job>>> GetJobsAsync()
    {
        var extractDefinitionsResult = await GetExtractDefinitionsAsync();
        if (extractDefinitionsResult.IsFailed) { return Result.Fail<IEnumerable<Job>>(extractDefinitionsResult.Errors); }

        var definition = extractDefinitionsResult.Value.FirstOrDefault() ?? new();
        var jobsResult = await GetAsync<IEnumerable<Job>>(definition.JobLink);
        if (jobsResult.IsFailed)
        {
            logger.LogError("Failed to retrieve jobs from {JobLink}: Error: {Errors}", definition.JobLink, Helpers.GetErrorMessage(jobsResult.Errors));
            return Result.Fail<IEnumerable<Job>>(jobsResult.Errors);
        }

        var filteredJobs = jobsResult.Value
            .Where(job => DateTime.TryParse(job.StopTime, out var stopTime) && stopTime > DateTime.UtcNow.AddMinutes(-sapConcurSettings.ExpensesFetchDurationInMinutes))
            .OrderBy(job => DateTime.Parse(job.StopTime))
            .ToList();

        if (filteredJobs?.Count == 0)
        {
            logger.LogInformation("No jobs found within the last {Minutes} minutes", sapConcurSettings.ExpensesFetchDurationInMinutes);
            return Result.Ok<IEnumerable<Job>>([]);
        }

        logger.LogInformation("Found {JobCount} jobs within the last {Minutes} minutes", filteredJobs.Count, sapConcurSettings.ExpensesFetchDurationInMinutes);
        return filteredJobs;
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
    public async Task<Result<IEnumerable<ExpenseDetails>>> GetExpenseFilesAsync()
    {
        var expenseDetailsList = new List<ExpenseDetails>();
        var jobsResult = await GetJobsAsync();
        if (jobsResult.IsFailed) { return Result.Fail(jobsResult.Errors); }

        foreach (var job in jobsResult.Value)
        {
            logger.LogInformation("Processing job {JobId} (StopTime: {StopTime})", job.Id, job.StopTime);

            var expenseContent = new List<Expense>();
            using var fileStream = await client.GetStreamAsync(job.FileLink);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (!entry.Name.Contains("CES_SAE")) continue;

                await using var stream = entry.Open();
                using var reader = new StreamReader(stream);
                var expenseFile = await reader.ReadToEndAsync();

                var parts = expenseFile.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if(int.TryParse(parts[2], out int number) && number != 0)
                {
                    var content = expenseFile
                        .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Split('|', StringSplitOptions.RemoveEmptyEntries));

                    var detailRecords = content
                        .Where(record => record.Length > 0 && record[0] == "DETAIL")
                        .Select(record => mapper.Map<Expense>(record));

                    expenseContent.AddRange(detailRecords);
                }
            }

            if (expenseContent.Count != 0)
            {
                logger.LogInformation("Adding {ExpenseCount} expenses for job {JobId}", expenseContent.Count, job.Id);
                expenseDetailsList.Add(ExpenseDetails.Create(job.StopTime, expenseContent));
            }
            else
            {
                logger.LogWarning("No valid expenses found for job {JobId}", job.Id);
            }
        }

        logger.LogInformation("Retrieved {ExpenseDetailCount} expense details", expenseDetailsList.Count);
        return Result.Ok(expenseDetailsList.AsEnumerable());
    }

    #region Purchase Orders

    public async Task<Result<VendorResponse>> GetVendorAsync(PurchaseOrder purchaseOrder)
    {
        var requestUri = "/api/v3.1/invoice/vendors";
        var queryParams = Helpers.CreateQueryParams(
            ("vendorCode", $"{purchaseOrder.VendorCode}-{purchaseOrder.Division}")
        );

        var queryString = Helpers.BuildQueryString(queryParams);
        var fullUri = $"{requestUri}?{queryString}";

        var result = await GetAsync<VendorResponse>(fullUri);
        return result;
    }

    public async Task<Result<VendorCustomResponse>> GetVendorCustomAsync(string itemId, string custom3Value)
    {
        var path = $"/list/v4/lists/{itemId}/children";
        var queryParams = Helpers.CreateQueryParams(
            ("value", custom3Value)
        );

        var queryString = Helpers.BuildQueryString(queryParams);
        var fullUri = $"{path}?{queryString}";

        var result = await GetAsync<VendorCustomResponse>(fullUri);
        return result;
    }

    public async Task<Result<VendorCustomResponse>> GetVendorCustomsAsync(string itemId, string custom3Value)
    {
        var path = $"/list/v4/items/{itemId}/children";
        var queryParams = Helpers.CreateQueryParams(
            ("value", custom3Value)
        );

        var queryString = Helpers.BuildQueryString(queryParams);
        var fullUri = $"{path}?{queryString}";

        var result = await GetAsync<VendorCustomResponse>(fullUri);
        return result;
    }

    public async Task<Result<bool>> PurchaseOrderExistsAsync(string purchaseOrderNumber)
    {
        var getRequestUri = $"/api/v3.0/invoice/purchaseorders/{purchaseOrderNumber}";
        var getResponse = await GetAsync(getRequestUri);

        return getResponse.IsSuccess ? Result.Ok(true) : Result.Ok(false);
    }

    public async Task<Result> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder, SAPConcurCustomValues customFields)
    {
        var purchaseOrderRequest = mapper.Map<SAPConcurPurchaseOrder>(purchaseOrder, opt => opt.Items["CustomFields"] = customFields);
        purchaseOrderRequest.PolicyExternalID = sapConcurSettings?.ConcurPolicyExternalId ?? string.Empty;
        var postRequestUri = $"/api/v3.0/invoice/purchaseorders/{purchaseOrderRequest?.PurchaseOrderNumber}";
        var postResponse = await PostAsync(postRequestUri, purchaseOrderRequest);

        if (postResponse.IsSuccess)
        {
            logger.LogInformation($"PO-{purchaseOrderRequest?.PurchaseOrderNumber} - create successful");
            return Result.Ok();
        }
        else
        {
            var errorMessage = postResponse.Errors.FirstOrDefault();
            logger.LogError($"PO-{purchaseOrderRequest?.PurchaseOrderNumber} - create unsuccessful: {errorMessage}");
            return Result.Fail($"PO Create: {errorMessage}");
        }
    }

    public async Task<Result> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder, SAPConcurCustomValues customFields)
    {
        var purchaseOrderRequest = mapper.Map<SAPConcurPurchaseOrder>(purchaseOrder, opt => opt.Items["CustomFields"] = customFields);
        purchaseOrderRequest.PolicyExternalID = sapConcurSettings?.ConcurPolicyExternalId ?? string.Empty;
        var putRequestUri = $"/api/v3.0/invoice/purchaseorders/{purchaseOrderRequest?.PurchaseOrderNumber}";
        var putResponse = await PutAsync(putRequestUri, purchaseOrderRequest);

        if (putResponse.IsSuccess)
        {
            logger.LogInformation($"PO-{purchaseOrderRequest?.PurchaseOrderNumber} - update successful");
            return Result.Ok();
        }
        else
        {
            var errorMessage = putResponse.Errors.FirstOrDefault();
            logger.LogError($"PO-{purchaseOrderRequest?.PurchaseOrderNumber} - update unsuccessful: {errorMessage}");
            return Result.Fail($"PO Update: {errorMessage}");
        }
    }

    public async Task<Result<bool>> PurchaseOrderReceiptExistsAsync(PurchaseOrderReceipt purchaseOrderReceipt)
    {
        var purchaseOrderReceiptRequest = mapper.Map<SAPConcurPurchaseOrderReceipt>(purchaseOrderReceipt);
        var getRequestUri = $"/api/v3.0/invoice/purchaseorderreceipts";
        var queryParams = Helpers.CreateQueryParams(
            ("goodsReceiptNumber", purchaseOrderReceiptRequest?.GoodsReceiptNumber ?? ""),
            ("lineItemExternalID", purchaseOrderReceiptRequest?.LineItemExternalID ?? ""),
            ("purchaseOrderNumber", purchaseOrderReceiptRequest?.PurchaseOrderNumber ?? "")
        );
        var queryString = Helpers.BuildQueryString(queryParams);
        var fullUri = $"{getRequestUri}?{queryString}";

        var getResponse = await GetAsync(fullUri);
        return getResponse.IsSuccess ? Result.Ok(true) : Result.Ok(false);
    }

    public async Task<Result> CreatePurchaseOrderReceiptAsync(PurchaseOrderReceipt purchaseOrderReceipt)
    {
        var purchaseOrderReceiptRequest = mapper.Map<SAPConcurPurchaseOrderReceipt>(purchaseOrderReceipt);
        var postRequestUri = $"/api/v3.0/invoice/purchaseorderreceipts";
        var postResponse = await PostAsync(postRequestUri, purchaseOrderReceiptRequest);

        if (postResponse.IsSuccess)
        {
            logger.LogInformation($"PO Receipt - {purchaseOrderReceiptRequest?.GoodsReceiptNumber} - create successful");
            return Result.Ok();
        }
        else
        {
            var errorMessage = postResponse.Errors.FirstOrDefault();
            logger.LogError($"PO Receipt - {purchaseOrderReceiptRequest?.GoodsReceiptNumber} - create unsuccessful: {errorMessage}");
            return Result.Fail($"PO Receipt Create: {errorMessage}");
        }
    }

    public async Task<Result> UpdatePurchaseOrderReceiptAsync(PurchaseOrderReceipt purchaseOrderReceipt)
    {
        var purchaseOrderReceiptRequest = mapper.Map<SAPConcurPurchaseOrderReceipt>(purchaseOrderReceipt);
        var putRequestUri = $"/api/v3.0/invoice/purchaseorderreceipts";
        var putResponse = await PutAsync(putRequestUri, purchaseOrderReceiptRequest);

        if (putResponse.IsSuccess)
        {
            logger.LogInformation($"PO Receipt - {purchaseOrderReceiptRequest?.GoodsReceiptNumber} - update successful");
            return Result.Ok();
        }
        else
        {
            var errorMessage = putResponse.Errors.FirstOrDefault();
            logger.LogError($"PO Receipt - {purchaseOrderReceiptRequest?.GoodsReceiptNumber} - update unsuccessful: {errorMessage}");
            return Result.Fail($"PO Receipt Update: {errorMessage}");
        }
    }

    #endregion

    #endregion
}
