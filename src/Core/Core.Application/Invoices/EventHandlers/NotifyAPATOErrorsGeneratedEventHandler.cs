namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class NotifyAPATOErrorsGeneratedEventHandler(ILogger<NotifyAPATOErrorsGeneratedEventHandler> logger, IRootstockService rootstockService) : IDomainEventHandler<APATOErrorsGenerated>
    {
        public async Task Handle(APATOErrorsGenerated notification, CancellationToken cancellationToken)
        {
            var message = $"The latest Payable Transaction Upload for Invoices produced {notification.APATOErrors.Count()} batch errors.";
            var chatterMessagePostedResult = await rootstockService.PostInvoicesErrorGeneratedToChatterAsync(message, notification.CompanyName);
            if (chatterMessagePostedResult.IsFailed)
                logger.LogError("Failed to post APATO errors to chatter for company {companyName}", notification.CompanyName);
            else
                logger.LogInformation("APATO errors posted to chatter for company {companyName}", notification.CompanyName);
        }
    }
}
