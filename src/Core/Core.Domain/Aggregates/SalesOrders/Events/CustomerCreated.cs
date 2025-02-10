using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Events
{
    public record CustomerCreated(string recordId) : IDomainEvent
    {
    }
}
