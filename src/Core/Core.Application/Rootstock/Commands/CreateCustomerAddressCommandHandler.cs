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
    public class CreateCustomerAddressCommandHandler(
        IRootstockService rootstockService,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<CreateCustomerAddressCommand, CustomerAddressCreated>
    {
        public async Task<Result<CustomerAddressCreated>> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customer = await rootstockService.GetCustomerInfo(request.payload.CustomerAccountID);

            var nextAddressSequence = await rootstockService.GetCustomerAddressNextSequence(request.payload.CustomerAccountNumber) ?? 1;
            var salesOrderCustomerAddress = SalesOrderCustomerAddress.Create(request.payload, nextAddressSequence, $"{request.payload.CustomerAccountNumber}_{nextAddressSequence}");

            var rootstockCustomerAddress = salesOrderCustomerAddress.GetRootstockCustomerAddress(request.payload);
            await rootstockService.CreateCustomerAddress(rootstockCustomerAddress);

            var customerAddressInfo = await rootstockService.GetCustomerAddressInfo(request.payload.CustomerAccountNumber, request.payload.ShipToAddress1, request.payload.ShipToCity, request.payload.ShipToState, request.payload.ShipToZip);

            return Result.Ok(new CustomerAddressCreated(customerAddressInfo));
        }
    }
}
