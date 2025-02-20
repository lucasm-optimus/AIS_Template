/*using Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.OBeer;

namespace Tilray.Integrations.Functions.UseCases.PurchaseOrders.OBeer;

public class GetPurchaseOrdersFromOBeer(ILogger<GetPurchaseOrdersFromOBeer> _logger, IMediator mediator, ServiceBusClient serviceBusClient)
{
    #region Function Implementation
    [Function("GetPurchaseOrderFromOBeer")]
    public async Task<IEnumerable<PurchaseOrder>> Run([TimerTrigger("%GetPurchaseOrderFromOBeer%", RunOnStartup = true)] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetOBeerPurchaseOrders());

        if (result.IsSuccess && result.Value != null)
        {
        }

        return new List<PurchaseOrder>();
    }
    #endregion
}
*/