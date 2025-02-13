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
            if (await rootstockService.SalesOrderExists(request.SalesOrder.CustomerReference))
            {
                logger.LogInformation($"[{request.CorrelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                // check the return value
                return Result.Fail($"[{request.CorrelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            // Get OrderType Id
            var orderTypeResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sootype__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{request.SalesOrder.OrderType}");
            if (orderTypeResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting order type id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting order type id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            request.SalesOrder.UpdateOrderId(orderTypeResult.Value);

            // Get ShipVia Id
            var shipViaResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syshipviatype__c", "rstk__externalid__c", request.SalesOrder.ShippingMethod);
            if (shipViaResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting ship via id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting ship via id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            request.SalesOrder.UpdateShipViaId(shipViaResult.Value);

            // Get carrier Id
            var carrierResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sycarrier__c", "rstk__externalid__c", request.SalesOrder.ShippingCarrier);
            if (carrierResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting carrier id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting carrier id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            request.SalesOrder.UpdateCarrierId(carrierResult.Value);

            // Get Customer address id
            var customerAddressResult = await rootstockService.GetIdFromExternalColumnReference("rstk__socaddr__c", "rstk__externalid__c", request.SalesOrder.CustomerAddressReference);
            if (customerAddressResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Getting customer address id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Getting customer address id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            request.SalesOrder.UpdateCustomerAddressId(customerAddressResult.Value);


            // Get product ids
            foreach (var item in request.SalesOrder.LineItems)
            {
                var customerProdResult = await rootstockService.GetIdFromExternalColumnReference("rstk__soprod__c", "rstk__externalid__c", $"{request.SalesOrder.Division}_{item.ItemNumber}");
                if (customerProdResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Getting product id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail($"[{request.CorrelationId}] Getting product id failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                item.UpdateProductId(customerProdResult.Value);
            }



            // Create Sales Agg
            logger.LogInformation($"[{request.CorrelationId}] Creating rootstock sales order started for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            var salesAggResult = SalesAgg.Create(request.SalesOrder);
            if (salesAggResult.IsFailed)
            {
                logger.LogError($"[{request.CorrelationId}] Creating rootstock sales order failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                return Result.Fail($"[{request.CorrelationId}] Creating rootstock sales order failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
            }

            var salesAgg = salesAggResult.Value;

            //Create SalesOrder
            var CreatedSalesOrderResult = await rootstockService.CreateSalesOrder(salesAgg.RootstockSalesOrder);
            logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //SalesOrder-ProcessLineItems if SalesOrder-ProcessHeader is successful
            if (CreatedSalesOrderResult.IsSuccess)
            {
                // update sales order id in line items
                var createdSoHdrResult = await rootstockService.GetSoHdr(CreatedSalesOrderResult.Value);

                if (createdSoHdrResult.IsFailed)
                {
                    logger.LogError($"[{request.CorrelationId}] Getting rootstock sales order header failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    return Result.Fail($"[{request.CorrelationId}] Getting rootstock sales order header failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                // SalesOrder-ProcessLineItems
                for (int i = 0; i < salesAgg.RootstockOrderLines.Count; i++)
                {
                    var currentLineItem = salesAgg.RootstockOrderLines[i];
                    currentLineItem.UpdateSoHdr(createdSoHdrResult.Value);
                    await rootstockService.CreateSalesOrderLineItem(currentLineItem);
                }

                //Standard Prepayment
                var prePaymentResult = salesAgg.SalesOrder.HasPrepayment(createdSoHdrResult.Value, "20011700");
                if (prePaymentResult.IsSuccess)
                {
                    var createdRootstockSoPrepayment = RstkSalesOrderPrePayment.Create(prePaymentResult.Value);
                    if (createdRootstockSoPrepayment.IsFailed)
                    {
                        logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                        return Result.Fail($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    }

                    await rootstockService.CreatePrePayment(createdRootstockSoPrepayment.Value);
                    logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                // Sy Data Prepayment -  CC order
                var syDataPrePaymentResult = salesAgg.HasSyDataPrePayment(createdSoHdrResult.Value);
                if (syDataPrePaymentResult.IsSuccess)
                {
                    var createdPrePaymentResult = await rootstockService.CreatePrePayment(syDataPrePaymentResult.Value);
                    if (createdPrePaymentResult.IsFailed)
                    {
                        logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                        return Result.Fail($"[{request.CorrelationId}] Creating rootstock prepayment failed for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    }

                    logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                return Result.Ok(new SalesOrderCreated());
            }
            else
            {
                logger.LogError($"[{request.CorrelationId}] Creating rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
                return Result.Fail($"[{request.CorrelationId}] Creating rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
            }
        }
    }
}
