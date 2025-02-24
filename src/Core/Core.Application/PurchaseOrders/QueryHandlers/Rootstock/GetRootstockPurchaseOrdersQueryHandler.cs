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

            var distinctPurchaseOrdersIds = PurchaseOrder.GetDistinctPurchaseOrdersIds(purchaseOrderReceipts);
            if (!distinctPurchaseOrdersIds.Any())
            {
                return Result.Ok(Enumerable.Empty<PurchaseOrder>());
            }

            var purchaseOrdersResult = await rootstockService.GetPurchaseOrdersAsync(distinctPurchaseOrdersIds);
            if (purchaseOrdersResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersResult.Errors);
            }

            var purchaseOrders = purchaseOrdersResult.Value ?? [];

            var purchaseOrdersLineItemResult = await rootstockService.GetPurchaseOrdersLineItemAsync(distinctPurchaseOrdersIds);
            if (purchaseOrdersLineItemResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersLineItemResult.Errors);
            }

            var companyReferencesResult = await rootstockService.GetCompanyReferencesAsync();
            if (companyReferencesResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(companyReferencesResult.Errors);
            }

            return await GetFormattedPurchaseOrdersAsync(purchaseOrders, companyReferencesResult.Value,
                purchaseOrderReceipts, purchaseOrdersLineItemResult.Value);
        }

        private async Task<Result<IEnumerable<PurchaseOrder>>> GetFormattedPurchaseOrdersAsync(IEnumerable<PurchaseOrder> purchaseOrders, IEnumerable<CompanyReference> companyReferences,
            IEnumerable<PurchaseOrderReceipt> purchaseOrderReceipts, IEnumerable<PurchaseOrderLineItem> purchaseOrderLineItems)
        {
            var tasks = PurchaseOrder.FilterPurchaseOrders(purchaseOrders, companyReferences).Select(async purchaseOrder =>
            {
                purchaseOrder.UpdatePurchaseOrder(purchaseOrderReceipts, purchaseOrderLineItems);
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
