
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;

namespace Tilray.Integrations.Core.Application.SOXReport.CommandHandlers
{
    public class UploadQueryToOneDriveCommandHandler(ISharepointService sharepointService, ILogger<UploadQueryToOneDriveCommandHandler> _logger) : ICommandHandler<UploadQueryToOneDriveCommand>
    {
        public async Task<Result> Handle(UploadQueryToOneDriveCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await sharepointService.UploadSFQueryAsync(request.Query);

            if (uploadResult.IsSuccess)
            {
                _logger.LogInformation($"Successfully saved query to OneDrive");
                return Result.Ok();
            }
            _logger.LogError($"Failed to save query to OneDrive");
            return Result.Fail("Failed to save query to OneDrive.");
        }
    }
}
