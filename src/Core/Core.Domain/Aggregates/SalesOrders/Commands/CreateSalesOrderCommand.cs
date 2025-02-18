using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record CreateSalesOrderCommand(MedSalesOrder SalesOrder, string CorrelationId) : ICommand<SalesOrderCreated>
    {
    }
}
