using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Functions.UseCases.Invoices.Sharepoint;

public class SAPConcurInvoicesFetched_UploadInvoicesToSharepoint(IMediator mediator, ILogger<SAPConcurInvoicesFetched_UploadInvoicesToSharepoint> logger)
{
    /// <summary>
    /// This function is responsible for uploading invoices file to Sharepoint.
    /// </summary>
    [Function(nameof(SAPConcurInvoicesFetched_UploadInvoicesToSharepoint))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SAPConcurInvoicesFetched, Subscriptions.UploadInvoicesToSharepoint, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var result = await mediator.Send(new UploadInvoicesToSharepointCommand(message.Body.ToString()));
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
