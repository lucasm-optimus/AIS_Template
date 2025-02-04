namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class UploadInvoicesToSharepointCommand : ICommand
{
    public CompanyReference CompanyReference { get; private set; } = new();
    public IEnumerable<Invoice> Invoices { get; private set; } = [];

    public UploadInvoicesToSharepointCommand(CompanyReference companyReference, IEnumerable<Invoice> invoices)
    {
        CompanyReference = companyReference;
        Invoices = invoices;
    }
}
