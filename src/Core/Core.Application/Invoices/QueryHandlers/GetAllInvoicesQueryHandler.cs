namespace Tilray.Integrations.Core.Application.Invoices.QueryHandlers;

public class GetAllInvoicesQueryHandler(ISAPConcurService sapConcurService, IRootstockService rootstockService) : IQueryManyHandler<GetAllInvoices, InvoiceGroup>
{
    public async Task<Result<IEnumerable<InvoiceGroup>>> Handle(GetAllInvoices request, CancellationToken cancellationToken)
    {
        var invoicesResult = await sapConcurService.GetInvoicesAsync();
        if (invoicesResult.IsFailed)
            return Result.Fail<IEnumerable<InvoiceGroup>>(string.Join(", ", invoicesResult.Errors.Select(e => e.Message)));

        var companyReferencesResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (companyReferencesResult.IsFailed)
            return Result.Fail<IEnumerable<InvoiceGroup>>(string.Join(", ", companyReferencesResult.Errors.Select(e => e.Message)));

        var companyWithInvoices = companyReferencesResult.Value
            .Select(company => InvoiceGroup.Create(
                company,
                invoicesResult.Value.Where(invoice => invoice.Company == company.Concur_Company__c)))
            .Where(invoiceGroup => invoiceGroup.Invoices.Any());

        if (!companyWithInvoices.Any())
        {
            return Result.Fail<IEnumerable<InvoiceGroup>>("No invoices found for any company");
        }

        return Result.Ok(companyWithInvoices);
    }
}
