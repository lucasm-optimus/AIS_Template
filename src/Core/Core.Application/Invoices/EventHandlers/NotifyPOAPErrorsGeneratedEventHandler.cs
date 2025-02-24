namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class NotifyPOAPErrorsGeneratedEventHandler(ILogger<NotifyPOAPErrorsGeneratedEventHandler> logger, IRootstockService rootstockService) : IDomainEventHandler<POAPErrorsGenerated>
    {
        public async Task Handle(POAPErrorsGenerated notification, CancellationToken cancellationToken)
        {
            var message = $"The latest PO-AP Match Upload for Invoices produced {notification.APMatchErrors.Count()} error(s).";
            var chatterMessagePostedResult = await rootstockService.PostInvoicesErrorGeneratedToChatterAsync(message, notification.CompanyName);
            if (chatterMessagePostedResult.IsFailed)
                logger.LogError("Failed to post PO-AP Match errors to chatter for company {companyName}", notification.CompanyName);
            else
                logger.LogInformation("PO-AP Match errors posted to chatter for company {companyName}", notification.CompanyName);
        }
    }
}
