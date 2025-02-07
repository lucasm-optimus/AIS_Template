using MediatR;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Ecom.Commands
{
    public class ProcessSalesOrderCommandHandler(
        IRootstockService rootstockService,
        IMediator mediator,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<ProcessSalesOrdersCommand, SalesOrdersProcessed>
    {
        public async Task<Result<SalesOrdersProcessed>> Handle(ProcessSalesOrdersCommand request, CancellationToken cancellationToken)
        {
            #region Preparing variables

            logger.LogInformation($"[{request.correlationId}] Begin processing {request.SalesOrders.Count()} sales orders.");

            var salesOrders = new List<SalesOrder>();
            var failedSalesOrders = new List<(SalesOrder salesOrder, IEnumerable<string> messages)>();

            #endregion

            #region Parallel processing of all orders

            var tasks = request.SalesOrders.Select(async salesOrder =>
            {
                var response = await ProcessIndividualSalesOrder(salesOrder, request.correlationId);
                if (response.IsSuccess && response.Value != null)
                {
                    salesOrders.Add(response.Value);
                }
                else
                {
                    failedSalesOrders.Add((response.Value, response.Reasons.Select(r => r.Message)));
                }
            });

            await Task.WhenAll(tasks);

            #endregion

            #region Logging process information

            if (failedSalesOrders.Any())
            {
                logger.LogWarning($"[{request.correlationId}] Failed to process {failedSalesOrders.Count()} sales orders.");

                foreach (var failedSalesOrder in failedSalesOrders)
                {
                    foreach (var message in failedSalesOrder.messages)
                    {
                        logger.LogWarning($"[{request.correlationId}] Sales Order {failedSalesOrder.salesOrder.ECommerceOrderID} failed to process: {message}");
                    }
                }
            }

            logger.LogInformation($"[{request.correlationId}] Successfully processed {salesOrders.Count()} sales orders.");

            #endregion

            return Result.Ok(new SalesOrdersProcessed(salesOrders));
        }

        private async Task<Result<SalesOrder>> ProcessIndividualSalesOrder(Models.Ecom.SalesOrder payload, string correlationId)
        {
            #region Preparing Sales Agg

            var result = Result.Ok();
            logger.LogInformation($"[{correlationId}]  Processing sales order {payload.ECommOrderID}");

            var salesAgg = SalesAgg.Create(payload.StoreName);
            logger.LogInformation($"[{correlationId}] Created sales order aggregate for {payload.ECommOrderID}");

            #endregion

            #region Validate Sales Orders

            var salesOrderValidated = salesAgg.ValidateSalesOrder(payload);
            logger.LogInformation($"[{correlationId}] Validated sales order {payload.ECommOrderID}");

            if (!salesOrderValidated.result)
            {
                logger.LogWarning($"[{correlationId}] Sales Order {payload.ECommOrderID} failed validation.");
                return Result.Fail<SalesOrder>(salesOrderValidated.messages);
            }

            #endregion

            #region Create Sales Order

            var salesOrderProcessed = salesAgg.CreateSalesOrder(payload, orderDefaults);
            logger.LogInformation($"[{correlationId}] Created sales order {payload.ECommOrderID}");

            #endregion

            #region Create Customer and Customer Address if does not exists

            var customerInfo = await rootstockService.GetCustomerInfo(payload.CustomerAccountNumber);
            if (customerInfo == null)
            {
                await mediator.Send(new CreateCustomerCommand(salesOrderProcessed.SalesOrderCustomer, correlationId));
                logger.LogInformation($"[{correlationId}] Created customer {payload.CustomerAccountNumber} for sales order {payload.ECommOrderID}");
            }

            var customerAddressInfo = await rootstockService.GetCustomerAddressInfo(payload.CustomerAccountNumber, payload.ShipToAddress1, payload.ShipToCity, payload.ShipToState, payload.ShipToZip);
            if (customerAddressInfo == null)
            {
                var response = (await mediator.Send(new CreateCustomerAddressCommand(salesOrderProcessed.SalesOrderCustomerAddress, payload.CustomerAccountID, payload.CustomerAccountNumber, correlationId))).Value;

                customerAddressInfo = response.CustomerAddressInfo;
                logger.LogInformation($"[{correlationId}] Created customer address {payload.CustomerAccountNumber} for sales order {payload.ECommOrderID}");
            }

            salesAgg.UpdateCustomerAddressReference(customerAddressInfo.CustomerID);

            #endregion

            return Result.Ok(salesAgg.SalesOrder);
        }
    }
}