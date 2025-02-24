namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events
{
    public record InvoicesAggCreated(
        string CompanyName,
        IEnumerable<APLineItem> APLines,
        IEnumerable<APATOLineItem> APATOLines,
        IEnumerable<POAPLineItem> POAPLines,
        IEnumerable<APATOMQLineItem> APATOMQLines) : IDomainEvent
    {
    }
}
