namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class UploadAPATOLinesEventHandler(ISharepointService sharepointService, ILogger<UploadAPATOLinesEventHandler> logger) : IDomainEventHandler<InvoicesAggCreated>
    {
        public async Task Handle(InvoicesAggCreated notification, CancellationToken cancellationToken)
        {
            var result = Result.Ok();
            var csvFileContent = Helpers.ConvertToCsv(notification.APATOLines);
            var csvFileBytes = Encoding.UTF8.GetBytes(csvFileContent);
            if (csvFileBytes == null || csvFileBytes.Length == 0)
            {
                LogError("UploadFileAsync: No content provided for upload", ref result);
                return;
            }

            var companyName = notification.APATOLines.FirstOrDefault()?.Company;
            var uploadUrlResult = await PrepareUploadUrl(companyName);
            if (uploadUrlResult.IsFailed)
            {
                LogError($"Failed to prepare upload URL for company {companyName}", ref result);
                return;
            }

            logger.LogInformation("Uploading file to SharePoint for company {CompanyName} at {Address}", companyName, uploadUrlResult.Value);
            result = await sharepointService.UploadFileAsync(csvFileBytes, uploadUrlResult.Value);

            if (result.IsSuccess)
                logger.LogInformation("Successfully uploaded file to SharePoint for company {CompanyName}", companyName);
            else
                logger.LogError("Failed to upload file to SharePoint for company {CompanyName}", companyName);
        }

        private void LogError(string message, ref Result result)
        {
            logger.LogError(message);
            result.WithError(message);
        }

        private async Task<Result<string>> PrepareUploadUrl(string companyName)
        {
            var uploadUrlResult = sharepointService.PrepareUploadUrl<APATOLineItem>(companyName, "csv");
            if (uploadUrlResult.IsFailed)
            {
                logger.LogError("Failed to prepare upload URL for company {CompanyName}", companyName);
            }
            return uploadUrlResult;
        }
    }
}
