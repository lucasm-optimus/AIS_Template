using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record CreateSalesOrderCommand(SalesOrder SalesOrder, string corelationId) : ICommand<SalesOrderCreated>
    {
    }
}
