using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;

namespace Tilray.Integrations.Core.Application.SalesOrdersPayments.QueryHandlers;

public class GetSalesOrdersPaymentsQueryHandler(IRootstockService rootstockService) : IQueryManyHandler<GetSalesOrdersPayments, SalesOrderPayment>
{
    public async Task<Result<IEnumerable<SalesOrderPayment>>> Handle(GetSalesOrdersPayments request, CancellationToken cancellationToken)
    {
        return await rootstockService.GetSalesOrdersPaymentsAsync();
    }
}
