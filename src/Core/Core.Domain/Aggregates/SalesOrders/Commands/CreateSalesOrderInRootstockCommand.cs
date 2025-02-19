namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands;

public class CreateSalesOrderInRootstockCommand(SalesOrder salesOrder) : ICommand
{
    public SalesOrder SalesOrder { get; } = salesOrder;
}
