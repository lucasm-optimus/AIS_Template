namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IObeerService
{
    Task<Result<InvoiceProcessingResult>> CreateInvoicesAsync(IEnumerable<Invoice> invoices);
}
