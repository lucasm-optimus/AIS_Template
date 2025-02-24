namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands
{
    public record CreateSalesOrderCommand(MedSalesOrder SalesOrder) : ICommand<SalesOrderCreated>
    {
    }
}
