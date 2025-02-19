namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands
{
    public record CreateCustomerAddressCommand(SalesOrderCustomerAddress Address, string CustomerAccountId, string CustomerAccountNumber) : ICommand<CustomerAddressCreated>
    {
    }
}
