using Tilray.Integrations.Core.Common.Extensions;

namespace Tilray.Integrations.Functions.Sharepoint;

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
        var result = await mediator.Send(message.Body.ToString().ToObject<UploadInvoicesToSharepointCommand>());
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: string.Join(", ", result.Errors));
        }
    }
}
