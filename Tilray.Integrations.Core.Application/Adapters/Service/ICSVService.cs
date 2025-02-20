
namespace Tilray.Integrations.Core.Application.Adapters.Service
{
    public interface ICSVService
    {
        Result<string> GenerateCsv(List<AuditItem> payload);
    }
}
