using Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.Rootstock;

namespace Tilray.Integrations.Functions.UseCases.PurchaseOrders.Rootstock;

public class GetPurchaseOrdersFromRootstockCRON(IMediator mediator)
{
    #region Function Implementation

    [Function("GetPurchaseOrdersFromRootstockCRON")]
    public async Task Run([TimerTrigger("%GetPurchaseOrdersFromRootstockCRON%", RunOnStartup = true)] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetRootstockPurchaseOrders());
        if (result.IsSuccess && result.Value != null)
        {
            await mediator.Publish(result.Value);
        }
    }

    #endregion
}
