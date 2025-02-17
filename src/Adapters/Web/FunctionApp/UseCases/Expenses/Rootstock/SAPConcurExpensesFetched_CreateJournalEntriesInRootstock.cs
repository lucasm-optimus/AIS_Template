namespace Tilray.Integrations.Functions.UseCases.Expenses.Rootstock;

public class SAPConcurExpensesFetched_CreateJournalEntriesInRootstock(ILogger<SAPConcurExpensesFetched_CreateJournalEntriesInRootstock> logger,
    IMediator mediator)
{
    /// <summary>
    /// This function is responsible for creating journal entries in Rootstock.
    /// </summary>
    [Function(nameof(SAPConcurExpensesFetched_CreateJournalEntriesInRootstock))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SAPConcurExpensesFetched, Subscriptions.CreateJournalEntriesInRootstock, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var result = await mediator.Send(message.Body.ToString().ToObject<CreateJournalEntriesInRootstockCommand>());
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
        }
    }
}
