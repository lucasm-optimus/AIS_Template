using Tilray.Integrations.Services.Rootstock.Service.Models;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events
{
    public record APATOErrorsGenerated(IEnumerable<APATOError> APATOErrors, string CompanyName):IDomainEvent
    {
    }
}
