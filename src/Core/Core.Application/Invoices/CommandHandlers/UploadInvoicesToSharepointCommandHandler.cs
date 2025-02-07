namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadInvoicesToSharepointCommandHandler(ISharepointService sharepointService, IMapper mapper) : ICommandHandler<UploadInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        if (!request.Invoices.Any())
        {
            return Result.Ok();
        }

        return await sharepointService.UploadFileAsync(request.Invoices, request.Company);
    }
}
