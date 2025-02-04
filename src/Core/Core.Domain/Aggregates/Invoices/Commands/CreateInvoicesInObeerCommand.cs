namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class CreateInvoicesInObeerCommand : ICommand
{
    public CompanyReference CompanyReference { get; private set; } = new();
    public IEnumerable<Invoice> Invoices { get; private set; } = [];

    public CreateInvoicesInObeerCommand(CompanyReference companyReference, IEnumerable<Invoice> invoices)
    {
        CompanyReference = companyReference;
        Invoices = invoices;
    }
}
