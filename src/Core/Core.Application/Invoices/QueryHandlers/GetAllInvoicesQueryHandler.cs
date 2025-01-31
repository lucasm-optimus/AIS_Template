namespace Tilray.Integrations.Core.Application.Invoices.QueryHandlers;

public class GetAllInvoicesQueryHandler(ISAPConcurService sapConcurService, IRootstockService rootstockService,
    IObeerService obeerService) : IQueryManyHandler<GetAllInvoices, InvoiceGroup>
{
    public async Task<Result<IEnumerable<InvoiceGroup>>> Handle(GetAllInvoices request, CancellationToken cancellationToken)
    {
        var invoicesResult = await sapConcurService.GetInvoicesAsync();
        if (!invoicesResult.IsSuccess)
            return Result.Fail<IEnumerable<InvoiceGroup>>($"Failed to fetch invoices: {string.Join(", ", invoicesResult.Errors.Select(e => e.Message))}");

        var companyReferencesResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (!companyReferencesResult.IsSuccess)
            return Result.Fail<IEnumerable<InvoiceGroup>>($"Failed to fetch company references: {string.Join(", ", companyReferencesResult.Errors.Select(e => e.Message))}");

        var companyWithInvoices = companyReferencesResult.Value
            .Select(company => InvoiceGroup.Create(
                company,
                invoicesResult.Value.Where(invoice => invoice.Company == company.Concur_Company__c).ToList()))
            .Where(invoiceGroup => invoiceGroup.Invoices.Count > 0);


        //foreach (var invoice in companyWithInvoices)
        //{
        //    await obeerService.CreateInvoicesAsync(invoice.Invoices);
        //}

        return Result.Ok(companyWithInvoices);
    }
}
