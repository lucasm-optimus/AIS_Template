using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Customer;

namespace Tilray.Integrations.Core.Application.SalesOrders.CommandHandlers
{
    public class CreateCustomerCommandHandler(
            IRootstockService rootstockService,
            ILogger<ProcessSalesOrderCommandHandler> logger,
            OrderDefaultsSettings orderDefaults) : ICommandHandler<CreateCustomerCommand, CustomerCreated>
    {
        public async Task<Result<CustomerCreated>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Begin creating customer {request.customer.CustomerNo}.");

            var result = await UpdateForeignKeys(request.customer);
            if (result.IsFailed)
            {
                return Result.Fail<CustomerCreated>(result.Errors);
            }

            var createdCustomerResult = await rootstockService.CreateCustomer(request.customer);
            if (createdCustomerResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer {request.customer.CustomerNo}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerCreated>(createdCustomerResult.Errors);
            }

            logger.LogInformation($"Customer {request.customer.CustomerNo} created.");
            return Result.Ok(new CustomerCreated(createdCustomerResult.Value));
        }

        private async Task<Result> UpdateForeignKeys(SalesOrderCustomer customer)
        {
            var result = Result.Ok();

            result = await UpdateForeignKey(customer.CustomerClass, "rstk__socclass__c", "rstk__externalid__c", customer.UpdateCustomerClass, result);
            result = await UpdateForeignKey(customer.AccountingDimension1, "rstk__sydim__c", "rstk__externalid__c", customer.UpdateAccountingDimension1, result);
            result = await UpdateForeignKey(customer.AccountingDimension2, "rstk__sydim__c", "rstk__externalid__c", customer.UpdateAccountingDimension2, result);
            result = await UpdateForeignKey(customer.PaymentTerms, "rstk__syterms__c", "rstk__externalid__c", customer.UpdatePaymentTerms, result);

            return result;
        }

        private async Task<Result> UpdateForeignKey(string externalIdValue, string objectName, string externalIdColumnName, Action<string> updateAction, Result result)
        {
            var foreignKeyResult = await rootstockService.GetIdFromExternalColumnReference(objectName, externalIdColumnName, externalIdValue);
            if (foreignKeyResult.IsFailed)
            {
                var errorMessage = $"Failed to get {objectName} id for {externalIdValue}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            else
            {
                updateAction(foreignKeyResult.Value);
            }

            return result;
        }
    }
}
