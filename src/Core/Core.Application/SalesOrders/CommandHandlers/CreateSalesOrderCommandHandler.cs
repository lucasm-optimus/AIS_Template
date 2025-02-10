using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
{
    public class CreateSalesOrderCommandHandler(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger) : ICommandHandler<CreateSalesOrderCommand, SalesOrderCreated>
    {
        public async Task<Result<SalesOrderCreated>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            if (await rootstockService.SalesOrderExists(request.SalesOrder.ECommerceOrderID))
            {
                logger.LogInformation($"[{request.CorrelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                // check the return value
                return Result.Fail($"[{request.CorrelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
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

            //SalesOrder-ProcessHeader
            var rotstockSalesOrderResult = salesAgg.ProcessRootstockHeader();
            logger.LogInformation($"[{request.CorrelationId}] Processed rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //Create SalesOrder
            var CreatedSalesOrderResult = await rootstockService.CreateSalesOrder(rotstockSalesOrderResult.Value);
            logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //SalesOrder-ProcessLineItems if SalesOrder-ProcessHeader is successful
            if (CreatedSalesOrderResult.IsSuccess)
            {
                // SalesOrder-ProcessLineItems
                for (int i = 1; i < request.SalesOrder.LineItems.Count; i++)
                {
                    var currentLineItem = request.SalesOrder.LineItems[i];
                    var rstkSalesOrderLineItemResult = salesAgg.ProcessRootstockLineItem(i, CreatedSalesOrderResult);
                    logger.LogInformation($"[{request.CorrelationId}] Processed rootstock sales order line item: {currentLineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

                    if (rstkSalesOrderLineItemResult.IsSuccess)
                    {
                        await rootstockService.CreateSalesOrderLineItem(rstkSalesOrderLineItemResult.Value);
                        logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order line item: {currentLineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    }
                    else
                    {
                        logger.LogError($"[{request.CorrelationId}] Creating rootstock sales order line item: {currentLineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
                        return Result.Fail(rstkSalesOrderLineItemResult.Errors);
                    }
                }

                //Prepayment - Process
                if (request.SalesOrder.CCPrepayment != null)
                {
                    var rstkPrePaymentResult = salesAgg.ProcessCCPrePayment();
                    if (rstkPrePaymentResult.IsSuccess)
                    {
                        await rootstockService.CreatePrePayment(rstkPrePaymentResult.Value);
                        logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    }
                    else
                    {
                        logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
                        return Result.Fail(rstkPrePaymentResult.Errors);
                    }
                }

                //Standard Prepayment If insurance payment is not made
                if (request.SalesOrder.StandardPrepayment != null)
                {
                    var rstkPrePaymentResult = salesAgg.ProcessCCPrePayment();
                    if (rstkPrePaymentResult.IsSuccess)
                    {
                        await rootstockService.CreatePrePayment(rstkPrePaymentResult.Value);
                        logger.LogInformation($"[{request.CorrelationId}] Created rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                    }
                    else
                    {
                        logger.LogError($"[{request.CorrelationId}] Creating rootstock prepayment for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID} failed.");
                        return Result.Fail(rstkPrePaymentResult.Errors);
                    }
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
