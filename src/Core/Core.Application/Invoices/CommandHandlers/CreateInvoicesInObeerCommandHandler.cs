using MediatR;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Adapters.Storage;
using Tilray.Integrations.Core.Common.Extensions;
using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class CreateInvoicesInObeerCommandHandler(IObeerService obeerService, IMediator mediator,
    ILogger<CreateInvoicesInObeerCommandHandler> logger, IBlobService blobService) : ICommandHandler<CreateInvoicesInObeerCommand, InvoicesProcessed>
{
    public async Task<Result<InvoicesProcessed>> Handle(CreateInvoicesInObeerCommand request, CancellationToken cancellationToken)
    {
        string invoicesContent = await blobService.DownloadBlobContentAsync(request.InvoiceGroupBlobName);
        var invoiceGroup = invoicesContent.ToObject<InvoiceGroup>();

        if (!invoiceGroup.Company.OBeer_Invoices__c)
        {
            logger.LogInformation("Company {CompanyName} is not configured to process invoices for Obeer",
                invoiceGroup.Company.Company_Name__c);
            return Result.Ok();
        }

        var invoicesProcessed = new InvoicesProcessed();
        foreach (var invoice in invoiceGroup.Invoices)
        {
            await obeerService.CreateInvoiceAsync(invoice, invoicesProcessed);
        }

        if (invoicesProcessed.HasErrors)
        {
            logger.LogWarning(
                "Invoices processing completed with errors. GRPOErrors: {GrpoErrorCount}, NonPOErrors: {NonPoErrorCount}",
                invoicesProcessed.ErrorsGrpo.Count,
                invoicesProcessed.ErrorsNoPo.Count
            );

            invoicesProcessed.CompanyReference = invoiceGroup.Company;
            await mediator.Publish(invoicesProcessed, cancellationToken);
            return Result.Fail(invoicesProcessed.Message);
        }

        logger.LogInformation("Successfully processed {InvoiceCount} invoices", invoiceGroup.Invoices.Count());
        return Result.Ok(invoicesProcessed);
    }
}
