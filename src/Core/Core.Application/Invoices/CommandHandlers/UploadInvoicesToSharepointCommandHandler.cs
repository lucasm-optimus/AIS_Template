using Tilray.Integrations.Core.Application.Adapters.Storage;

namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadInvoicesToSharepointCommandHandler(ISharepointService sharepointService, IBlobService blobService,
    ILogger<UploadInvoicesToSharepointCommandHandler> logger) : ICommandHandler<UploadInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        string invoicesContent = await blobService.DownloadBlobContentAsync(request.InvoiceGroupBlobName);
        var invoiceGroup = invoicesContent.ToObject<InvoiceGroup>();

        if (invoiceGroup.Invoices?.Any() != true)
        {
            return Result.Ok();
        }

        if (!invoiceGroup.Company.OBeer_Invoices__c)
        {
            logger.LogInformation("Company {CompanyName} is not configured to process invoices for Obeer",
                invoiceGroup.Company.Company_Name__c);
            return Result.Ok();
        }

        return await sharepointService.UploadInvoicesAsync(invoiceGroup.Invoices, invoiceGroup.Company);
    }
}
