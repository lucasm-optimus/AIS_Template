using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock;

public class SalesOrderProcessed_CreateInRootStock(IMediator mediator, ILogger<SalesOrderProcessed_CreateInRootStock> logger)
{
    #region Function Implementation

    /// <summary>
    /// Function to create sales order in RootStock
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageActions"></param>
    /// <returns></returns>

    [Function(nameof(SalesOrderProcessed_CreateInRootStock))]
    public async Task Run([ServiceBusTrigger(Topics.EcomSalesOrderReceived, Subscriptions.CreateSalesOrderInRootStock, Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var salesOrder = JsonConvert.DeserializeObject<MedSalesOrder>(message.Body.ToString());
            var response = await mediator.Send(new CreateSalesOrderCommand(salesOrder));

            if (response.IsFailed)
                await HandleFailedSalesOrderCreation(logger, message, messageActions, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"EcomSalesOrderCreated_CreateSalesOrderInRootStock: MessageId: {message.MessageId}: An error occurred while processing the request");
            await messageActions.DeadLetterMessageAsync(message);
        }
    }

    private static async Task HandleFailedSalesOrderCreation(ILogger<SalesOrderProcessed_CreateInRootStock> logger, ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, Result<SalesOrderCreated> response)
    {
        logger.LogError($"SalesOrderProcessed_CreateInRootStock: MessageId: {message.MessageId}: Failed to create sales order in RootStock. ");
        foreach (var reason in response.Reasons)
        {
            logger.LogError($"SalesOrderProcessed_CreateInRootStock: MessageId: {message.MessageId}: {reason.Message}");
        }
        await messageActions.DeadLetterMessageAsync(message);
    }

    #endregion
}
