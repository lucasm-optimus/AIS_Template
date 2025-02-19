

namespace Tilray.Integrations.Core.Application.SalesOrders.CommandHandlers
{
    public class ProcessSalesOrderCommandHandler(
            IRootstockService rootstockService,
            IMediator mediator,
            ILogger<ProcessSalesOrderCommandHandler> logger,
            OrderDefaultsSettings orderDefaults) : ICommandHandler<ProcessSalesOrdersCommand, SalesOrdersProcessed>
    {
        public async Task<Result<SalesOrdersProcessed>> Handle(ProcessSalesOrdersCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Begin processing {request.SalesOrders.Count()} sales orders.");

            var succesSalesOrders = new List<MedSalesOrder>();
            var failedSalesOrders = new List<string>();

            var tasks = request.SalesOrders.Select(async salesOrder =>
            {
                var response = await ProcessIndividualSalesOrder(salesOrder);
                if (response.IsSuccess)
                {
                    succesSalesOrders.Add(response.Value);
                }
                else
                {
                    failedSalesOrders.Add(salesOrder.ECommOrderID);
                    foreach (var message in response.Reasons.Select(r => r.Message).ToList())
                    {
                        logger.LogWarning($"Failed to process sales order:{salesOrder.ECommOrderID}, message: {message}");
                    }
                }
            });

            await Task.WhenAll(tasks);

            logger.LogInformation($"Successfully processed {succesSalesOrders.Count()} sales orders.");

            return Result.Ok(new SalesOrdersProcessed(succesSalesOrders, failedSalesOrders));
        }

        private async Task<Result<MedSalesOrder>> ProcessIndividualSalesOrder(Domain.Aggregates.SalesOrders.Ecom.SalesOrder payload)
        {
            logger.LogInformation($"Processing sales order {payload.ECommOrderID}");

            var salesAggResult = SalesAgg.Create(payload, orderDefaults);
            if (salesAggResult.IsFailed)
            {
                logger.LogWarning($"Failed to create sales order aggregate for {payload.ECommOrderID}");
                return Result.Fail<MedSalesOrder>(salesAggResult.Errors);
            }

            var salesAgg = salesAggResult.Value;
            logger.LogInformation($"Created sales order aggregate for {payload.ECommOrderID}");

            var customerResult = await EnsureCustomerExists(payload, salesAgg);
            if (customerResult.IsFailed)
            {
                return Result.Fail<MedSalesOrder>(customerResult.Errors);
            }

            var customerAddressResult = await EnsureCustomerAddressExists(payload, salesAgg);
            if (customerAddressResult.IsFailed)
            {
                return Result.Fail<MedSalesOrder>(customerAddressResult.Errors);
            }

            return Result.Ok(salesAgg.SalesOrder);
        }

        private async Task<Result> EnsureCustomerExists(Domain.Aggregates.SalesOrders.Ecom.SalesOrder payload, SalesAgg salesAgg)
        {
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
                    return Result.Fail(response.Errors);
                }
            }
            else
            {
                salesAgg.SalesOrder.UpdateCustomerId(customerInfoResult.Value.CustomerId);
            }

            return Result.Ok();
        }

        private async Task<Result> EnsureCustomerAddressExists(Domain.Aggregates.SalesOrders.Ecom.SalesOrder payload, SalesAgg salesAgg)
        {
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
                    return Result.Fail(response.Errors);
                }
            }
            else
            {
                salesAgg.SalesOrder.UpdateCustomerAddressReference(customerAddressInfoResult.Value.CustomerAddressID);
            }

            return Result.Ok();
        }
    }
}