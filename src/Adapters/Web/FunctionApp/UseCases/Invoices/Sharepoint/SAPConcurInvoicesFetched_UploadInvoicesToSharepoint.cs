namespace Tilray.Integrations.Functions.UseCases.Invoices.Sharepoint;

public class SAPConcurInvoicesFetched_UploadInvoicesToSharepoint(IMediator mediator, ILogger<SAPConcurInvoicesFetched_UploadInvoicesToSharepoint> logger)
{
    /// <summary>
    /// This function is responsible for uploading invoices file to Sharepoint.
    /// </summary>
    [Function(nameof(SAPConcurInvoicesFetched_UploadInvoicesToSharepoint))]
    public async Task Run(
        [ServiceBusTrigger("%TopicSAPConcurInvoicesFetched%", "%SubscriptionUploadInvoicesToSharepoint%", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var result = await mediator.Send(new UploadInvoicesToSharepointCommand(message.Body.ToString()));
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: string.Join(", ", result.Errors));
        }
    }
}
