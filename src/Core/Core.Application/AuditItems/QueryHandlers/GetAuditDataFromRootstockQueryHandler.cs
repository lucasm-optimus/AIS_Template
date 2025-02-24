namespace Tilray.Integrations.Core.Application.AuditItems.QueryHandlers;

public class GetAuditDataFromRootstockQueryHandler(IRootstockService rootstockService) : IQueryHandler<GetAuditData, SOXReportAgg>
{
    public async Task<Result<SOXReportAgg>> Handle(GetAuditData request, CancellationToken cancellationToken)
    {
        var reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var soxReport = await rootstockService.GetAuditItemsAsync(reportDate);
        var query = rootstockService.GetQuery(reportDate);
        if (soxReport.IsSuccess)
        {
            return Result.Ok(new SOXReportAgg { AuditItems = soxReport.Value, AuditItemsQuery = query });
        }
        return Result.Fail<SOXReportAgg>($"Failed to fetch audit items: {soxReport.Errors.FirstOrDefault()?.Message}");
        ;
    }
}
