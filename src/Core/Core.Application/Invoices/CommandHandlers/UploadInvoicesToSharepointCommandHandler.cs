using Microsoft.Extensions.Logging;

namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class UploadInvoicesToSharepointCommandHandler(ISharepointService sharepointService, ILogger<UploadInvoicesToSharepointCommandHandler> logger) : ICommandHandler<UploadInvoicesToSharepointCommand>
{
    public async Task<Result> Handle(UploadInvoicesToSharepointCommand request, CancellationToken cancellationToken)
    {
        if (request.Invoices?.Any() != true)
        {
            return Result.Ok();
        }

        if (!request.Company.OBeer_Invoices__c)
        {
            logger.LogInformation("Company {CompanyName} is not configured to process invoices for Obeer",
                request.Company.Company_Name__c);
            return Result.Ok();
        }

        return await sharepointService.UploadFileAsync(request.Invoices, request.Company);
    }
}
