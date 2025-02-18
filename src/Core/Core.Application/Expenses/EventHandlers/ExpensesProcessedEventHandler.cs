namespace Tilray.Integrations.Core.Application.Expenses.EventHandlers;

public class ExpensesProcessedEventHandler(IRootstockService rootstockService, ISharepointService sharepointService, ILogger<InvoicesProcessedEventHandler> logger)
    : INotificationHandler<ExpensesProcessed>
{
    public async Task Handle(ExpensesProcessed notification, CancellationToken cancellationToken)
    {
        if (!notification.HasErrors)
            return;

        await rootstockService.PostExpenseMessageToChatterAsync(notification.CompanyReference.Rootstock_Company__c,
            notification.ExpenseErrors.Count);

        await sharepointService.UploadFileAsync(notification.ExpenseErrors, notification.CompanyReference);
    }
}
