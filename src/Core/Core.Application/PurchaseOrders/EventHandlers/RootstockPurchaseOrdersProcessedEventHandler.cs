using System.IO;
using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Application.PurchaseOrders.EventHandlers;

public class RootstockPurchaseOrdersProcessedEventHandler(IStream stream) : IDomainEventHandler<RootstockPurchaseOrdersProcessed>
{
    public async Task Handle(RootstockPurchaseOrdersProcessed notification, CancellationToken cancellationToken)
    {
        var tasks = notification.PurchaseOrders.Select(async purchaseOrder =>
        {
            await stream.SendEventAsync(purchaseOrder, Topics.RootstockPurchaseOrderFetched);
        });

        await Task.WhenAll(tasks);

    }
}
