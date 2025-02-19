namespace Tilray.Integrations.Core.Application.SalesOrders.QueryHandlers;

public record ValidateSalesOrderQuery(IEnumerable<SalesOrder> salesOrders) : QueryManyBase<SalesOrder>
{
    public IEnumerable<SalesOrder> SalesOrders { get; } = salesOrders;
}
