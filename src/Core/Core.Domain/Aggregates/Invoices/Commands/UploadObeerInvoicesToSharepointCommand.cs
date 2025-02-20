namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class UploadObeerInvoicesToSharepointCommand(string invoiceGroupBlobName) : ICommand
{
    public string InvoiceGroupBlobName { get; set; } = invoiceGroupBlobName;
}
