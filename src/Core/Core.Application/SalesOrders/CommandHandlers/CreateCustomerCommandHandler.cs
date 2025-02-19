using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

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

            #region Update foreign keys

            var result = Result.Ok();
            var customerClassResult = await rootstockService.GetIdFromExternalColumnReference("rstk__socclass__c", "rstk__externalid__c", request.customer.CustomerClass);
            if (customerClassResult.IsFailed)
            {
                var errorMessage = $"Failed to get customer class id for {request.customer.CustomerClass}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            request.customer.UpdateCustomerClass(customerClassResult.Value);

            var accountingDimension1Result = await rootstockService.GetIdFromExternalColumnReference("rstk__sydim__c", "rstk__externalid__c", request.customer.AccountingDimension1);
            if (accountingDimension1Result.IsFailed)
            {
                var errorMessage = $"Failed to get accounting dimension 1 id for {request.customer.AccountingDimension1}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            request.customer.UpdateAccountingDimension1(accountingDimension1Result.Value);

            var accountingDimension2Result = await rootstockService.GetIdFromExternalColumnReference("rstk__sydim__c", "rstk__externalid__c", request.customer.AccountingDimension2);
            if (accountingDimension2Result.IsFailed)
            {
                var errorMessage = $"Failed to get accounting dimension 2 id for {request.customer.AccountingDimension2}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            request.customer.UpdateAccountingDimension2(accountingDimension2Result.Value);

            var paymentTermsResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syterms__c", "rstk__externalid__c", request.customer.PaymentTerms);
            if (paymentTermsResult.IsFailed)
            {
                var errorMessage = $"Failed to get payment terms id for {request.customer.PaymentTerms}.";
                logger.LogError(errorMessage);
                result.WithError(errorMessage);
            }
            request.customer.UpdatePaymentTerms(paymentTermsResult.Value);

            if (result.IsFailed)
            {
                return Result.Fail<CustomerCreated>(result.Errors);
            }

            #endregion

            #region Create Customer

            var rootstockCustomerResult = RstkCustomer.Create(request.customer);

            if (rootstockCustomerResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer {request.customer.CustomerNo}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerCreated>(rootstockCustomerResult.Errors);
            }

            var createdCustomerResult = await rootstockService.CreateCustomer(rootstockCustomerResult.Value);

            if (createdCustomerResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer {request.customer.CustomerNo}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerCreated>(createdCustomerResult.Errors);
            }

            logger.LogInformation($"Customer {request.customer.CustomerNo} created.");
            return Result.Ok(new CustomerCreated(createdCustomerResult.Value));

            #endregion
        }
    }
}
