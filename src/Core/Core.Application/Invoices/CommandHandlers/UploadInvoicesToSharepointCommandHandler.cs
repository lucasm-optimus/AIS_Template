namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadInvoicesToSharepointCommandHandler(ISharepointService sharepointService, IMapper mapper) : ICommandHandler<UploadInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        var sharepointInvoices = request.Invoices
            .SelectMany(invoice =>
                invoice.LineItems.LineItem.Select((lineItem, index) =>
                new { invoice, lineItem, LineItemNumber = index + 1 })
                    )
            .Select(x => mapper.Map<SharepointInvoice>((x.invoice, x.lineItem, x.LineItemNumber)))
            .ToList();
        return await sharepointService.UploadFileAsync(sharepointInvoices, request.CompanyReference);
    }
}
