using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Ecom.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
{
    public class CreateCustomerAddressCommandHandler(
        IRootstockService rootstockService,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<CreateCustomerAddressCommand, CustomerAddressCreated>
    {
        public async Task<Result<CustomerAddressCreated>> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Begin creating customer address for {request.CustomerAccountId}.");

            var customerInfoResult = await rootstockService.GetCustomerInfo(request.CustomerAccountId);

            if (customerInfoResult.IsFailed)
            {
                var errorMessage = $"Customer {request.CustomerAccountId} not found.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(customerInfoResult.Errors);
            }

            var customerInfo = customerInfoResult.Value;

            logger.LogInformation($"Getting next address sequence for customer {customerInfo.CustomerId}.");
            var nextAddressSequence = await rootstockService.GetCustomerAddressNextSequence(customerInfo.CustomerId) ?? 1;

            logger.LogInformation($"Creating customer address {nextAddressSequence} for customer {customerInfo.CustomerId}.");
            var rootstockCustomerAddress = request.Address.GetRootstockCustomerAddress(customerInfo.CustomerId, nextAddressSequence);
            var createdCustomerAddressResult = await rootstockService.CreateCustomerAddress(rootstockCustomerAddress);

            if (createdCustomerAddressResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer address {nextAddressSequence} for customer {request.CustomerAccountNumber}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(createdCustomerAddressResult.Errors);
            }

            logger.LogInformation($"Getting customer address info for customer {request.CustomerAccountNumber}.");
            var customerAddressInfoResult = await rootstockService.GetCustomerAddressInfo(request.CustomerAccountNumber, request.Address.Address1, request.Address.City, request.Address.State, request.Address.Zip);

            if (customerAddressInfoResult.IsFailed)
            {
                var errorMessage = $"Failed to get customer address info for customer {request.CustomerAccountNumber}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(customerAddressInfoResult.Errors);
            }

            return Result.Ok(new CustomerAddressCreated(customerAddressInfoResult.Value, customerInfoResult.Value));
        }
    }
}
