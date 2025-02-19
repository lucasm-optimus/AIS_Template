using Tilray.Integrations.Core.Application.SalesOrdersPayments.QueryHandlers;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;

namespace Tilray.Integrations.Functions.UseCases.SalesOrdersPayments.Rootstock;

public class GetSalesOrdersPaymentsFromRootstock(IMediator mediator)
{
    [Function("GetSalesOrdersPaymentsFromRootstock")]
    [ServiceBusOutput(Topics.SAPConcurExpensesFetched, Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<SalesOrderPayment>> Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetSalesOrdersPayments());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return [];
    }
}
