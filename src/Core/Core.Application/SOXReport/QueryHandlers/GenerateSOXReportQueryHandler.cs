using FluentResults;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries;

namespace Tilray.Integrations.Core.Application.SOXReport.QueryHandlers
{
    public class GenerateSOXReportQueryHandler( IRootstockService rootstockService) : IQueryHandler<GenerateSOXReportQuery, SOXReportAgg>
    {
        public async Task<Result<SOXReportAgg>> Handle(GenerateSOXReportQuery request, CancellationToken cancellationToken)
        {
            var reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var soxReport = await rootstockService.GetAuditItemsAsync(reportDate);
            var query = rootstockService.GetQuery(reportDate);
            if (soxReport.IsSuccess)
            {
                return Result.Ok(new SOXReportAgg { ReportItems = soxReport.Value, Query = query });
            }
            return Result.Fail<SOXReportAgg>($"Failed to fetch audit items: {soxReport.Errors.FirstOrDefault()?.Message}");
;        }
    }
}
