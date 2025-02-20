using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;

namespace Tilray.Integrations.Core.Application.SOXReport.CommandHandlers
{
    public class SaveReportToOneDriveCommandHandler(IOneDriveService _oneDriveService, ILogger<SaveReportToOneDriveCommandHandler> _logger) : ICommandHandler<SaveReportToOneDriveCommand>
    {
        public async Task<Result> Handle(SaveReportToOneDriveCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await _oneDriveService.PutFileAsync(request.FileName, request.ReportContent, "text/plain");

            if (uploadResult.IsSuccess)
            {
                _logger.LogInformation($"Successfully saved report to OneDrive: {request.FileName}");
                return Result.Ok();
            }
            _logger.LogError($"Failed to save report to OneDrive: {request.FileName}");
            return Result.Fail("Failed to save report to OneDrive");
        }

    }
}
