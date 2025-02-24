namespace Tilray.Integrations.Core.Application.SalesOrders.CommandHandlers
{
    public class CreateSalesOrderCommandHandler(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger) : ICommandHandler<CreateSalesOrderCommand, SalesOrderCreated>
    {
        public async Task<Result<SalesOrderCreated>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            if (await SalesOrderExists(request))
            {
                return Result.Ok();
            }

            var updateForeignKeysResult = await UpdateForeignKeys(request);
            if (updateForeignKeysResult.IsFailed)
            {
                return updateForeignKeysResult;
            }

            return await CreateSalesOrder(request);
        }

        private async Task<bool> SalesOrderExists(CreateSalesOrderCommand request)
        {
            if (await rootstockService.SalesOrderExists(request.SalesOrder.CustomerReference))
            {
                logger.LogInformation("Sales order already exists for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                return true;
            }
            return false;
        }

        private async Task<Result> UpdateForeignKeys(CreateSalesOrderCommand request)
        {
            var updateTasks = new List<Task<Result>>
                {
                    UpdateForeignKey(request, "rstk__sootype__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{request.SalesOrder.OrderType}", request.SalesOrder.UpdateOrderId, "order type id"),
                    UpdateForeignKey(request, "rstk__syshipviatype__c", "rstk__externalid__c", request.SalesOrder.ShippingMethod, request.SalesOrder.UpdateShipViaId, "ship via id"),
                    UpdateForeignKey(request, "rstk__sycarrier__c", "rstk__externalid__c", request.SalesOrder.ShippingCarrier, request.SalesOrder.UpdateCarrierId, "carrier id"),
                    UpdateForeignKey(request, "rstk__socaddr__c", "rstk__externalid__c", request.SalesOrder.CustomerAddressReference, request.SalesOrder.UpdateCustomerAddressId, "customer address id"),
                    UpdateProductIds(request)
                };

            var results = await Task.WhenAll(updateTasks);
            var errors = results.Where(result => result.IsFailed).SelectMany(result => result.Errors);
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

        private async Task<Result> UpdateForeignKey(CreateSalesOrderCommand request, string objectName, string externalIdColumnName, string externalIdValue, Action<string> updateAction, string idType)
        {
            var result = await rootstockService.GetIdFromExternalColumnReference(objectName, externalIdColumnName, externalIdValue);
            if (result.IsFailed)
            {
                logger.LogError("Getting {IdType} failed for ECommerceOrderID:{ECommerceOrderID}.", idType, request.SalesOrder.ECommerceOrderID);
                return Result.Fail($"Getting {idType} failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }
            updateAction(result.Value);
            return Result.Ok();
        }

        private async Task<Result> UpdateProductIds(CreateSalesOrderCommand request)
        {
            var updateTasks = request.SalesOrder.LineItems.Select(async item =>
            {
                var result = await rootstockService.GetIdFromExternalColumnReference("rstk__soprod__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{item.ItemNumber}");
                if (result.IsFailed)
                {
                    logger.LogError("Getting product id failed for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                    return Result.Fail($"Getting product id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }
                item.UpdateProductId(result.Value);
                return Result.Ok();
            });

            var results = await Task.WhenAll(updateTasks);
            return results.FirstOrDefault(result => result.IsFailed) ?? Result.Ok();
        }

        private async Task<Result<SalesOrderCreated>> CreateSalesOrder(CreateSalesOrderCommand request)
        {
            logger.LogInformation("Creating rootstock sales order started for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);

            var createdSalesOrderResult = await rootstockService.CreateSalesOrder(request.SalesOrder);
            logger.LogInformation("Created rootstock sales order header for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);

            if (createdSalesOrderResult.IsSuccess)
            {
                var createdSoHdrResult = await rootstockService.GetSoHdr(createdSalesOrderResult.Value);
                if (createdSoHdrResult.IsFailed)
                {
                    logger.LogError("Getting rootstock sales order header failed for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                    return Result.Fail($"Getting rootstock sales order header failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                await CreateLineItems(request.SalesOrder, createdSoHdrResult.Value);
                await CreatePrePayments(request.SalesOrder, createdSoHdrResult.Value, request);

                logger.LogInformation("SalesOrderProcessed_CreateInRootStock: Sales order created in RootStock.");
                return Result.Ok(new SalesOrderCreated());
            }
            else
            {
                logger.LogError("Creating rootstock sales order header for ECommerceOrderID:{ECommerceOrderID} failed.", request.SalesOrder.ECommerceOrderID);
                return Result.Fail($"Creating rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
            }
        }

        private async Task<Result> CreateLineItems(MedSalesOrder salesOrder, string soHdrId)
        {
            var result = Result.Ok();

            for (int i = 1; i < salesOrder.LineItems.Count; i++)
            {
                var createSalesOrderLineItemResult = await rootstockService.CreateSalesOrderLineItem(salesOrder.LineItems[i], soHdrId);

                if (createSalesOrderLineItemResult.IsFailed)
                    return Result.Fail(createSalesOrderLineItemResult.Errors);
                else
                    logger.LogInformation("Created rootstock sales order line item for ECommerceOrderID:{ECommerceOrderID}.", salesOrder.ECommerceOrderID);
            }

            return result;
        }

        private async Task<Result> CreatePrePayments(MedSalesOrder salesOrder, string soHdrId, CreateSalesOrderCommand request)
        {
            var syDataPrePayments = await ExecuteSyDataPrePayments(rootstockService, logger, salesOrder, soHdrId, request);
            if (syDataPrePayments.IsFailed)
                return syDataPrePayments;

            return await ExecutePrePayments(rootstockService, logger, salesOrder, soHdrId, request);
        }

        private async Task<Result> ExecutePrePayments(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger, MedSalesOrder salesOrder, string soHdrId, CreateSalesOrderCommand request)
        {
            var prePaymentResult = salesOrder.HasPrepayment(soHdrId, "20011700");
            if (prePaymentResult.IsSuccess)
            {
                var result = Result.Ok();

                var divisionResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sydiv__c", "rstk__externalid__c", request.SalesOrder.Division);
                result.WithErrors(divisionResult.Errors);

                var paymentAccountResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syacc__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{prePaymentResult.Value.PrepaymentAccount}");
                result.WithErrors(paymentAccountResult.Errors);

                if (result.IsFailed)
                    return result;

                var createdPrepaymentResult = await rootstockService.CreatePrePayment(prePaymentResult.Value, divisionResult.Value, paymentAccountResult.Value);
                if (createdPrepaymentResult.IsFailed)
                {
                    logger.LogError("Creating rootstock prepayment failed for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                    return Result.Fail(createdPrepaymentResult.Errors);
                }
                else
                {
                    logger.LogInformation("Created rootstock prepayment for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                }

                return Result.Ok();
            }

            return Result.Fail("Sales order has no payments");
        }

        private async Task<Result> ExecuteSyDataPrePayments(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger, MedSalesOrder salesOrder, string soHdrId, CreateSalesOrderCommand request)
        {
            var hasPrepaymentDataResult = salesOrder.HasPrePaymentData();
            if (hasPrepaymentDataResult.IsSuccess)
            {
                var createdPrePaymentResult = await rootstockService.CreatePrePayment(hasPrepaymentDataResult.Value);
                if (createdPrePaymentResult.IsFailed)
                {
                    logger.LogError("Creating rootstock prepayment failed for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                    return Result.Fail(createdPrePaymentResult.Errors);
                }
                else
                {
                    logger.LogInformation("Created rootstock prepayment for ECommerceOrderID:{ECommerceOrderID}.", request.SalesOrder.ECommerceOrderID);
                }
            }

            return Result.Ok();
        }
    }
}
