namespace Tilray.Integrations.Core.Application.Invoices.QueryHandlers;

public class GetAllInvoicesQueryHandler(ISAPConcurService sapConcurService, IRootstockService rootstockService,
    IBlobService blobService, ILogger<GetAllInvoicesQueryHandler> logger)
    : IQueryManyHandler<GetAllInvoices, string>
{
    public async Task<Result<IEnumerable<string>>> Handle(GetAllInvoices request, CancellationToken cancellationToken)
    {
        var invoicesResult = await sapConcurService.GetInvoicesAsync();
        if (invoicesResult.IsFailed || invoicesResult.Value?.Any() != true)
            return invoicesResult.IsFailed ? Result.Fail<IEnumerable<string>>(invoicesResult.Errors) : Result.Ok();

        var companyReferencesResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (companyReferencesResult.IsFailed)
            return Result.Fail<IEnumerable<string>>(companyReferencesResult.Errors);

        var companyWithInvoices = companyReferencesResult.Value
            .Select(company =>
            {
                var invoices = invoicesResult.Value.Where(invoice => invoice.Company == company.Concur_Company__c).ToList();
                logger.LogInformation("Company {CompanyName} with ConcurCompanyCode {ConcurCompanyCode} has {InvoiceCount} invoices",
                    company.Company_Name__c, company.Concur_Company__c, invoices.Count);
                return InvoiceGroup.Create(company, invoices);
            })
            .Where(invoiceGroup => invoiceGroup.Invoices.Any());

        if (!companyWithInvoices.Any())
        {
            return Result.Fail<IEnumerable<string>>("No invoices found for any company");
        }

        var blobList = await Task.WhenAll(companyWithInvoices.Select(company =>
            blobService.UploadBlobContentAsync(company, "invoice")));

        return Result.Ok(blobList.AsEnumerable());
    }
}
