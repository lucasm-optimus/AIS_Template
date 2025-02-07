namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class UploadInvoicesToSharepointCommand(string invoicesBlobName) : ICommand
{
    public string InvoicesBlobName { get; set; } = invoicesBlobName;
}
