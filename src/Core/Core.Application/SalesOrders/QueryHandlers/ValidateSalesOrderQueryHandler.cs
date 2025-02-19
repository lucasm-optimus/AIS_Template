namespace Tilray.Integrations.Core.Application.SalesOrders.QueryHandlers;

public class ValidateSalesOrderQueryHandler(IRootstockService rootstockService, ILogger<ValidateSalesOrderQueryHandler> logger) : IQueryManyHandler<ValidateSalesOrderQuery, SalesOrder>
{
    public async Task<Result<IEnumerable<SalesOrder>>> Handle(ValidateSalesOrderQuery request, CancellationToken cancellationToken)
    {
        if (!request.AreNonEmptySalesOrders())
        {
            logger.LogInformation("No sales orders to validate");
            return Result.Fail<IEnumerable<SalesOrder>>("No sales orders to validate");
        }

        return await rootstockService.ValidateSalesOrders(request.SalesOrders);
    }
}
