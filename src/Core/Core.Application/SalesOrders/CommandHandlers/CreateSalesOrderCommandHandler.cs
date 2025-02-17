using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
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
                logger.LogInformation($"[{request.CorrelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return true;
            }
            return false;
        }

        private async Task<Result> UpdateForeignKeys(CreateSalesOrderCommand request)
        {
            var updateTasks = new List<Task<Result>>
                {
                    UpdateOrderTypeId(request),
                    UpdateShipViaId(request),
                    UpdateCarrierId(request),
                    UpdateCustomerAddressId(request),
                    UpdateProductIds(request)
                };

            var results = await Task.WhenAll(updateTasks);
            return results.FirstOrDefault(result => result.IsFailed) ?? Result.Ok();
        }

        private async Task<Result> UpdateOrderTypeId(CreateSalesOrderCommand request)
        {
            var orderTypeResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sootype__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{request.SalesOrder.OrderType}");
            if (orderTypeResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting order type id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting order type id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }
            request.SalesOrder.UpdateOrderId(orderTypeResult.Value);
            return Result.Ok();
        }

        private async Task<Result> UpdateShipViaId(CreateSalesOrderCommand request)
        {
            var shipViaResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syshipviatype__c", "rstk__externalid__c", request.SalesOrder.ShippingMethod);
            if (shipViaResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting ship via id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting ship via id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }
            request.SalesOrder.UpdateShipViaId(shipViaResult.Value);
            return Result.Ok();
        }

        private async Task<Result> UpdateCarrierId(CreateSalesOrderCommand request)
        {
            var carrierResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sycarrier__c", "rstk__externalid__c", request.SalesOrder.ShippingCarrier);
            if (carrierResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting carrier id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting carrier id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }
            request.SalesOrder.UpdateCarrierId(carrierResult.Value);
            return Result.Ok();
        }

        private async Task<Result> UpdateCustomerAddressId(CreateSalesOrderCommand request)
        {
            var customerAddressResult = await rootstockService.GetIdFromExternalColumnReference("rstk__socaddr__c", "rstk__externalid__c", request.SalesOrder.CustomerAddressReference);
            if (customerAddressResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting customer address id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting customer address id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }
            request.SalesOrder.UpdateCustomerAddressId(customerAddressResult.Value);
            return Result.Ok();
        }

        private async Task<Result> UpdateProductIds(CreateSalesOrderCommand request)
        {
            var updateTasks = request.SalesOrder.LineItems.Select(async item =>
            {
                var customerProdResult = await rootstockService.GetIdFromExternalColumnReference("rstk__soprod__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{item.ItemNumber}");
                if (customerProdResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Getting product id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail($"[{request.CorrelationId}] Getting product id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }
                item.UpdateProductId(customerProdResult.Value);
                return Result.Ok();
            });

            var results = await Task.WhenAll(updateTasks);
            return results.FirstOrDefault(result => result.IsFailed) ?? Result.Ok();
        }

        private async Task<Result<SalesOrderCreated>> CreateSalesOrder(CreateSalesOrderCommand request)
        {
            logger.LogInformation($"[{request.CorrelationId}] Creating rootstock sales order started for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            var salesAggResult = SalesAgg.Create(request.SalesOrder);
            if (salesAggResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Creating rootstock sales order failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Creating rootstock sales order failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            var salesAgg = salesAggResult.Value;
            var createdSalesOrderResult = await rootstockService.CreateSalesOrder(salesAgg.RootstockSalesOrder);
            logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            if (createdSalesOrderResult.IsSuccess)
            {
                var createdSoHdrResult = await rootstockService.GetSoHdr(createdSalesOrderResult.Value);
                if (createdSoHdrResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Getting rootstock sales order header failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail($"[{request.CorrelationId}] Getting rootstock sales order header failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                await ProcessLineItems(salesAgg, createdSoHdrResult.Value);
                await ProcessPrePayments(salesAgg, createdSoHdrResult.Value, request);

                return Result.Ok(new SalesOrderCreated());
            }
            else
            {
                logger.LogError($"[{request.CorrelationId}] Creating rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
                return Result.Fail($"[{request.CorrelationId}] Creating rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
            }
        }

        private async Task<Result> ProcessLineItems(SalesAgg salesAgg, string soHdrId)
        {
            var result = Result.Ok();

            for (int i = 0; i < salesAgg.RootstockOrderLines.Count; i++)
            {
                var currentLineItem = salesAgg.RootstockOrderLines[i];
                currentLineItem.UpdateSoHdr(soHdrId);
                var createSalesOrderLineItemResult = await rootstockService.CreateSalesOrderLineItem(currentLineItem);
                
                if (createSalesOrderLineItemResult.IsFailed)
                    return Result.Fail(createSalesOrderLineItemResult.Errors);
            }

            return result;
        }

        private async Task<Result> ProcessPrePayments(SalesAgg salesAgg, string soHdrId, CreateSalesOrderCommand request)
        {
            var syDataPrePayments = await ExecuteSyDataPrePayments(rootstockService, logger, salesAgg, soHdrId, request);
            if (syDataPrePayments.IsFailed)
                return syDataPrePayments;

            return await ExecutePrePayments(rootstockService, logger, salesAgg, soHdrId, request);
        }

        private async Task<Result> ExecutePrePayments(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger, SalesAgg salesAgg, string soHdrId, CreateSalesOrderCommand request)
        {
            var prePaymentResult = salesAgg.SalesOrder.HasPrepayment(soHdrId, "20011700");
            if (prePaymentResult.IsSuccess)
            {
                var result = Result.Ok();

                var divisionResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sydiv__c", "rstk__externalid__c", request.SalesOrder.Division);
                result.WithErrors(divisionResult.Errors);

                var paymentAccountResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syacc__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{prePaymentResult.Value.PrepaymentAccount}");
                result.WithErrors(paymentAccountResult.Errors);

                if (result.IsFailed)
                    return result;

                var soPrepaymentResult = RstkSalesOrderPrePayment.Create(prePaymentResult.Value, divisionResult.Value, paymentAccountResult.Value);
                if (soPrepaymentResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail(soPrepaymentResult.Errors);
                }

                var createdPrepaymentResult = await rootstockService.CreatePrePayment(soPrepaymentResult.Value);
                if (createdPrepaymentResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail(createdPrepaymentResult.Errors);
                }
                else
                {
                    logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                return Result.Ok();
            }

            return Result.Fail("Sales order has no payments");
        }

        private async Task<Result> ExecuteSyDataPrePayments(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger, SalesAgg salesAgg, string soHdrId, CreateSalesOrderCommand request)
        {
            var syDataPrePaymentResult = salesAgg.SalesOrder.HasSyDataPrePayment(soHdrId);
            if (syDataPrePaymentResult.IsSuccess)
            {
                var createdPrePaymentResult = await rootstockService.CreatePrePayment(syDataPrePaymentResult.Value);
                if (createdPrePaymentResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail(createdPrePaymentResult.Errors);
                }
                else
                {
                    logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }
            }

            return Result.Ok();
        }
    }
}
