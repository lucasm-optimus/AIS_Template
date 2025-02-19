namespace Tilray.Integrations.Core.Application.SalesOrders.QueryHandlers;

public record ValidateSalesOrderQuery(IEnumerable<SalesOrder> SalesOrders)
    : QueryManyBase<SalesOrder>
{
    public bool AreValidSaleOrders() => SalesOrders?.Any() == true;
}
