namespace Tilray.Integrations.Core.Application.SalesOrders.QueryHandlers;

public class ValidateSalesOrderQuery(IEnumerable<SalesOrder> salesOrders) : IQuery<IEnumerable<SalesOrder>>
{
    public IEnumerable<SalesOrder> SalesOrders { get; } = salesOrders;
}
