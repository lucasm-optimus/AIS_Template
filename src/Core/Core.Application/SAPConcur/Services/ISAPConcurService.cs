using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Application.SAPConcur.Services
{
    public interface ISAPConcurService
    {
        Task<Result<IEnumerable<SalesOrder>>> GetSalesOrders();
    }
}
