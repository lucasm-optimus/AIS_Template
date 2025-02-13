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

            logger.LogInformation($"Begin processing {request.SalesOrders.Count()} sales orders.");

            var salesOrders = new List<MedSalesOrder>();

            #endregion

            #region Parallel processing of all orders

            var tasks = request.SalesOrders.Select(async salesOrder =>
            {
                var response = await ProcessIndividualSalesOrder(salesOrder);
                if (response.IsSuccess)
                {
                    salesOrders.Add(response.Value);
                }
                else
                {
                    foreach (var message in response.Reasons.Select(r => r.Message).ToList())
                    {
                        logger.LogWarning($"Failed to process sales order:{salesOrder.ECommOrderID}, message: {message}");
                    }
                }
            });

            await Task.WhenAll(tasks);

            logger.LogInformation($"Successfully processed {salesOrders.Count()} sales orders.");

            #endregion

            return Result.Ok(new SalesOrdersProcessed(salesOrders, $"Successfully processed {salesOrders.Count()} sales orders."));
        }

        private async Task<Result<MedSalesOrder>> ProcessIndividualSalesOrder(Models.Ecom.SalesOrder payload)
        {
            #region Preparing Variables

            var result = Result.Ok();
            logger.LogInformation($"Processing sales order {payload.ECommOrderID}");

            #endregion

            #region Process Sales Order

            var salesAggResult = SalesAgg.Create(payload, orderDefaults);
            if (salesAggResult.IsFailed)
            {
                logger.LogWarning($"Failed to create sales order aggregate for {payload.ECommOrderID}");
                return Result.Fail<MedSalesOrder>(salesAggResult.Errors);
            }

            var salesAgg = salesAggResult.Value;
            logger.LogInformation($"Created sales order aggregate for {payload.ECommOrderID}");

            #endregion

            #region Create Customer and Customer Address if does not exists

            var customerInfoResult = await rootstockService.GetCustomerInfo(payload.CustomerAccountID);
            if (customerInfoResult.IsFailed)
            {
                var response = await mediator.Send(new CreateCustomerCommand(salesAgg.SalesOrderCustomer));
                if (response.IsSuccess)
                {
                    salesAgg.SalesOrder.UpdateCustomerId(response.Value.recordId);
                    logger.LogInformation($"Created customer {payload.CustomerAccountID} for sales order {payload.ECommOrderID}");
                }
                else
                {
                    logger.LogWarning($"Failed to create customer {payload.CustomerAccountID} for sales order {payload.ECommOrderID}");
                    return Result.Fail<MedSalesOrder>(customerInfoResult.Errors);
                }
            }
            else
            {
                salesAgg.SalesOrder.UpdateCustomerId(customerInfoResult.Value.CustomerId);
            }

            var customerAddressInfoResult = await rootstockService.GetCustomerAddressInfo(payload.CustomerAccountNumber, payload.ShipToAddress1, payload.ShipToCity, payload.ShipToState, payload.ShipToZip);
            if (customerAddressInfoResult.IsFailed)
            {
                var response = await mediator.Send(new CreateCustomerAddressCommand(salesAgg.SalesOrderCustomerAddress, payload.CustomerAccountID, payload.CustomerAccountNumber));
                if (response.IsSuccess)
                {
                    var customerAddressInfo = response.Value.CustomerAddressInfo;
                    logger.LogInformation($"Created customer address {customerAddressInfo.CustomerAddressID} for sales order {payload.ECommOrderID}");
                }
                else
                {
                    var errorMessage = $"Failed to create customer address for sales order {payload.ECommOrderID} and customer no:{payload.CustomerAccountNumber}";
                    logger.LogWarning(errorMessage);
                    return Result.Fail<MedSalesOrder>(response.Errors);
                }
            }
            else
            {
                salesAgg.SalesOrder.UpdateCustomerAddressReference(customerAddressInfoResult.Value.CustomerAddressID);
            }

            #endregion

            return Result.Ok(salesAgg.SalesOrder);
        }
    }
}