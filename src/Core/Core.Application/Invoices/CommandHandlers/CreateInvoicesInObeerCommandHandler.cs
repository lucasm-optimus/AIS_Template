namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class CreateInvoicesInObeerCommandHandler(IObeerService obeerService, ISharepointService sharepointService) : ICommandHandler<CreateInvoicesInObeerCommand>
{
    public async Task<Result> Handle(CreateInvoicesInObeerCommand request, CancellationToken cancellationToken)
    {
        var invoiceResult = await obeerService.CreateInvoicesAsync(request.Invoices);
        if (invoiceResult.IsSuccess) return Result.Ok();

        var uploadErrorsGrpoResult = await sharepointService.UploadFileAsync(invoiceResult.Value.ErrorsGrpo, request.CompanyReference);
        var uploadErrorsNoPoResult = await sharepointService.UploadFileAsync(invoiceResult.Value.ErrorsNoPo, request.CompanyReference);

        if (uploadErrorsGrpoResult.IsFailed || uploadErrorsNoPoResult.IsFailed)
        {
            var errors = new List<IError>();
            if (uploadErrorsGrpoResult.IsFailed) errors.AddRange(uploadErrorsGrpoResult.Errors);
            if (uploadErrorsNoPoResult.IsFailed) errors.AddRange(uploadErrorsNoPoResult.Errors);
            return Result.Fail(errors);
        }

        return Result.Ok();
    }
}
