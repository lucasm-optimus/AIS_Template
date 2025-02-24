namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers
{
    public class ImportInvoicesInRootstockCommandHandler(
        ILogger<ImportInvoicesInRootstockCommandHandler> logger,
        IMediator mediator,
        GLAccountsSettings glAccounts,
        IBlobService blobService) : ICommandHandler<ImportInvoicesInRootstockCommand, InvoicesAggCreated>
    {
        public async Task<Result<InvoicesAggCreated>> Handle(ImportInvoicesInRootstockCommand request, CancellationToken cancellationToken)
        {
            string invoicesContent = await blobService.DownloadBlobContentAsync(request.InvoiceGroupBlobName);
            var invoiceGroup = invoicesContent.ToObject<InvoiceGroup>();
            var companyName = invoiceGroup.Company.Company_Name__c.Trim();

            var invoiceAggCreatedResult = InvoiceAgg.Create(invoiceGroup, glAccounts);
            if (invoiceAggCreatedResult.IsSuccess)
            {
                logger.LogInformation("Successfully processed invoice matching group {InvoiceGroupBlobName}", request.InvoiceGroupBlobName);

                var invoicesAggCreated = new InvoicesAggCreated(
                    CompanyName: companyName,
                    APLines: invoiceAggCreatedResult.Value.APLines,
                    APATOLines: invoiceAggCreatedResult.Value.APATOLines,
                    POAPLines: invoiceAggCreatedResult.Value.POAPMatchMQs,
                    APATOMQLines: invoiceAggCreatedResult.Value.APATOMQs);

                await mediator.Publish(invoicesAggCreated, cancellationToken);

                return Result.Ok(invoicesAggCreated);
            }
            else
            {
                logger.LogError("Failed to create CompanyInvoicesAPMatch for invoice group {InvoiceGroupBlobName}", request.InvoiceGroupBlobName);
                return Result.Fail<InvoicesAggCreated>(invoiceAggCreatedResult.Errors);
            }

        }
    }
}