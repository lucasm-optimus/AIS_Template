using System.IO;
using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Core.Application.PurchaseOrders.EventHandlers;

public class OBeerPurchaseOrdersProcessedEventHandler(IStream stream) : IDomainEventHandler<OBeerPurchaseOrdersProcessed>
{
    public async Task Handle(OBeerPurchaseOrdersProcessed notification, CancellationToken cancellationToken)
    {
        var tasks = notification.PurchaseOrders.Select(async purchaseOrder =>
        {
            await stream.SendEventAsync(purchaseOrder, Topics.OBeerPurchaseOrdersFetched);
        });

        await Task.WhenAll(tasks);
    }
}
