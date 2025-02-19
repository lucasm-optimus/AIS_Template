using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Functions.UseCases.Expenses.Rootstock;

public class SAPConcurExpensesFetched_CreateJournalEntriesInRootstock(IMediator mediator)
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
        try
        {
            var result = await mediator.Send(new CreateJournalEntriesInRootstockCommand(message.Body.ToString()));
            if (result.IsFailed)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
            }
        }
        catch (Exception ex)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, deadLetterErrorDescription: ex.InnerException?.Message);
            throw;
        }
    }
}
