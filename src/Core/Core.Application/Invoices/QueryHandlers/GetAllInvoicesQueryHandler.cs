using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Core.Application.Invoices.QueryHandlers;

public class GetAllInvoicesQueryHandler(ISAPConcurService sapConcurService, IRootstockService rootstockService, IStream stream,
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

        var tasks = companyWithInvoices.Select(async group =>
        {
            var blob = await blobService.UploadBlobContentAsync(group, "invoice");

            var properties = new Dictionary<string, object>
            {
                { "ObeerFlag", group.Company.OBeer_Invoices__c }
            };

            await stream.SendEventAsync(blob, Topics.SAPConcurInvoicesFetched, properties);
            return blob;
        });

        var blobUrls = await Task.WhenAll(tasks);
        return Result.Ok(blobUrls.AsEnumerable());
    }
}
