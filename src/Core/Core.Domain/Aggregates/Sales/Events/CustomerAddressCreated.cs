using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Events
{
    public record CustomerAddressCreated(RstkCustomerAddressInfoResponse CustomerAddressInfo, RstkCustomerInfoResponse CustomerInfo) : IDomainEvent
    {
    }
}
