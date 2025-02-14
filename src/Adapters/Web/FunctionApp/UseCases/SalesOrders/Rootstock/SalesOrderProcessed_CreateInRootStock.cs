using Newtonsoft.Json;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;

namespace Tilray.Integrations.Functions.UseCases.Rootstock;

public class SalesOrderProcessed_CreateInRootStock(IMediator mediator, ILogger<SalesOrderProcessed_CreateInRootStock> logger)
{
    #region Function Implementation

    /// <summary>
    /// Function responsible to create sales order in RootStock
    /// </summary>
    /// <param name="message">Topic message</param>
    /// <param name="log"></param>

    [Function(nameof(SalesOrderProcessed_CreateInRootStock))]
    public async Task Run([ServiceBusTrigger(TOPICS.EcomSalesOrderReceived, SUBSCRIPTIONS.CreateMedSalesOrderInRootStock, Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var salesOrder = JsonConvert.DeserializeObject<MedSalesOrder>(message.Body.ToString());
            var response = await mediator.Send(new CreateSalesOrderCommand(salesOrder, message.CorrelationId));

            if (response.IsSuccess)
            {
                logger.LogInformation($"SalesOrderProcessed_CreateInRootStock: MessageId: {message.MessageId}: Sales order created in RootStock. ");
                await messageActions.CompleteMessageAsync(message);
            }
            else
            {
                logger.LogError($"SalesOrderProcessed_CreateInRootStock: MessageId: {message.MessageId}: Failed to create sales order in RootStock. ");
                foreach (var reason in response.Reasons)
                {
                    logger.LogError($"SalesOrderProcessed_CreateInRootStock: MessageId: {message.MessageId}: {reason.Message}");
                }
                await messageActions.DeadLetterMessageAsync(message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"EcomSalesOrderCreated_CreateSalesOrderInRootStock: MessageId: {message.MessageId}: An error occurred while processing the request");
            await messageActions.DeadLetterMessageAsync(message);
            throw;
        }
    }

    #endregion
}
