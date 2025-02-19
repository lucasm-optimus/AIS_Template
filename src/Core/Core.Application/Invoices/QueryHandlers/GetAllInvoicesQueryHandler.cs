using Tilray.Integrations.Core.Application.Adapters.Storage;

namespace Tilray.Integrations.Core.Application.Invoices.QueryHandlers;

public class GetAllInvoicesQueryHandler(ISAPConcurService sapConcurService, IRootstockService rootstockService, IBlobService blobService)
    : IQueryManyHandler<GetAllInvoices, string>
{
    public async Task<Result<IEnumerable<string>>> Handle(GetAllInvoices request, CancellationToken cancellationToken)
    {
        var invoicesResult = await sapConcurService.GetInvoicesAsync();
        if (invoicesResult.IsFailed)
            return Result.Fail<IEnumerable<string>>(invoicesResult.Errors);

        if (invoicesResult.Value is null || !invoicesResult.Value.Any())
            return Result.Ok();

        var companyReferencesResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (companyReferencesResult.IsFailed)
            return Result.Fail<IEnumerable<string>>(companyReferencesResult.Errors);

        var companyWithInvoices = companyReferencesResult.Value
            .Select(company => InvoiceGroup.Create(
                company,
                invoicesResult.Value.Where(invoice => invoice.Company == company.Concur_Company__c)))
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
