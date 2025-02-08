using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

public record SalesOrdersProcessed(List<MedSalesOrder> salesOrder) : IDomainEvent
{
}
