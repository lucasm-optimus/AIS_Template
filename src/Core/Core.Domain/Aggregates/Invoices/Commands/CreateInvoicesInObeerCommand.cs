using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class CreateInvoicesInObeerCommand(string invoiceGroupBlobName) : ICommand<InvoicesProcessed>
{
    public string InvoiceGroupBlobName { get; set; } = invoiceGroupBlobName;
}
