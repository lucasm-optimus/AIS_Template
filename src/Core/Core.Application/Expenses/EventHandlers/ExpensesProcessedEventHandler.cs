namespace Tilray.Integrations.Core.Application.Expenses.EventHandlers;

public class ExpensesProcessedEventHandler(IRootstockService rootstockService, ISharepointService sharepointService, ILogger<InvoicesProcessedEventHandler> logger)
    : INotificationHandler<ExpensesProcessed>
{
    public async Task Handle(ExpensesProcessed notification, CancellationToken cancellationToken)
    {
        if (!notification.HasErrors)
            return;

        if (notification.ExpenseErrors.Count > 0)
        {
            var result = await rootstockService.PostExpensesChatterMessageAsync(notification.CompanyReference.Rootstock_Company__c,
                notification.ExpenseErrors.Count);
            if(result.IsFailed)
                logger.LogError("Failed to post expenses chatter message: {Message}", Helpers.GetErrorMessage(result.Errors));

            result = await sharepointService.UploadFileAsync(notification.ExpenseErrors, notification.CompanyReference);
            if (result.IsFailed)
                logger.LogError("Failed to upload expenses. errors: {Message}", Helpers.GetErrorMessage(result.Errors));
        }
    }
}
