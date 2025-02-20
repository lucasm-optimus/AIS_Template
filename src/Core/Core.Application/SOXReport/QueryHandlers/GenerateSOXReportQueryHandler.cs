using FluentResults;
using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries;

namespace Tilray.Integrations.Core.Application.SOXReport.QueryHandlers
{
    public class GenerateSOXReportQueryHandler(ILogger<GenerateSOXReportQueryHandler> _logger, ISalesforceService _salesforceService) : IQueryHandler<GenerateSOXReportQuery, List<AuditItem>>
    {
        public async Task<Result<List<AuditItem>>> Handle(GenerateSOXReportQuery request, CancellationToken cancellationToken)
        {
            var soxReport = await _salesforceService.GetAuditItemsAsync(request.ReportDate);
            if (soxReport.IsSuccess)
            {
                return Result.Ok(soxReport.Value);
            }
            return Result.Fail<List<AuditItem>>($"Failed to fetch audit items: {soxReport.Errors.FirstOrDefault()?.Message}");
        }
    }
}
