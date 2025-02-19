namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IAuthorizeNetService
{
    Task<Result> CapturePaymentAsync(string transactionId, decimal amount);
}
