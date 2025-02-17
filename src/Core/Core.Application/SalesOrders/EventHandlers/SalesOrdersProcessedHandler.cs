using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Application.SalesOrders.EventHandlers
{
    public class SalesOrdersProcessedHandler(ServiceBusClient serviceBusClient) : IDomainEventHandler<SalesOrdersProcessed>
    {
        public async Task Handle(SalesOrdersProcessed notification, CancellationToken cancellationToken)
        {
            var orders = notification.SuccessfullOrders;
            var sender = serviceBusClient.CreateSender(Topics.EcomSalesOrderReceived);

            var tasks = orders.Select(async salesOrder =>
            {
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(salesOrder));
                await sender.SendMessageAsync(message);
            });

            await Task.WhenAll(tasks);
        }
    }
}
