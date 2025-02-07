using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Events
{
    public record SalesOrderProcessed(SalesOrder SalesOrder, SalesOrderCustomer SalesOrderCustomer, SalesOrderCustomerAddress SalesOrderCustomerAddress) : IDomainEvent
    {
    }
}
