using Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.Rootstock;

namespace Tilray.Integrations.Functions.UseCases.PurchaseOrders.Rootstock;

public class GetPurchaseOrdersFromRootstock(ILogger<GetPurchaseOrdersFromRootstock> _logger, IMediator mediator, ServiceBusClient serviceBusClient)
{
    #region Function Implementation
    [Function("GetPurchaseOrderFromRootstock")]
    public async Task Run([TimerTrigger("%GetPurchaseOrderFromRootstock%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetRootstockPurchaseOrders());
        if (result.IsSuccess && result.Value != null)
        {
            await mediator.Publish(result.Value);
        }
    }
    #endregion
}
