using Tilray.Integrations.Core.Application.Adapters.Stream;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Application.SalesOrders.EventHandlers
{
    public class SalesOrdersProcessedHandler(IStreamService stream) : IDomainEventHandler<SalesOrdersProcessed>
    {
        public async Task Handle(SalesOrdersProcessed notification, CancellationToken cancellationToken)
        {
            var orders = notification.SuccessfullOrders;


            var tasks = orders.Select(async salesOrder =>
            {
                await stream.SendEventAsync(salesOrder, Topics.EcomSalesOrderReceived);
            });

            await Task.WhenAll(tasks);
        }
    }
}
