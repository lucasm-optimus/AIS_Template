using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;

namespace Tilray.Integrations.FunctionsApp.UseCases.Rootstock;

public class SalesOrderProcessed_CreateInRootStock(IMediator mediator)
{
    #region Function Implementation

    /// <summary>
    /// Function responsible to create sales order in RootStock
    /// </summary>
    /// <param name="message">Topic message</param>
    /// <param name="log"></param>
    [FunctionName("SalesOrderProcessed_CreateInRootStock")]
    public async Task Run([ServiceBusTrigger("%TopicFSISalesOrderCreated%", "%SubscriptionCreateSalesOrderInRootStock%", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions, ILogger log)
    {
        try
        {
            var salesOrder = JsonConvert.DeserializeObject<SalesOrder>(message.Body.ToString());
            await mediator.Send(new CreateRootstockSalesOrderCommand(salesOrder));
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"FSISalesOrderCreated_CreateSalesOrderInRootStock: MessageId: {message.MessageId}: An error occurred while processing the request");
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, ex.InnerException?.Message);
            throw;
        }
    }

    #endregion
}
