using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Adapters.Storage;
using Tilray.Integrations.Core.Common.Extensions;

namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadInvoicesToSharepointCommandHandler(ISharepointService sharepointService, IBlobService blobService,
    ILogger<UploadInvoicesToSharepointCommandHandler> logger) : ICommandHandler<UploadInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        string invoicesContent = await blobService.DownloadBlobContentAsync(request.InvoicesBlobName);
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

        return await sharepointService.UploadFileAsync(invoiceGroup.Invoices, invoiceGroup.Company);
    }
}
