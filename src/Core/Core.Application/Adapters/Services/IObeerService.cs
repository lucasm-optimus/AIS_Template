using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IObeerService
{
    Task<Result> CreateInvoiceAsync(Invoice invoice, InvoicesProcessed invoicesProcessed);
}
