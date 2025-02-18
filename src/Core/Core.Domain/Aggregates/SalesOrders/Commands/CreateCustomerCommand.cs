using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record CreateCustomerCommand(SalesOrderCustomer customer) : ICommand<CustomerCreated>
    {
    }
}
