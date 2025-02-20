namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadObeerInvoicesToSharepointCommandHandler(ISharepointService sharepointService, IBlobService blobService,
    ILogger<UploadObeerInvoicesToSharepointCommandHandler> logger) : ICommandHandler<UploadObeerInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadObeerInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        string invoicesContent = await blobService.DownloadBlobContentAsync(request.InvoiceGroupBlobName);
        var invoiceGroup = invoicesContent.ToObject<InvoiceGroup>();

        if (!invoiceGroup.AreNonEmptyInvoices())
        {
            logger.LogInformation("No invoices to upload to Sharepoint");
            return Result.Ok();
        }

        if (!invoiceGroup.Company.CanProcessInvoicesForObeer)
        {
            logger.LogInformation("Skipping SharePoint upload: Company {CompanyName} is not configured to process invoices for Obeer.",
                invoiceGroup.Company.Company_Name__c);
            return Result.Ok();
        }

        logger.LogInformation("Uploading {InvoiceCount} Obeer invoices for company {CompanyName} to Sharepoint. InvoiceNumbers: {InvoiceNumbers}",
            invoiceGroup.Invoices.Count(), invoiceGroup.Company.Company_Name__c, invoiceGroup.Invoices.Select(inv => inv.InvoiceNumber));
        return await sharepointService.UploadInvoicesAsync(invoiceGroup.Invoices, invoiceGroup.Company);
    }
}
