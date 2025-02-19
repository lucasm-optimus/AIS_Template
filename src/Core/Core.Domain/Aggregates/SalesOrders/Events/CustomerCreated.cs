namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events
{
    public record CustomerCreated(string recordId) : IDomainEvent
    {
    }
}
