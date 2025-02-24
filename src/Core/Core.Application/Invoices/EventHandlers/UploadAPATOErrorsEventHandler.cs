using Tilray.Integrations.Services.Rootstock.Service.Models;

namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class UploadAPATOErrorsEventHandler(ISharepointService sharepointService, ILogger<UploadAPATOErrorsEventHandler> logger) : IDomainEventHandler<APATOErrorsGenerated>
    {
        public async Task Handle(APATOErrorsGenerated notification, CancellationToken cancellationToken)
        {
            var result = Result.Ok();
            var csvFileContent = Helpers.ConvertToCsv(notification.APATOErrors);
            var csvFileBytes = Encoding.UTF8.GetBytes(csvFileContent);
            if (csvFileBytes == null || csvFileBytes.Length == 0)
            {
                LogError("UploadAPATOError: No content provided for upload", ref result);
                return;
            }

            var uploadUrlResult = await PrepareUploadUrl(notification.CompanyName);
            if (uploadUrlResult.IsFailed)
            {
                LogError($"UploadAPATOError: Failed to prepare upload URL for company {notification.CompanyName}", ref result);
                return;
            }

            logger.LogInformation("UploadAPATOError: Uploading file to SharePoint for company {CompanyName} at {Address}", notification.CompanyName, uploadUrlResult.Value);
            result = await sharepointService.UploadFileAsync(csvFileBytes, uploadUrlResult.Value);

            if (result.IsSuccess)
                logger.LogInformation("UploadAPATOError: Successfully uploaded file to SharePoint for company {CompanyName}", notification.CompanyName);
            else
                logger.LogError("UploadAPATOError: Failed to upload file to SharePoint for company {CompanyName}", notification.CompanyName);
        }

        private void LogError(string message, ref Result result)
        {
            logger.LogError(message);
            result.WithError(message);
        }

        private async Task<Result<string>> PrepareUploadUrl(string companyName)
        {
            var uploadUrlResult = sharepointService.PrepareUploadUrl<APATOError>(companyName, "csv");
            if (uploadUrlResult.IsFailed)
            {
                logger.LogError("UploadAPATOError: Failed to prepare upload URL for company {CompanyName}", companyName);
            }
            return uploadUrlResult;
        }
    }
}
