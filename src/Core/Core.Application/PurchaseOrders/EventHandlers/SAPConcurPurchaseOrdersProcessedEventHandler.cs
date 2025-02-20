namespace Tilray.Integrations.Core.Application.PurchaseOrders.EventHandlers;

public class SAPConcurPurchaseOrdersProcessedEventHandler : INotificationHandler<SAPConcurPurchaseOrdersProcessed>
{
    private readonly ISharepointService sharepointService;
    private readonly IRootstockService rootstockService;
    private readonly ILogger<SAPConcurPurchaseOrdersProcessedEventHandler> logger;

    public SAPConcurPurchaseOrdersProcessedEventHandler(ISharepointService sharepointService, IRootstockService rootstockService, ILogger<SAPConcurPurchaseOrdersProcessedEventHandler> logger)
    {
        this.sharepointService = sharepointService;
        this.rootstockService = rootstockService;
        this.logger = logger;
    }

    public async Task Handle(SAPConcurPurchaseOrdersProcessed notification, CancellationToken cancellationToken)
    {
        if (notification.HasImportBatchItems())
        {
            await UploadImportBatchItems(notification, "Rootstock");
        }

        if (notification.HasErrorBatchItems())
        {
            await PostChatterMessageForErrors(notification, "Rootstock");
            await UploadErrorBatchItemsToSharePoint(notification, "Rootstock");
        }
    }

    private async Task UploadImportBatchItems(SAPConcurPurchaseOrdersProcessed importErrorBatchItem, string erp)
    {
        var importPath = $"General/PurchaseOrders/PurchaseOrderSync_{erp}_{DateTime.Now:yyyy-MM-dd-HHmmss}.csv";
        var result = await sharepointService.UploadError_ImportbatchAsync(importErrorBatchItem.ImportBatchItem, importPath);
        if (result.IsFailed)
        {
            LogErrors(result.Errors, "ImportBatch");
        }
    }

    private async Task PostChatterMessageForErrors(SAPConcurPurchaseOrdersProcessed importErrorBatchItem, string erp)
    {
        var chatterGroupIdResult = await rootstockService.RetrieveGroupAsync();

        if (chatterGroupIdResult.IsSuccess)
        {
            var chatterGroupId = chatterGroupIdResult.Value;
            var chatterMessage = ChatterMessage.CreateForPurchaseOrderSync(chatterGroupId, erp, importErrorBatchItem.ErrorBatchItem.Count());

            await PostChatterMessageAsync(chatterMessage);
        }
    }

    private async Task UploadErrorBatchItemsToSharePoint(SAPConcurPurchaseOrdersProcessed importErrorBatchItem, string erp)
    {
        var errorPath = $"General/PurchaseOrders/Errors/PurchaseOrderSync_{erp}_Errors_{DateTime.Now:yyyy-MM-dd-HHmmss}.csv";
        var result = await sharepointService.UploadError_ImportbatchAsync(importErrorBatchItem.ErrorBatchItem, errorPath);
        if (result.IsFailed)
        {
            LogErrors(result.Errors, "ErrorBatch");
        }
    }

    private async Task<Result> PostChatterMessageAsync(ChatterMessage chatterMessage)
    {
        var existingPostResult = await rootstockService.CheckIfChatterPostExistsAsync(chatterMessage.RecordIDToAddFeedItemTo, chatterMessage.MessagePieces?.FirstOrDefault()?.Text ?? string.Empty);
        if (existingPostResult.IsFailed)
        {
            return Result.Fail(existingPostResult.Errors);
        }

        if (existingPostResult.Value)
        {
            return Result.Ok();
        }

        return await rootstockService.CreateChatterPostAsync(chatterMessage);
    }

    private void LogErrors(IEnumerable<IError> errors, string errorType)
    {
        foreach (var error in errors)
        {
            logger.LogError("Failed to upload {ErrorType} errors: {Message}", errorType, error.Message);
        }
    }
}