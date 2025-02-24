using Tilray.Integrations.Core.Application.Constants;

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
            var result = await mediator.Send(new CreatePurchaseOrderInSAPConcurCommand(purchaseOrders, "Rootstock"));

            if (result.IsFailed)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
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
