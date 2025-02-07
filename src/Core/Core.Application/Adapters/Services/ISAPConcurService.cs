namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISAPConcurService
{
    Task<Result<IEnumerable<Invoice>>> GetInvoicesAsync();
}
