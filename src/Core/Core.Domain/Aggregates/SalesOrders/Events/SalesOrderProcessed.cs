using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events
{
    public record SalesOrderProcessed(MedSalesOrder SalesOrder, SalesOrderCustomer SalesOrderCustomer, SalesOrderCustomerAddress SalesOrderCustomerAddress) : IDomainEvent
    {
    }
}
