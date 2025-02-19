using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Functions.UseCases.Invoices.OBeer;

public class SAPConcurInvoicesFetched_CreateInvoicesInObeer(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for creating invoices in Obeer.
    /// </summary>
    [Function(nameof(SAPConcurInvoicesFetched_CreateInvoicesInObeer))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SAPConcurInvoicesFetched, Subscriptions.CreateInvoicesInObeer, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var result = await mediator.Send(new CreateInvoicesInObeerCommand(message.Body.ToString()));
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
