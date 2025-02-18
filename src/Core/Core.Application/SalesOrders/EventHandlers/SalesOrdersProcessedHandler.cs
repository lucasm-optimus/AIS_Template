using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Application.SalesOrders.EventHandlers
{
    public class SalesOrdersProcessedHandler(IStream stream) : IDomainEventHandler<SalesOrdersProcessed>
    {
        public async Task Handle(SalesOrdersProcessed notification, CancellationToken cancellationToken)
        {
            var tasks = notification.SuccessfullOrders.Select(async salesOrder =>
            {
                await stream.SendEventAsync(salesOrder, Topics.EcomSalesOrderReceived);
            });

            await Task.WhenAll(tasks);
        }
    }
}
