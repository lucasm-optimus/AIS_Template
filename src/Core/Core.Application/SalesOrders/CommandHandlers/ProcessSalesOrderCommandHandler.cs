using MediatR;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

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

            var salesOrders = new List<MedSalesOrder>();
            var failedSalesOrders = new List<(MedSalesOrder salesOrder, IEnumerable<string> messages)>();

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

        private async Task<Result<MedSalesOrder>> ProcessIndividualSalesOrder(Models.Ecom.SalesOrder payload, string correlationId)
        {
            #region Preparing Sales Agg

            var result = Result.Ok();
            logger.LogInformation($"[{correlationId}]  Processing sales order {payload.ECommOrderID}");

            #endregion

            #region Validate Sales Orders

            var salesOrderValidatationResult = SalesAgg.ValidateSalesOrder(payload);
            logger.LogInformation($"[{correlationId}] Validated sales order {payload.ECommOrderID}");

            if (salesOrderValidatationResult.IsFailed)
            {
                logger.LogWarning($"[{correlationId}] Sales Order {payload.ECommOrderID} failed validation.");
                return Result.Fail<MedSalesOrder>(salesOrderValidatationResult.Errors);
            }

            #endregion

            #region Create Sales Order

            var salesAggResult = SalesAgg.Create(payload.StoreName);
            if (salesAggResult.IsFailed)
            {
                logger.LogWarning($"[{correlationId}] Failed to create sales order aggregate for {payload.ECommOrderID}");
                return Result.Fail<MedSalesOrder>(salesAggResult.Errors);
            }

            var salesAgg = salesAggResult.Value;
            logger.LogInformation($"[{correlationId}] Created sales order aggregate for {payload.ECommOrderID}");

            var salesOrderProcessigResult = salesAgg.Process(payload, orderDefaults);
            if (salesOrderProcessigResult.IsFailed)
            {
                logger.LogWarning($"[{correlationId}] Failed to process sales order {payload.ECommOrderID}");
                return Result.Fail<MedSalesOrder>(salesOrderProcessigResult.Errors);
            }
            logger.LogInformation($"[{correlationId}] Created sales order {payload.ECommOrderID}");

            #endregion

            #region Create Customer and Customer Address if does not exists

            var customerInfoResult = await rootstockService.GetCustomerInfo(payload.CustomerAccountID);
            if (customerInfoResult.IsFailed)
            {
                var response = await mediator.Send(new CreateCustomerCommand(salesOrderProcessigResult.Value.SalesOrderCustomer, correlationId));
                if (response.IsSuccess)
                {
                    logger.LogInformation($"[{correlationId}] Created customer {payload.CustomerAccountID} for sales order {payload.ECommOrderID}");
                }
                else
                {
                    logger.LogWarning($"[{correlationId}] Failed to create customer {payload.CustomerAccountID} for sales order {payload.ECommOrderID}");
                    return Result.Fail<MedSalesOrder>(customerInfoResult.Errors);
                }
            }

            var customerAddressInfoResult = await rootstockService.GetCustomerAddressInfo(payload.CustomerAccountID, payload.ShipToAddress1, payload.ShipToCity, payload.ShipToState, payload.ShipToZip);
            if (customerAddressInfoResult.IsFailed)
            {
                var response = await mediator.Send(new CreateCustomerAddressCommand(salesOrderProcessigResult.Value.SalesOrderCustomerAddress, payload.CustomerAccountID, payload.CustomerAccountNumber, correlationId));
                if (response.IsSuccess)
                {
                    var customerAddressInfo = response.Value.CustomerAddressInfo;
                    salesAgg.UpdateCustomerAddressReference(customerAddressInfo.CustomerAddressID);
                    logger.LogInformation($"[{correlationId}] Created customer address {customerAddressInfo.CustomerAddressID} for sales order {payload.ECommOrderID}");
                }
                else
                {
                    var errorMessage = $"[{correlationId}] Failed to create customer address for sales order {payload.ECommOrderID} and customer no:{payload.CustomerAccountNumber}";
                    logger.LogWarning(errorMessage);
                    return Result.Fail<MedSalesOrder>(response.Errors);
                }
            }

            #endregion

            return Result.Ok(salesAgg.SalesOrder);
        }
    }
}