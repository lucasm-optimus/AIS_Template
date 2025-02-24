namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class UploadPOAPErrorsEventHandler(ILogger<UploadPOAPErrorsEventHandler> logger, ISharepointService sharepointService) : IDomainEventHandler<POAPErrorsGenerated>
    {
        public async Task Handle(POAPErrorsGenerated notification, CancellationToken cancellationToken)
        {
            var result = Result.Ok();
            var csvFileContent = Helpers.ConvertToCsv(notification.APMatchErrors);
            var csvFileBytes = Encoding.UTF8.GetBytes(csvFileContent);
            if (csvFileBytes == null || csvFileBytes.Length == 0)
            {
                LogError("UploadPOAPErrors: No content provided for upload", ref result);
                return;
            }

            var uploadUrlResult = await PrepareUploadUrl(notification.CompanyName);
            if (uploadUrlResult.IsFailed)
            {
                LogError($"UploadPOAPErrors: Failed to prepare upload URL for company {notification.CompanyName}", ref result);
                return;
            }

            logger.LogInformation("UploadPOAPErrors: Uploading file to SharePoint for company {CompanyName} at {Address}", notification.CompanyName, uploadUrlResult.Value);
            result = await sharepointService.UploadFileAsync(csvFileBytes, uploadUrlResult.Value);

            if (result.IsSuccess)
                logger.LogInformation("UploadPOAPErrors: Successfully uploaded file to SharePoint for company {CompanyName}", notification.CompanyName);
            else
                logger.LogError("UploadPOAPErrors: Failed to upload file to SharePoint for company {CompanyName}", notification.CompanyName);
        }

        private void LogError(string message, ref Result result)
        {
            logger.LogError(message);
            result.WithError(message);
        }

        private async Task<Result<string>> PrepareUploadUrl(string companyName)
        {
            var uploadUrlResult = sharepointService.PrepareUploadUrl<APMatchError>(companyName, "csv");
            if (uploadUrlResult.IsFailed)
            {
                logger.LogError("UploadPOAPErrors: Failed to prepare upload URL for company {CompanyName}", companyName);
            }
            return uploadUrlResult;
        }
    }
}
