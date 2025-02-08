using Newtonsoft.Json;
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
    public async Task Run([ServiceBusTrigger("%TopicEcomSalesOrderRecieved%", "%SubscriptionCreateMedSalesOrderInRootStock%", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var salesOrder = JsonConvert.DeserializeObject<MedSalesOrder>(message.Body.ToString());
            var response = await mediator.Send(new CreateSalesOrderCommand(salesOrder, message.CorrelationId));
            await messageActions.CompleteMessageAsync(message);
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
