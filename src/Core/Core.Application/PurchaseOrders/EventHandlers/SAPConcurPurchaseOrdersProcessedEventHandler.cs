using Tilray.Integrations.Core.Application.Constants;
using static Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Constants;

namespace Tilray.Integrations.Core.Application.PurchaseOrders.EventHandlers;

public class SAPConcurPurchaseOrdersProcessedEventHandler(ISharepointService sharepointService, IRootstockService rootstockService, ILogger<SAPConcurPurchaseOrdersProcessedEventHandler> logger) : INotificationHandler<SAPConcurPurchaseOrdersProcessed>
{
    public async Task Handle(SAPConcurPurchaseOrdersProcessed notification, CancellationToken cancellationToken)
    {
        if (notification.HasProcessedPurchaseOrders())
        {
            await UploadProcessedPurchaseOrders(notification, GroupNames.Rootstock);
        }

        if (notification.HasFailedPurchaseOrders())
        {
            await PostChatterMessageForErrors(notification, GroupNames.Rootstock);
            await UploadFailedPurchaseOrdersToSharePoint(notification, GroupNames.Rootstock);
        }
    }

    private async Task UploadProcessedPurchaseOrders(SAPConcurPurchaseOrdersProcessed processedPurchaseOrders, string erp)
    {
        var importPath = $"General/PurchaseOrders/PurchaseOrderSync_{erp}_{DateTime.Now:yyyy-MM-dd-HHmmss-fff}.csv";
        var result = await sharepointService.UploadProcessedPurchaseOrdersAsync(processedPurchaseOrders.ProcessedPurchaseOrders, importPath);
        if (result.IsFailed)
        {
            LogErrors(result.Errors, "processedPurchaseOrders");
        }
    }

    private async Task PostChatterMessageForErrors(SAPConcurPurchaseOrdersProcessed failedPurchaseOrders, string erp)
    {
        var chatterGroupResult  = await rootstockService.PostPurchaseOrdersMessageToChatterAsync(erp, failedPurchaseOrders.FailedPurchaseOrders.Count());
    }

    private async Task UploadFailedPurchaseOrdersToSharePoint(SAPConcurPurchaseOrdersProcessed failedPurchaseOrders, string erp)
    {
        var errorPath = $"General/PurchaseOrders/Errors/PurchaseOrderSync_{erp}_Errors_{DateTime.Now:yyyy-MM-dd-HHmmss-fff}.csv";
        var result = await sharepointService.UploadFailedPurchaseOrdersAsync(failedPurchaseOrders.FailedPurchaseOrders, errorPath);
        if (result.IsFailed)
        {
            LogErrors(result.Errors, "failedPurchaseOrders");
        }
    }

    private void LogErrors(IEnumerable<IError> errors, string errorType)
    {
        foreach (var error in errors)
        {
            logger.LogError("Failed to upload {ErrorType} errors: {Message}", errorType, error.Message);
        }
    }
}