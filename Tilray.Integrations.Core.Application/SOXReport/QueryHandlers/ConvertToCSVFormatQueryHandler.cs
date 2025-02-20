using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries;

namespace Tilray.Integrations.Core.Application.SOXReport.QueryHandlers
{
    public class ConvertToCSVFormatQueryHandler(ILogger<ConvertToCSVFormatQueryHandler> _logger, ICSVService _csvService) : IQueryHandler<ConvertToCSVFormatQuery, string>
    {
        public async Task<Result<string>> Handle(ConvertToCSVFormatQuery request, CancellationToken cancellationToken)
        {
            var csvFile = _csvService.GenerateCsv(request.AuditItems);
            return csvFile;
        }

    }
}
