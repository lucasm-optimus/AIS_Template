using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;

namespace Tilray.Integrations.Services.Authorize.net.Service;

public class AuthorizeNetService(HttpClient httpClient, AuthorizeNetSettings authorizeNetSettings, ILogger<AuthorizeNetService> logger) : IAuthorizeNetService
{
    private async Task<Result<string>> PostAsync<T>(T entity)
    {
        var json = Helpers.CreateStringContent(entity);
        var response = await httpClient.PostAsync($"{authorizeNetSettings.BaseUrl}", json);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = $"Error: {Helpers.GetErrorFromResponse(response)}";
            logger.LogError(errorMessage);
            return Result.Fail<string>(errorMessage);
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return Result.Ok(responseBody);
    }

    private async Task<Result<string>> GetTransactionDetailsAsync(string transactionId)
    {
        var request = TransactionDetailsRequest.Create(
            authorizeNetSettings.APILoginId,
            authorizeNetSettings.TransactionKey,
            transactionId
        );

        var result = await PostAsync(request);
        if (result.IsFailed)
            return result.ToResult();

        var response = JsonConvert.DeserializeObject<TransactionDetailsResponse>(result.Value);
        if (response?.GetTransactionDetailsResponse?.Transaction != null)
        {
            var status = response.GetTransactionDetailsResponse.Transaction.TransactionStatus;
            return Result.Ok(status);
        }

        var errorResponse = JsonConvert.DeserializeObject<TransactionErrorResponse>(result.Value);
        if (errorResponse?.Messages != null)
        {
            logger.LogError($"Error: {errorResponse.Messages.Message[0].Text}");
            return Result.Fail(errorResponse.Messages.Message[0].Text);
        }

        return Result.Fail("Failed to get transaction details");
    }

    public async Task<Result> CapturePaymentAsync(string transactionId, decimal amount)
    {
        var transactionDetailsResult = await GetTransactionDetailsAsync(transactionId);
        if (transactionDetailsResult.IsFailed)
            return transactionDetailsResult.ToResult();

        var status = transactionDetailsResult.Value;
        if (status is "settledSuccessfully" or "voided")
        {
            return Result.Fail(
                new Error($"Transaction already {status}")
                    .WithMetadata("Status", status)
                    .WithMetadata("TransactionId", transactionId)
            );
        }

        var request = TransactionCaptureRequest.Create(authorizeNetSettings.APILoginId, authorizeNetSettings.TransactionKey,
            transactionId, amount);

        var result = await PostAsync(request);
        if (result.IsFailed)
            return result.ToResult();

        var response = JsonConvert.DeserializeObject<TransactionCaptureResponse>(result.Value);
        if (response?.TransactionResponse != null)
        {
            if (response.TransactionResponse.ResponseCode == "1" &&
                response.TransactionResponse.Messages.Message[0].Code.StartsWith("I"))
            {
                logger.LogInformation($"Successfully captured payment {transactionId}");
                return Result.Ok();
            }
        }

        var errorResponse = JsonConvert.DeserializeObject<TransactionErrorResponse>(result.Value);
        if (errorResponse?.Messages != null)
        {
            var error = errorResponse.Messages.Message[0];
            logger.LogError($"Capture error {error.Code}: {error.Text}");
            return Result.Fail(error.Text);
        }

        return Result.Fail("Unknown error occurred during payment capture.");
    }
}
