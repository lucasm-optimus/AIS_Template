using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Customer.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer.Commands
{
    public record CreateCustomerAddressCommand(EcomSalesOrder payload) : ICommand<CustomerAddressCreated>
    {
    }
}
