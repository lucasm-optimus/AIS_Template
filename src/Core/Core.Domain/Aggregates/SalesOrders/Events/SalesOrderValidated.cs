namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

public record SalesOrderValidated(bool result, IEnumerable<string> messages) : IDomainEvent
{
}
