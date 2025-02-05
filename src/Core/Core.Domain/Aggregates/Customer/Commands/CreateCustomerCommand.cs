using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Customer.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer.Commands
{
    public record CreateCustomerCommand(EcomSalesOrder salesOrder) : ICommand<CustomerCreated>
    {
    }
}
