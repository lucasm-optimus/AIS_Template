
namespace Tilray.Integrations.Core.Application.Adapters.Service
{
    public interface ISalesforceService
    {
        Task<Result<List<AuditItem>>> GetAuditItemsAsync(string reportDate);
    }
}
