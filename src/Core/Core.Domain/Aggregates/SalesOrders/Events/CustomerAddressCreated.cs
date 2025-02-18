using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events
{
    public record CustomerAddressCreated(RstkCustomerAddressInfoResponse CustomerAddressInfo, RstkCustomerInfoResponse CustomerInfo) : IDomainEvent
    {
    }
}
