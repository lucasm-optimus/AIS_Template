using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Domain.Aggregates.Attendant;
using Optimus.Core.Domain.Aggregates.Attendant.Commands;
using Optimus.Core.Domain.Aggregates.Attendant.Events;

namespace Optimus.Core.Application.Attendant.Commands;
#pragma warning disable

public sealed class CreateAttendantValidator : AbstractValidator<CreateAttendantCommand>
{
    public CreateAttendantValidator()
    {
        RuleFor(cmd => cmd.CardId)
            .NotEmpty()
            .WithMessage("The Attendant' CardId can't be empty");

        RuleFor(cmd => cmd.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("The Name can't be empty");

        RuleFor(cmd => cmd.Name)
        .MinimumLength(2)
        .WithMessage("Minimum length should be {MinimumLength} characters");

        RuleFor(cmd => cmd.Name)
        .MaximumLength(75)
        .WithMessage("Maximum length should be {MaximumLength} characters");

    }
}

public class CreateAttendantCommandHandler(IAttendantState state, IMediator mediator) : ICommandHandler<CreateAttendantCommand, AttendantCreated>
{
    public async Task<Result<AttendantCreated>> Handle(CreateAttendantCommand command, CancellationToken cancellationToken)
    {
        AttendantAgg? existing = await state.GetByCard(command.CardId);
        if (existing != null)
        {
            var result = Result.Ok();
            return result.WithValidationError("CardId", "This CardId is already in use");
        }

        var attendantHasBeenCreated = AttendantAgg.Create(command);

        if (attendantHasBeenCreated.IsSuccess)
        {
            Result<AttendantAgg>? saveResult = await state.Add(attendantHasBeenCreated.Value.Attendant);

            if (saveResult.IsSuccess && saveResult?.Value != null)
            {
                await mediator.Publish(attendantHasBeenCreated.Value);
                return attendantHasBeenCreated;
            }
            return Result.Fail<AttendantCreated>("Attendant creation failed!").WithErrors(saveResult?.Errors);
        }
        return attendantHasBeenCreated;
    }
}
