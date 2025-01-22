using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Domain.Aggregates.Attendant;
using Optimus.Core.Domain.Aggregates.Attendant.Commands;
using Optimus.Core.Domain.Aggregates.Attendant.Events;
using Optimus.Core.Domain.Aggregates.Ticket;
using MediatR;

namespace Optimus.Core.Application.Attendant.Commands;

public sealed class DisableAttendantCommandValidator : AbstractValidator<DisableAttendantCommand>
{
    public DisableAttendantCommandValidator(IAttendantState state)
    {
        RuleFor(cmd => cmd.CardId)
            .NotEmpty()
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                instance.Attendang = await state.GetByCard(value);
                return instance?.Attendang != null;
            })
            .WithMessage("The CardId can't be empty");
    }
}

public sealed class EnableAttendantCommandValidator : AbstractValidator<EnableAttendantCommand>
{
    public EnableAttendantCommandValidator(IAttendantState state)
    {
        RuleFor(cmd => cmd.CardId)
            .NotEmpty()
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                instance.Attendang = await state.GetByCard(value);
                return instance?.Attendang != null;
            })
            .WithMessage("The CardId can't be empty");
    }
}

public class ChangeAttendantCommandHandler(IAttendantState state, IMediator mediator) : 
    ICommandHandler<DisableAttendantCommand, AttendantDisabled>,
    ICommandHandler<EnableAttendantCommand, AttendantEnabled>
{
    public async Task<Result<AttendantDisabled>> Handle(DisableAttendantCommand request, CancellationToken cancellationToken)
    {
        if (request.Attendang == null)
        {
            return Result.Fail("Not found").WithValidationError("Attendant", "Attendant not found");
        }

        var hasBeenChanged = request.Attendang.Disable();

        if (hasBeenChanged.IsSuccess && hasBeenChanged.Value != null)
        {
            Result<AttendantAgg>? saveResult = await state.Update(hasBeenChanged.Value.Attendant.Id, hasBeenChanged.Value.Attendant);

            if (saveResult.IsSuccess && saveResult?.Value != null)
            {
                await mediator.Publish(hasBeenChanged.Value);
                return hasBeenChanged.Value;
            }
            hasBeenChanged.WithErrors(saveResult?.Errors);
        }
        return hasBeenChanged;
    }

    public async Task<Result<AttendantEnabled>> Handle(EnableAttendantCommand request, CancellationToken cancellationToken)
    {
        if (request.Attendang == null)
        {
            return Result.Fail("Not found").WithValidationError("Attendant", "Attendant not found");
        }
        var hasBeenChanged = request.Attendang.Enable();

        if (hasBeenChanged.IsSuccess && hasBeenChanged.Value != null)
        {
            Result<AttendantAgg>? saveResult = await state.Update(hasBeenChanged.Value.Attendant.Id, hasBeenChanged.Value.Attendant);

            if (saveResult.IsSuccess && saveResult?.Value != null)
            {
                await mediator.Publish(hasBeenChanged.Value);
                return hasBeenChanged.Value;
            }
            hasBeenChanged.WithErrors(saveResult?.Errors);
        }
        return hasBeenChanged;
    }
}
