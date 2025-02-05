using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record ProcessSalesOrdersCommand (IEnumerable<EcomSalesOrder> SalesOrder) : ICommand<EcomSalesOrderProcessed>
    {
    }
}