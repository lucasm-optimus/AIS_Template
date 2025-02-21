using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Core.Application.PurchaseOrders.CommandHandlers
{
    public class CreatePurchaseOrderInSAPConcurCommandHandler(ISAPConcurService sapConcurService, IMediator mediator) : ICommandHandler<CreatePurchaseOrderInSAPConcurCommand, SAPConcurPurchaseOrdersProcessed>
    {
        public async Task<Result<SAPConcurPurchaseOrdersProcessed>> Handle(CreatePurchaseOrderInSAPConcurCommand request, CancellationToken cancellationToken)
        {
            var purchaseOrdersProcessingResult = new SAPConcurPurchaseOrdersProcessed
            {
                ProcessedPurchaseOrders = new List<ProcessedPurchaseOrder>(),
                FailedPurchaseOrders = new List<FailedPurchaseOrder>()
            };

            await ProcessPurchaseOrder(request.PurchaseOrder, purchaseOrdersProcessingResult);

            // Publish the PurchaseOrdersProcessed event
            await mediator.Publish(purchaseOrdersProcessingResult, cancellationToken);

            return Result.Ok(purchaseOrdersProcessingResult);
        }

        private async Task ProcessPurchaseOrder(PurchaseOrder purchaseOrder, SAPConcurPurchaseOrdersProcessed purchaseOrdersProcessingResult)
        {
            SAPConcurCustomValues customFields = await GetCustomFieldsForVendor(purchaseOrder);

            var purchaseOrderExistsResult = await sapConcurService.PurchaseOrderExistsAsync(purchaseOrder.PurchaseOrderNumber);

            if (purchaseOrderExistsResult.IsSuccess && purchaseOrderExistsResult.Value)
            {
                var updateResult = await sapConcurService.UpdatePurchaseOrderAsync(purchaseOrder, customFields);
                if (updateResult.IsFailed)
                {
                    purchaseOrdersProcessingResult.AddFailedPurchaseOrder(GroupNames.PurchaseOrder, purchaseOrder.PurchaseOrderNumber, updateResult.Errors.FirstOrDefault()?.Message);
                }
            }
            else
            {
                var createResult = await sapConcurService.CreatePurchaseOrderAsync(purchaseOrder, customFields);
                if (createResult.IsFailed)
                {
                    purchaseOrdersProcessingResult.AddFailedPurchaseOrder(GroupNames.PurchaseOrder, purchaseOrder.PurchaseOrderNumber, createResult.Errors.FirstOrDefault()?.Message);
                }
                else
                {
                    purchaseOrdersProcessingResult.AddProcessedPurchaseOrder(GroupNames.PurchaseOrder, purchaseOrder.PurchaseOrderNumber);
                }
            }

            if (purchaseOrder.PurchaseOrderReceipts != null)
            {
                await ProcessPurchaseOrderReceipts(purchaseOrder.PurchaseOrderReceipts, purchaseOrdersProcessingResult);
            }
        }

        private async Task<SAPConcurCustomValues> GetCustomFieldsForVendor(PurchaseOrder purchaseOrder)
        {
            SAPConcurCustomValues customFields = new();
            var vendorResult = await sapConcurService.GetVendorAsync(purchaseOrder);
            if (vendorResult.IsSuccess && vendorResult.Value?.Vendor?.Any() == true)
            {
                customFields = await SetVendorCustomsAsync(vendorResult.Value.Vendor[0]);
            }

            return customFields;
        }

        private async Task ProcessPurchaseOrderReceipts(IEnumerable<PurchaseOrderReceipt> purchaseOrderReceipts, SAPConcurPurchaseOrdersProcessed purchaseOrdersProcessingResult)
        {
            var receiptTasks = purchaseOrderReceipts.Select(async receipt =>
            {
                var purchaseOrderReceiptExistsResult = await sapConcurService.PurchaseOrderReceiptExistsAsync(receipt);

                if (purchaseOrderReceiptExistsResult.IsSuccess && purchaseOrderReceiptExistsResult.Value)
                {
                    var updateResult = await sapConcurService.UpdatePurchaseOrderReceiptAsync(receipt);
                    if (updateResult.IsFailed)
                    {
                        purchaseOrdersProcessingResult.AddFailedPurchaseOrder(GroupNames.PurchaseOrderReceipt, receipt.PurchaseOrderNumber, updateResult.Errors.FirstOrDefault()?.Message);
                    }
                }
                else
                {
                    var createResult = await sapConcurService.CreatePurchaseOrderReceiptAsync(receipt);
                    if (createResult.IsFailed)
                    {
                        purchaseOrdersProcessingResult.AddFailedPurchaseOrder(GroupNames.PurchaseOrderReceipt, receipt.PurchaseOrderNumber, createResult.Errors.FirstOrDefault()?.Message);
                    }
                    else
                    {
                        purchaseOrdersProcessingResult.AddProcessedPurchaseOrder(GroupNames.PurchaseOrderReceipt, receipt.PurchaseOrderNumber);
                    }
                }
            });

            await Task.WhenAll(receiptTasks);
        }

        private async Task<SAPConcurCustomValues> SetVendorCustomsAsync(SAPConcurCustomValues vendor)
        {
            var customFields = new SAPConcurCustomValues();

            var custom1Result = await GetCustomFieldAsync("e9eaccb8-4c0e-274b-b6ed-58a7de10fc60", vendor.Custom1, true);
            customFields.Custom1 = custom1Result.ShortCode;

            var custom2Result = await GetCustomFieldAsync(custom1Result.Id, vendor.Custom2);
            customFields.Custom2 = custom2Result.ShortCode;

            var custom3Result = await GetCustomFieldAsync(custom2Result.Id, vendor.Custom3);
            customFields.Custom3 = custom3Result.ShortCode;

            var custom4Result = await GetCustomFieldAsync(custom3Result.Id, vendor.Custom4);
            customFields.Custom4 = custom4Result.ShortCode;

            return customFields;
        }

        private async Task<(string Id, string ShortCode)> GetCustomFieldAsync(string parentId, string customValue, bool isCustom1 = false)
        {
            Result<VendorCustomResponse> customResult;

            if (isCustom1)
            {
                customResult = await sapConcurService.GetVendorCustomAsync(parentId, customValue);
            }
            else
            {
                customResult = await sapConcurService.GetVendorCustomsAsync(parentId, customValue);
            }

            if (customResult.IsSuccess && customResult.Value?.Content?.Any() == true)
            {
                var content = customResult.Value.Content[0];
                return (content?.Id ?? string.Empty, content?.ShortCode ?? string.Empty);
            }
            return (string.Empty, string.Empty);
        }
    }
}