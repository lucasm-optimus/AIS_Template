using Tilray.Integrations.Core.Application.Adapters.Repositories;

namespace Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.OBeer;

public class GetOBeerPurchaseOrdersQueryHandler(ISnowflakeRepository snowflakeRepository) : IQueryManyHandler<GetOBeerPurchaseOrders, PurchaseOrder>
{
    public async Task<Result<IEnumerable<PurchaseOrder>>> Handle(GetOBeerPurchaseOrders request, CancellationToken cancellationToken)
    {
        /*var purchaseOrdersResult = await snowflakeRepository.GetPurchaseOrdersAsync();

        if (purchaseOrdersResult.IsFailed)
        {
            return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersResult.Errors);
        }

        var purchaseOrders = PurchaseOrder.GroupByPurchaseOrderNumber(purchaseOrdersResult.Value);

        foreach (var purchaseOrder in purchaseOrders)
        {
            purchaseOrder.FilterAndDistinctLineItemsAndReceipts();
        }

        var filteredPurchaseOrders = PurchaseOrder.FilterNonEmptyLineItems(purchaseOrders);

        return Result.Ok(filteredPurchaseOrders.AsEnumerable());*/

        return null;
    }
}