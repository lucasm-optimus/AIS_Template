using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.Rootstock;

namespace Tilray.Integrations.Functions.UseCases.PurchaseOrders.Rootstock;

public class GetPurchaseOrdersFromRootstockCRON(IMediator mediator)
{
    #region Function Implementation

    [Function("GetPurchaseOrdersFromRootstockCRON")]
    [ServiceBusOutput(Topics.RootstockPurchaseOrdersFetched, Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<PurchaseOrder>> Run([TimerTrigger("%GetPurchaseOrdersFromRootstockCRON%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetRootstockPurchaseOrders());
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }
        return [];
    }

    #endregion
}
