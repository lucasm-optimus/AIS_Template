using Tilray.Integrations.Services.Rootstock.Service.Models;

namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class CreateAPATOLinesEventHandler(IRootstockService rootstockService, ILogger<CreateAPATOLinesEventHandler> logger, IMediator mediator) : IDomainEventHandler<InvoicesAggCreated>
    {
        public async Task Handle(InvoicesAggCreated notification, CancellationToken cancellationToken)
        {
            var documentNumbers = notification.APATOLines
                .Select(li => li.DocumentNumber)
                .Distinct()
                .ToList();

            var apatoErrors = new List<APATOError>();

            foreach (var documentNumber in documentNumbers)
            {
                var apatoBatch = notification.APATOLines
                    .Where(li => li.DocumentNumber == documentNumber)
                    .ToList();

                var vendorIdResult = await GetIdFromExternalColumnReference("rstk__povend__c", "rstk__externalid__c", apatoBatch[0].Vendor.Split('-')[0], documentNumber, "vendor");
                var companyIdResult = await GetIdFromExternalColumnReference("rstkf__glcmp__c", "rstkf__externalid__c", apatoBatch[0].Company, documentNumber, "company");

                if (vendorIdResult.IsFailed || companyIdResult.IsFailed) {
                    apatoErrors.AddRange(apatoBatch.Select(item => APATOError.Create(item, string.Empty, string.Empty, string.Empty, "Failed to get vendor or company id")));
                    continue;
                }

                foreach (var item in apatoBatch)
                {
                    var glAccountIdResult = await GetIdFromExternalColumnReference("rstkf__glacct__c", "rstkf__externalid__c", item.GLAccount, documentNumber, "GL Account");

                    if (glAccountIdResult.IsFailed)
                        apatoErrors.Add(APATOError.Create(item, glAccountIdResult.Value, companyIdResult.Value, vendorIdResult.Value, string.Join(" | ", glAccountIdResult.Reasons)));
                    else
                    {
                        var createApatoResult = await rootstockService.CreateApato(
                            apatoLine: item,
                            glAccountId: glAccountIdResult.Value,
                            companyId: companyIdResult.Value,
                            vendorId: vendorIdResult.Value);

                        if (createApatoResult.IsFailed)
                            apatoErrors.Add(APATOError.Create(item, glAccountIdResult.Value, companyIdResult.Value, vendorIdResult.Value, string.Join(" | ", createApatoResult.Reasons)));
                        else
                            logger.LogInformation("Successfully created APATO line for document number {DocumentNumber}", documentNumber);
                    }
                }
            }

            if (apatoErrors.Any())
            {
                var apatoErrorsGenerated = new APATOErrorsGenerated(apatoErrors, notification.CompanyName);
                await mediator.Publish(apatoErrorsGenerated, cancellationToken);
            }
        }

        private async Task<Result<string>> GetIdFromExternalColumnReference(string objectName, string externalIdColumnName, string externalIdValue, string documentNumber, string idType)
        {
            var result = await rootstockService.GetIdFromExternalColumnReference(objectName, externalIdColumnName, externalIdValue);
            if (result.IsFailed)
            {
                logger.LogError("Failed to get {IdType} id for document number {DocumentNumber}", idType, documentNumber);
            }
            return result;
        }
    }
}
