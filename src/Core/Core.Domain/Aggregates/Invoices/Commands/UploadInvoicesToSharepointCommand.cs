namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class UploadInvoicesToSharepointCommand : ICommand
{
    public CompanyReference CompanyReference { get; private set; }
    public List<Invoice> Invoices { get; private set; }

    public UploadInvoicesToSharepointCommand(CompanyReference companyReference, List<Invoice> invoices)
    {
        CompanyReference = companyReference;
        Invoices = invoices;
    }
}
