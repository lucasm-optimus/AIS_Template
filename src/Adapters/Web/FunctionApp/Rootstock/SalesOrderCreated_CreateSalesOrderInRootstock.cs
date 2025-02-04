using Tilray.Integrations.Core.Common.Extensions;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Commands;

namespace Tilray.Integrations.Functions.Rootstock;

public class SalesOrderCreated_CreateSalesOrderInRootstock(IMediator mediator, ILogger<SalesOrderCreated_CreateSalesOrderInRootstock> logger)
{
    /// <summary>
    /// This function is responsible for creating the sales order in Rootstock.
    /// </summary>
    [Function(nameof(SalesOrderCreated_CreateSalesOrderInRootstock))]
    public async Task Run(
        [ServiceBusTrigger("%TopicSalesOrderCreated%", "%SubscriptionCreateSalesOrderInRootStock%", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var salesOrder = message.Body.ToString().ToObject<SalesOrder>();
        var result = await mediator.Send(new CreateSalesOrderInRootstockCommand(salesOrder));
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: string.Join(", ", result.Errors));
        }
    }
}
