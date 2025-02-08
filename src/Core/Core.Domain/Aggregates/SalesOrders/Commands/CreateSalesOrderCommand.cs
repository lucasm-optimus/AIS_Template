using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record CreateSalesOrderCommand(MedSalesOrder SalesOrder, string CorrelationId) : ICommand<SalesOrderCreated>
    {
    }
}
