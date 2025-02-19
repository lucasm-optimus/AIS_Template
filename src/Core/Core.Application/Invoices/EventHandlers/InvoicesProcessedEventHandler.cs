namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers;

public class InvoicesProcessedEventHandler(ISharepointService sharepointService, ILogger<InvoicesProcessedEventHandler> logger)
    : INotificationHandler<InvoicesProcessed>
{
    public async Task Handle(InvoicesProcessed notification, CancellationToken cancellationToken)
    {
        if (!notification.HasErrors)
            return;

        if (notification.ErrorsGrpo.Count > 0)
        {
            var result = await sharepointService.UploadFileAsync(
                notification.ErrorsGrpo,
                notification.CompanyReference);
            if (result.IsFailed)
                LogErrors(result.Errors, "GRPO");
        }

        if (notification.ErrorsNonPO.Count > 0)
        {
            var result = await sharepointService.UploadFileAsync(
                notification.ErrorsNonPO,
                notification.CompanyReference);
            if (result.IsFailed)
                LogErrors(result.Errors, "NonPO");
        }
    }

    private void LogErrors(IEnumerable<IError> errors, string errorType)
    {
        foreach (var error in errors)
        {
            logger.LogError("Failed to upload {ErrorType} errors: {Message}",
                errorType, error.Message);
        }
    }
}
