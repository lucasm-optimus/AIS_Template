namespace Tilray.Integrations.Core.Application.AuditItems.QueryHandlers;

public class GetAuditDataFromRootstockQueryHandler(IRootstockService rootstockService) : IQueryHandler<GetAuditData, AuditItemAgg>
{
    public async Task<Result<AuditItemAgg>> Handle(GetAuditData request, CancellationToken cancellationToken)
    {
        var reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var soxReport = await rootstockService.GetAuditItemsAsync(reportDate);
        var query = rootstockService.GetAuditItemsQuery(reportDate);
        if (soxReport.IsSuccess)
        {
            var auditItemAgg = AuditItemAgg.Create(soxReport.Value, query);
            return Result.Ok(auditItemAgg);
        }
        return Result.Fail<AuditItemAgg>($"Failed to fetch audit items: {soxReport.Errors.FirstOrDefault()?.Message}");
    }
}
