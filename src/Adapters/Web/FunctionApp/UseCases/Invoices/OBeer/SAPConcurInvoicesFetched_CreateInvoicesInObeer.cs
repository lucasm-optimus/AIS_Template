namespace Tilray.Integrations.Functions.UseCases.Invoices.OBeer;

public class SAPConcurInvoicesFetched_CreateInvoicesInObeer(IMediator mediator, ILogger<SAPConcurInvoicesFetched_CreateInvoicesInObeer> logger)
{
    /// <summary>
    /// This function is responsible for creating invoices in Obeer.
    /// </summary>
    [Function(nameof(SAPConcurInvoicesFetched_CreateInvoicesInObeer))]
    public async Task Run(
        [ServiceBusTrigger("%TopicSAPConcurInvoicesFetched%", "%SubscriptionCreateInvoicesInObeer%", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var result = await mediator.Send(message.Body.ToString().ToObject<CreateInvoicesInObeerCommand>());
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: string.Join(", ", result.Errors));
        }
    }
}
