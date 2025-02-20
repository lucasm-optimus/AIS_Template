using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;

namespace Tilray.Integrations.Core.Application.SOXReport.CommandHandlers
{
    public class SaveQueryToOneDriveCommandHandler(IOneDriveService _oneDriveService, ILogger<SaveQueryToOneDriveCommandHandler> _logger) : ICommandHandler<SaveQueryToOneDriveCommand>
    {
        public async Task<Result> Handle(SaveQueryToOneDriveCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await _oneDriveService.PutFileAsync(request.FileName, request.Query, "text/plain");

            if (uploadResult.IsSuccess)
            {
                _logger.LogInformation($"Successfully saved query to OneDrive: {request.FileName}");
                return Result.Ok();
            }
            _logger.LogError($"Failed to save query to OneDrive: {request.FileName}");
            return Result.Fail("Failed to save query to OneDrive.");
        }
    }
}
