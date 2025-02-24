using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

namespace Tilray.Integrations.Core.Application.SalesOrders.CommandHandlers
{
    public class CreateCustomerAddressCommandHandler(
                IRootstockService rootstockService,
                ILogger<ProcessSalesOrderCommandHandler> logger) : ICommandHandler<CreateCustomerAddressCommand, CustomerAddressCreated>
    {
        public async Task<Result<CustomerAddressCreated>> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Begin creating customer address for {CustomerAccountId}.", request.CustomerAccountId);

            var customerInfoResult = await GetCustomerInfo(request.CustomerAccountId);
            if (customerInfoResult.IsFailed)
            {
                return Result.Fail<CustomerAddressCreated>(customerInfoResult.Errors);
            }

            var customerInfo = customerInfoResult.Value;

            var result = await UpdateForeignKeys(request.Address);
            if (result.IsFailed)
            {
                return Result.Fail<CustomerAddressCreated>(result.Errors);
            }

            var createdCustomerAddressResult = await CreateCustomerAddress(request, customerInfo.CustomerId);
            if (createdCustomerAddressResult.IsFailed)
            {
                return Result.Fail<CustomerAddressCreated>(createdCustomerAddressResult.Errors);
            }

            var customerAddressInfoResult = await GetCustomerAddressInfo(request);
            if (customerAddressInfoResult.IsFailed)
            {
                return Result.Fail<CustomerAddressCreated>(customerAddressInfoResult.Errors);
            }

            return Result.Ok(new CustomerAddressCreated(customerAddressInfoResult.Value, customerInfo));
        }

        private async Task<Result<RstkCustomerInfoResponse>> GetCustomerInfo(string customerAccountId)
        {
            var customerInfoResult = await rootstockService.GetCustomerInfo(customerAccountId);
            if (customerInfoResult.IsFailed)
            {
                logger.LogError("Customer {CustomerAccountId} not found.", customerAccountId);
            }
            return customerInfoResult;
        }

        private async Task<Result> UpdateForeignKeys(SalesOrderCustomerAddress address)
        {
            var result = Result.Ok();
            result = await UpdateForeignKey(address.TaxLocation, "rstk__sotax__c", "rstk__externalid__c", address.UpdateTaxLocation, result);
            return result;
        }

        private async Task<Result> UpdateForeignKey(string externalIdValue, string objectName, string externalIdColumnName, Action<string> updateAction, Result result)
        {
            var foreignKeyResult = await rootstockService.GetIdFromExternalColumnReference(objectName, externalIdColumnName, externalIdValue);
            if (foreignKeyResult.IsFailed)
            {
                logger.LogError("Failed to get {ObjectName} id for {ExternalIdValue}.", objectName, externalIdValue);
                result.WithError($"Failed to get {objectName} id for {externalIdValue}.");
            }
            else
            {
                updateAction(foreignKeyResult.Value);
            }
            return result;
        }

        private async Task<Result<string>> CreateCustomerAddress(CreateCustomerAddressCommand request, string customerId)
        {
            logger.LogInformation("Getting next address sequence for customer {CustomerId}.", customerId);
            var nextAddressSequence = await rootstockService.GetCustomerAddressNextSequence(customerId) ?? 1;

            logger.LogInformation("Creating customer address {NextAddressSequence} for customer {CustomerId}.", nextAddressSequence, customerId);
            return await rootstockService.CreateCustomerAddress(request.Address, request.CustomerAccountId, nextAddressSequence, request.CustomerAccountNumber);
        }

        private async Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(CreateCustomerAddressCommand request)
        {
            logger.LogInformation("Getting customer address info for customer {CustomerAccountNumber}.", request.CustomerAccountNumber);
            return await rootstockService.GetCustomerAddressInfo(request.CustomerAccountNumber, request.Address.Address1, request.Address.City, request.Address.State, request.Address.Zip);
        }
    }
}
