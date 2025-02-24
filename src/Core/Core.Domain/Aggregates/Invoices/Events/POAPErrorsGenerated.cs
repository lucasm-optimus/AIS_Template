namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events
{
    public record POAPErrorsGenerated(IEnumerable<APMatchError> APMatchErrors, string CompanyName) : IDomainEvent
    {
    }
}
