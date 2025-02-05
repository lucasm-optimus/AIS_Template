using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer.Events
{
    public record CustomerAddressCreated(RstkCustomerAddressInfoResponse customerAddressInfo) : IDomainEvent
    {
    }
}
