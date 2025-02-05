using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Application.Ecom.Commands;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain.Aggregates.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Customer.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Customer.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
{
    public class CreateCustomerCommandHandler(
        IRootstockService rootstockService,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<CreateCustomerCommand, CustomerCreated>
    {
        public async Task<Result<CustomerCreated>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var salesOrderCustomer = SalesOrderCustomer.Create(request.salesOrder, orderDefaults);
            var rootstockCustomer = salesOrderCustomer.GetRootstockCustomer();
            await rootstockService.CreateCustomer(rootstockCustomer);
            return Result.Ok(new CustomerCreated());
        }
    }
}
