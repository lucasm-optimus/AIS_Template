namespace Tilray.Integrations.Functions.UseCases.Invoices.OBeer;

public class SAPConcurInvoicesFetched_CreateInvoicesInRootstock(IMediator mediator, ILogger<SAPConcurInvoicesFetched_CreateInvoicesInRootstock> logger)
{
    /// <summary>
    /// This function is responsible for creating invoices in Obeer.
    /// </summary>
    [Function(nameof(SAPConcurInvoicesFetched_CreateInvoicesInRootstock))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SAPConcurInvoicesFetched, Subscriptions.CreateInvoicesInRootstock, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var result = await mediator.Send(new ImportInvoicesInRootstockCommand(message.Body.ToString()));
            if (result.IsFailed)
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to process message {MessageId}", message.MessageId);
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: e.Message);
        }
    }
}
