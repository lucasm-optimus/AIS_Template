

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands
{
    public record CreateCustomerCommand(SalesOrderCustomer customer) : ICommand<CustomerCreated>
    {
    }
}
