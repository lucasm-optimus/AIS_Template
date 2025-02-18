using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Ecom.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

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

            #region Get Customer Information

            var customerInfoResult = await rootstockService.GetCustomerInfo(request.CustomerAccountId);

            if (customerInfoResult.IsFailed)
            {
                var errorMessage = $"Customer {request.CustomerAccountId} not found.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(customerInfoResult.Errors);
            }

            var customerInfo = customerInfoResult.Value;

            #endregion

            #region Update foreign keys

            var result = Result.Ok();
            var taxResult = await rootstockService.GetIdFromExternalColumnReference("rstk__sotax__c", "rstk__externalid__c", request.Address.TaxLocation);
            if (taxResult.IsFailed)
            {
                var errorMessage = $"Failed to get tax location id for {request.Address.TaxLocation}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            request.Address.UpdateTaxLocation(taxResult.Value);

            #endregion

            #region Create customer address

            logger.LogInformation($"Getting next address sequence for customer {customerInfo.CustomerId}.");
            var nextAddressSequence = await rootstockService.GetCustomerAddressNextSequence(customerInfo.CustomerId) ?? 1;

            logger.LogInformation($"Creating customer address {nextAddressSequence} for customer {customerInfo.CustomerId}.");
            var rootstockCustomerAddressResult = RstkCustomerAddress.Create(request.Address, customerInfo.CustomerId, nextAddressSequence);

            if (rootstockCustomerAddressResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer address {nextAddressSequence} for customer {request.CustomerAccountNumber}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerAddressCreated>(rootstockCustomerAddressResult.Errors);
            }
            var createdCustomerAddressResult = await rootstockService.CreateCustomerAddress(rootstockCustomerAddressResult.Value);

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

            #endregion

            return Result.Ok(new CustomerAddressCreated(customerAddressInfoResult.Value, customerInfo));
        }
    }
}
