namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Events;


public record OBeerPurchaseOrdersProcessed(IEnumerable<PurchaseOrder> PurchaseOrders) : IDomainEvent { }
