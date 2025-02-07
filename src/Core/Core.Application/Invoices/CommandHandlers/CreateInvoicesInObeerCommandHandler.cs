using MediatR;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Application.Invoices.CommandHandlers;

public class CreateInvoicesInObeerCommandHandler(IObeerService obeerService, IMediator mediator,
    ILogger<CreateInvoicesInObeerCommandHandler> logger) : ICommandHandler<CreateInvoicesInObeerCommand, InvoicesProcessed>
{
    public async Task<Result<InvoicesProcessed>> Handle(CreateInvoicesInObeerCommand request, CancellationToken cancellationToken)
    {
        var invoicesProcessed = new InvoicesProcessed();
        foreach (var invoice in request.Invoices)
        {
            await obeerService.CreateInvoiceAsync(invoice, invoicesProcessed);
        }

        if (invoicesProcessed.HasErrors)
        {
            logger.LogWarning(
                "Invoices processing completed with errors. GRPOErrors: {GrpoErrorCount}, NonPOErrors: {NonPoErrorCount}",
                invoicesProcessed.ErrorsGrpo.Count,
                invoicesProcessed.ErrorsNoPo.Count
            );

            invoicesProcessed.CompanyReference = request.Company;
            await mediator.Publish(invoicesProcessed);
            return Result.Fail(invoicesProcessed.Message);
        }

        logger.LogInformation("Successfully processed {InvoiceCount} invoices", request.Invoices.Count());
        return Result.Ok(invoicesProcessed);
    }
}
