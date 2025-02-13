using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Ecom.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
{
    public class CreateCustomerCommandHandler(
        IRootstockService rootstockService,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<CreateCustomerCommand, CustomerCreated>
    {
        public async Task<Result<CustomerCreated>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Begin creating customer {request.customer.CustomerNo}.");
            var rootstockCustomer = request.customer.GetRootstockCustomer();
            var createdCustomerResult = await rootstockService.CreateCustomer(rootstockCustomer);

            if (createdCustomerResult.IsFailed)
            {
                var errorMessage = $"Failed to create customer {request.customer.CustomerNo}.";
                logger.LogError(errorMessage);
                return Result.Fail<CustomerCreated>(createdCustomerResult.Errors);
            }

            logger.LogInformation($"Customer {request.customer.CustomerNo} created.");
            return Result.Ok(new CustomerCreated(createdCustomerResult.Value));
        }
    }
}
