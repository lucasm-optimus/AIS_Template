using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class CreateInvoicesInObeerCommand(string invoicesBlobName) : ICommand<InvoicesProcessed>
{
    public string InvoicesBlobName { get; set; } = invoicesBlobName;
}
