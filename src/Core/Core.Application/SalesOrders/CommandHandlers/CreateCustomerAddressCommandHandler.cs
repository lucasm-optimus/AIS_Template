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
            logger.LogInformation($"[{request.CorrelationId}] Begin creating customer address for {request.CustomerAccountId}.");

            var customer = await rootstockService.GetCustomerInfo(request.CustomerAccountId);

            if (customer == null)
            {
                var errorMessage = $"Customer {request.CustomerAccountId} not found.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(errorMessage);
            }

            logger.LogInformation($"[{request.CorrelationId}] Getting next address sequence for customer {request.CustomerAccountNumber}.");
            var nextAddressSequence = await rootstockService.GetCustomerAddressNextSequence(request.CustomerAccountNumber) ?? 1;

            logger.LogInformation($"[{request.CorrelationId}] Creating customer address {nextAddressSequence} for customer {request.CustomerAccountNumber}.");
            var rootstockCustomerAddress = request.Address.GetRootstockCustomerAddress(request.CustomerAccountNumber);
            var createdCustomerAddress = await rootstockService.CreateCustomerAddress(rootstockCustomerAddress);

            if (createdCustomerAddress == null)
            {
                var errorMessage = $"Failed to create customer address {nextAddressSequence} for customer {request.CustomerAccountNumber}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(errorMessage);
            }

            logger.LogInformation($"[{request.CorrelationId}] Getting customer address info for customer {request.CustomerAccountNumber}.");
            var customerAddressInfo = await rootstockService.GetCustomerAddressInfo(request.CustomerAccountNumber, request.Address.Address1, request.Address.City, request.Address.State, request.Address.Zip);

            if (customerAddressInfo == null)
            {
                var errorMessage = $"Failed to get customer address info for customer {request.CustomerAccountNumber}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(errorMessage);
            }

            return Result.Ok(new CustomerAddressCreated(customerAddressInfo, customer));
        }
    }
}
