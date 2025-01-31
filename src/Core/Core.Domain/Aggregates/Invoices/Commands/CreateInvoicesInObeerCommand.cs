namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class CreateInvoicesInObeerCommand : ICommand
{
    public CompanyReference CompanyReference { get; private set; } = new();
    public List<Invoice> Invoices { get; private set; } = [];

    public CreateInvoicesInObeerCommand(CompanyReference companyReference, List<Invoice> invoices)
    {
        CompanyReference = companyReference;
        Invoices = invoices;
    }
}
