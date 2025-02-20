namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Events;


public record RootstockPurchaseOrdersProcessed(IEnumerable<PurchaseOrder> PurchaseOrders) : IDomainEvent { }
