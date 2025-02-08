using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands
{
    public record ProcessSalesOrdersCommand(List<Models.Ecom.SalesOrder> SalesOrders, string correlationId) : ICommand<SalesOrdersProcessed>
    {
    }
}