namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands
{
    public record ProcessSalesOrdersCommand(List<Ecom.SalesOrder> SalesOrders) : ICommand<SalesOrdersProcessed>
    {
    }
}