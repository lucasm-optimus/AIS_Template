using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock;

public class SalesOrderCreated_CreateSalesOrderInRootstock(IMediator mediator, ILogger<SalesOrderCreated_CreateSalesOrderInRootstock> logger)
{
    /// <summary>
    /// This function is responsible for creating the sales order in Rootstock.
    /// </summary>
    [Function(nameof(SalesOrderCreated_CreateSalesOrderInRootstock))]
    public async Task Run(
        [ServiceBusTrigger(Topics.SalesOrderCreated, Subscriptions.CreateSalesOrderInRootStock, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var salesOrder = message.Body.ToString().ToObject<SalesOrder>();
        var result = await mediator.Send(new CreateSalesOrderInRootstockCommand(salesOrder));
        if (result.IsFailed)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(result.Errors));
        }
    }
}
