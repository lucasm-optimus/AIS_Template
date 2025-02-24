namespace Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.Rootstock
{
    public class GetRootstockPurchaseOrdersQueryHandler(IRootstockService rootstockService) : IQueryManyHandler<GetRootstockPurchaseOrders, PurchaseOrder>
    {
        public async Task<Result<IEnumerable<PurchaseOrder>>> Handle(GetRootstockPurchaseOrders request, CancellationToken cancellationToken)
        {
            var purchaseOrderReceiptsResult = await rootstockService.GetPurchaseOrderReceiptsAsync();
            if (purchaseOrderReceiptsResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrderReceiptsResult.Errors);
            }

            var purchaseOrderReceipts = purchaseOrderReceiptsResult.Value ?? [];

            var distinctPurchaseOrders = PurchaseOrder.GetDistinctPurchaseOrders(purchaseOrderReceipts);
            if (!distinctPurchaseOrders.Any())
            {
                return Result.Ok(Enumerable.Empty<PurchaseOrder>());
            }

            var purchaseOrdersResult = await rootstockService.GetPurchaseOrdersAsync(distinctPurchaseOrders);
            if (purchaseOrdersResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersResult.Errors);
            }

            var purchaseOrders = purchaseOrdersResult.Value ?? [];

            var purchaseOrdersLineItemResult = await rootstockService.GetPurchaseOrdersLineItemAsync(distinctPurchaseOrders);
            if (purchaseOrdersLineItemResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersLineItemResult.Errors);
            }

            var purchaseOrdersLineItem = purchaseOrdersLineItemResult.Value ?? [];

            var companyReferencesResult = await rootstockService.GetCompanyReferencesAsync();
            if (companyReferencesResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(companyReferencesResult.Errors);
            }

            var companyReferences = companyReferencesResult.Value ?? [];

            purchaseOrders = PurchaseOrder.FilterPurchaseOrders(purchaseOrders, companyReferences);

            var mapResult = await MapPurchaseOrdersAsync(purchaseOrders, purchaseOrderReceipts, purchaseOrdersLineItem);
            if (mapResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(mapResult.Errors);
            }

            return Result.Ok(mapResult.Value);
        }

        private async Task<Result<IEnumerable<PurchaseOrder>>> MapPurchaseOrdersAsync(IEnumerable<PurchaseOrder> purchaseOrders, IEnumerable<PurchaseOrderReceipt> purchaseOrderReceipts, IEnumerable<PurchaseOrderLineItem> purchaseOrdersLineItem)
        {
            var tasks = purchaseOrders.Select(async purchaseOrder =>
            {
                purchaseOrder.SetPurchaseOrdersReceipt(purchaseOrderReceipts);
                purchaseOrder.SetLineItems(purchaseOrdersLineItem);
                return await rootstockService.SetVendorAddressNumberAsync(purchaseOrder);
            });

            var results = await Task.WhenAll(tasks);

            var failedResults = results.Where(result => result.IsFailed).ToList();
            if (failedResults.Count != 0)
            {
                var errors = failedResults.SelectMany(result => result.Errors).ToList();
                return Result.Fail<IEnumerable<PurchaseOrder>>(errors);
            }

            return Result.Ok(purchaseOrders);
        }
    }
}
