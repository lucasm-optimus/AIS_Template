namespace Tilray.Integrations.Core.Application.AuditItems.CommandHandlers;

public class UploadAuditDataToSharepointCommandHandler(ISharepointService sharepointService, ILogger<UploadAuditDataToSharepointCommandHandler> _logger)
    : ICommandHandler<UploadAuditDataToSharepointCommand>
{
    public async Task<Result> Handle(UploadAuditDataToSharepointCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<IError>();

        var auditItemsQueryResult = await sharepointService.UploadAuditItemsQueryAsync(request.AuditItemsQuery);
        if (auditItemsQueryResult.IsFailed)
        {
            _logger.LogError("Failed to upload audit items query to Sharepoint");
            errors.AddRange(auditItemsQueryResult.Errors);
        }

        var auditItemsResult = await sharepointService.UploadAuditItemsAsync(request.AuditItems);
        if (auditItemsResult.IsFailed)
        {
            _logger.LogError("Failed to upload audit items to Sharepoint");
            errors.AddRange(auditItemsResult.Errors);
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }
}
