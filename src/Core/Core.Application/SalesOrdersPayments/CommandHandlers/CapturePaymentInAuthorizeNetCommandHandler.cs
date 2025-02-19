using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Events;

namespace Tilray.Integrations.Core.Application.SalesOrdersPayments.CommandHandlers;

public class CapturePaymentInAuthorizeNetCommandHandler(IAuthorizeNetService authorizeNetService, IMediator mediator) : ICommandHandler<CapturePaymentInAuthorizeNetCommand, SalesOrderPaymentProcessed>
{
    public async Task<Result<SalesOrderPaymentProcessed>> Handle(CapturePaymentInAuthorizeNetCommand request, CancellationToken cancellationToken)
    {
        var result = await authorizeNetService.CapturePaymentAsync(request.TransactionId, request.PaymentAmount);
        if (result.IsSuccess)
        {
            return Result.Ok(new SalesOrderPaymentProcessed(request.Id, request.TransactionId));
        }

        var error = result.Errors.FirstOrDefault();
        switch (error)
        {
            case SalesOrderPaymentAlreadyProcessedError alreadyProcessed:
                await mediator.Publish(new CaptureAttemptedOnInvalidTransaction(alreadyProcessed.TransactionId,
                    alreadyProcessed.Status), cancellationToken);
                break;

            default:
                await mediator.Publish(new CapturePaymentFailed(request.TransactionId,
                    Helpers.GetErrorMessage(result.Errors)), cancellationToken);
                break;
        }

        return Result.Fail<SalesOrderPaymentProcessed>(result.Errors);
    }
}
