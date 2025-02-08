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
            var salesAgg = SalesAgg.Create(request.SalesOrder);

            //SalesOrder-ProcessHeader
            var rotstockSalesOrder = salesAgg.ProcessRootstockHeader();
            logger.LogInformation($"[{request.CorrelationId}] Processed rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //Create SalesOrder
            var CreatedSalesOrder = await rootstockService.CreateSalesOrder(rotstockSalesOrder);
            logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //SalesOrder-ProcessLineItems if SalesOrder-ProcessHeader is successful
            if (CreatedSalesOrder.Success)
            {
                // SalesOrder-ProcessLineItems
                for (int i = 1; i < request.SalesOrder.LineItems.Count; i++)
                {
                    var currentLineItem = request.SalesOrder.LineItems[i];
                    var rstkSalesOrderLineItem = salesAgg.ProcessRootstockLineItem(i, CreatedSalesOrder);
                    logger.LogInformation($"[{request.CorrelationId}] Processed rootstock sales order line item: {currentLineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

                    await rootstockService.CreateSalesOrderLineItem(rstkSalesOrderLineItem);
                    logger.LogInformation($"[{request.CorrelationId}] Created rootstock sales order line item: {currentLineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

                }

                //Prepayment - Process
                if (request.SalesOrder.CCPrepayment != null)
                {
                    var rstkPrePayment = salesAgg.ProcessCCPrePayment();
                    await rootstockService.CreatePrePayment(rstkPrePayment);
                }

                //Standard Prepayment If insurance payment is not made
                if (request.SalesOrder.StandardPrepayment != null)
                {
                    var rstkPrePayment = salesAgg.ProcessCCPrePayment();
                    await rootstockService.CreatePrePayment(rstkPrePayment);
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
