using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record CreateCustomerAddressCommand(SalesOrderCustomerAddress Address, string CustomerAccountId, string CustomerAccountNumber, string CorrelationId) : ICommand<CustomerAddressCreated>
    {
    }
}
