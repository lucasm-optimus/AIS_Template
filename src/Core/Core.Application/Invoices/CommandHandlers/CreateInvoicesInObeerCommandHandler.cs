namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class CreateInvoicesInObeerCommandHandler(IObeerService obeerService, ISharepointService sharepointService) : ICommandHandler<CreateInvoicesInObeerCommand>
{
    public async Task<Result> Handle(CreateInvoicesInObeerCommand request, CancellationToken cancellationToken)
    {
        var result = await obeerService.CreateInvoicesAsync(request.Invoices);
        if (result.IsFailed)
        {
            await sharepointService.UploadFileAsync(result.Value.ErrorsGrpo, request.CompanyReference);
            await sharepointService.UploadFileAsync(result.Value.ErrorsNoPo, request.CompanyReference);
        }

        return Result.Ok();
    }
}
