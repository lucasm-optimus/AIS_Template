using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Ecom;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record ProcessSalesOrdersCommand(List<SalesOrder> SalesOrders) : ICommand<SalesOrdersProcessed>
    {
    }
}