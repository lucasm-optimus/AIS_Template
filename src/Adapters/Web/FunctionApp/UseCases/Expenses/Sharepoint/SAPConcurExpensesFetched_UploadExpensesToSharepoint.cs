namespace Tilray.Integrations.Functions.UseCases.Expenses.Sharepoint;

public class SAPConcurExpensesFetched_UploadExpensesToSharepoint(ILogger<SAPConcurExpensesFetched_UploadExpensesToSharepoint> logger,
    IMediator mediator)
{
    /// <summary>
    /// This function is responsible for uploading expenses to Sharepoint.
    /// </summary>
    [Function(nameof(SAPConcurExpensesFetched_UploadExpensesToSharepoint))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SAPConcurExpensesFetched, Subscriptions.UploadExpensesToSharepoint, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var result = await mediator.Send(new UploadExpensesToSharepointCommand(message.Body.ToString()));
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
        }
    }
}
