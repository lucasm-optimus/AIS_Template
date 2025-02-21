using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Functions.UseCases.PurchaseOrders.SAPConcur;

public class RootstockPurchaseOrderFetched_CreatePurchaseOrderInSAPConcur(IMediator mediator, ILogger<RootstockPurchaseOrderFetched_CreatePurchaseOrderInSAPConcur> logger)
{
    #region Function Implementation

    [Function(nameof(RootstockPurchaseOrderFetched_CreatePurchaseOrderInSAPConcur))]
    public async Task Run([ServiceBusTrigger(Topics.RootstockPurchaseOrdersFetched, Subscriptions.CreatePurchaseOrdersInSAPConcur, Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var purchaseOrders = message.Body.ToString().ToObject<PurchaseOrder>();
            var command = new CreatePurchaseOrderInSAPConcurCommand(purchaseOrders, "Rootstock");
            var commandResult = await mediator.Send(command);

            if (commandResult.IsFailed)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: string.Join(", ", commandResult.Errors));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the purchase orders.");
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, deadLetterErrorDescription: ex.InnerException?.Message);
            throw;
        }
    }

    #endregion
}
