namespace Tilray.Integrations.Core.Application.PurchaseOrders.QueryHandlers.Rootstock
{
    public class GetRootstockPurchaseOrdersQueryHandler(IRootstockService rootstockService) : IQueryManyHandler<GetRootstockPurchaseOrders, PurchaseOrder>
    {
        public async Task<Result<IEnumerable<PurchaseOrder>>> Handle(GetRootstockPurchaseOrders request, CancellationToken cancellationToken)
        {
            var purchaseOrderReceiptsResult = await GetPurchaseOrderReceiptsAsync();
            if (purchaseOrderReceiptsResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrderReceiptsResult.Errors);
            }

            var purchaseOrderReceipts = purchaseOrderReceiptsResult.Value ?? Enumerable.Empty<PurchaseOrderReceipt>();

            var distinctPurchaseOrders = PurchaseOrder.GetDistinctPurchaseOrders(purchaseOrderReceipts);

            if (!distinctPurchaseOrders.Any())
            {
                return Result.Ok(Enumerable.Empty<PurchaseOrder>());
            }

            var purchaseOrdersResult = await GetPurchaseOrdersAsync(distinctPurchaseOrders);
            if (purchaseOrdersResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersResult.Errors);
            }

            var purchaseOrders = purchaseOrdersResult.Value ?? Enumerable.Empty<PurchaseOrder>();

            var purchaseOrdersLineItemResult = await GetPurchaseOrdersLineItemAsync(distinctPurchaseOrders);
            if (purchaseOrdersLineItemResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(purchaseOrdersLineItemResult.Errors);
            }

            var purchaseOrdersLineItem = purchaseOrdersLineItemResult.Value ?? Enumerable.Empty<PurchaseOrderLineItem>();

            var companyReferencesResult = await GetCompanyReferencesAsync();
            if (companyReferencesResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(companyReferencesResult.Errors);
            }

            var companyReferences = companyReferencesResult.Value ?? Enumerable.Empty<CompanyReference>();

            purchaseOrders = PurchaseOrder.FilterPurchaseOrders(purchaseOrders, companyReferences);

            var mapResult = await MapPurchaseOrdersAsync(purchaseOrders, purchaseOrderReceipts, purchaseOrdersLineItem);
            if (mapResult.IsFailed)
            {
                return Result.Fail<IEnumerable<PurchaseOrder>>(mapResult.Errors);
            }

            return Result.Ok(mapResult.Value);
        }

        private async Task<Result<IEnumerable<PurchaseOrderReceipt>>> GetPurchaseOrderReceiptsAsync()
        {
            var purchaseOrderReceiptsResult = await rootstockService.GetPurchaseOrderReceiptsAsync();
            return purchaseOrderReceiptsResult;
        }

        private async Task<Result<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersAsync(IEnumerable<string> distinctPurchaseOrders)
        {
            var purchaseOrdersResult = await rootstockService.GetPurchaseOrdersAsync(distinctPurchaseOrders);
            return purchaseOrdersResult;
        }

        private async Task<Result<IEnumerable<PurchaseOrderLineItem>>> GetPurchaseOrdersLineItemAsync(IEnumerable<string> distinctPurchaseOrders)
        {
            var purchaseOrdersLineItemResult = await rootstockService.GetPurchaseOrdersLineItemAsync(distinctPurchaseOrders);
            return purchaseOrdersLineItemResult;
        }

        private async Task<Result<IEnumerable<CompanyReference>>> GetCompanyReferencesAsync()
        {
            var companyReferencesResult = await rootstockService.GetCompanyReferencesAsync();
            return companyReferencesResult;
        }

        private async Task<Result<IEnumerable<PurchaseOrder>>> MapPurchaseOrdersAsync(IEnumerable<PurchaseOrder> purchaseOrders, IEnumerable<PurchaseOrderReceipt> purchaseOrderReceipts, IEnumerable<PurchaseOrderLineItem> purchaseOrdersLineItem)
        {
            var tasks = purchaseOrders.Select(async po =>
            {
                po.SetPurchaseOrdersReceipt(purchaseOrderReceipts);
                po.SetLineItems(purchaseOrdersLineItem);
                return await rootstockService.SetVendorAddressAndMap(po);
            });

            var results = await Task.WhenAll(tasks);

            var failedResults = results.Where(result => result.IsFailed).ToList();
            if (failedResults.Any())
            {
                var errors = failedResults.SelectMany(result => result.Errors).ToList();
                return Result.Fail<IEnumerable<PurchaseOrder>>(errors);
            }

            var successfulResults = results.Where(result => result.IsSuccess).Select(result => result.Value).ToList();

            return Result.Ok(successfulResults.AsEnumerable());
        }
    }
}