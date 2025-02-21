
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;

namespace Tilray.Integrations.Core.Application.SOXReport.CommandHandlers
{
    public class UploadReportToOneDriveCommandHandler(ISharepointService sharepointService, ILogger<UploadReportToOneDriveCommandHandler> _logger) : ICommandHandler<UploadReportToOneDriveCommand>
    {
        public async Task<Result> Handle(UploadReportToOneDriveCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await sharepointService.UploadSOXReportAsync(request.AuditItems);

            if (uploadResult.IsSuccess)
            {
                _logger.LogInformation($"Successfully saved report to OneDrive");
                return Result.Ok();
            }
            _logger.LogError($"Failed to save report to OneDrive");
            return Result.Fail("Failed to save report to OneDrive");


        }

    }
}
