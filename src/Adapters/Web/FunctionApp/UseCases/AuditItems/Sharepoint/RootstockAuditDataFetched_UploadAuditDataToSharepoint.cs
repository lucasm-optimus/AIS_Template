namespace Tilray.Integrations.Functions.UseCases.AuditItems.Sharepoint;

public class RootstockAuditDataFetched_UploadAuditDataToSharepoint(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for uploading Audit Items to Sharepoint.
    /// </summary>
    [Function(nameof(RootstockAuditDataFetched_UploadAuditDataToSharepoint))]
    public async Task Run(
       [ServiceBusTrigger(Topics.RootstockAuditItemsFetched, Subscriptions.UploadAuditItemsToSharepoint, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var result = await mediator.Send(message.Body.ToString().ToObject<UploadAuditDataToSharepointCommand>());
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
