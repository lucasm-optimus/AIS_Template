namespace Tilray.Integrations.Core.Application.SalesOrders.QueryHandlers;

public class ValidateSalesOrderQueryHandler(IRootstockService rootstockService) : IQueryManyHandler<ValidateSalesOrderQuery, SalesOrder>
{
    public async Task<Result<IEnumerable<SalesOrder>>> Handle(ValidateSalesOrderQuery request, CancellationToken cancellationToken)
    {
        if (request.SalesOrders == null || !request.SalesOrders.Any())
        {
            return Result.Fail<IEnumerable<SalesOrder>>("No sales orders to validate");
        }

        return await rootstockService.ValidateSalesOrders(request.SalesOrders);
    }
}
