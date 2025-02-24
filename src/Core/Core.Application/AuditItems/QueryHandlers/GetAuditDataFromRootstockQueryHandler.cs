namespace Tilray.Integrations.Core.Application.AuditItems.QueryHandlers;

public class GetAuditDataFromRootstockQueryHandler(IRootstockService rootstockService) : IQueryHandler<GetAuditData, AuditItemAgg>
{
    public async Task<Result<AuditItemAgg>> Handle(GetAuditData request, CancellationToken cancellationToken)
    {
        var reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var auditItemsResult = await rootstockService.GetAuditItemsAsync(reportDate);
        var auditItemsQuery = rootstockService.GetAuditItemsQuery(reportDate);
        if (auditItemsResult.IsSuccess)
        {
            var auditItemAgg = AuditItemAgg.Create(auditItemsResult.Value, auditItemsQuery);
            return Result.Ok(auditItemAgg);
        }

        return Result.Fail<AuditItemAgg>(auditItemsResult.Errors);
    }
}
