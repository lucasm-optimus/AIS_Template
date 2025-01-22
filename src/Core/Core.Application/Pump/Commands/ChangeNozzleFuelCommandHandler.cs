using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Application.Pump.Commands;

/// <summary>
/// The command validator contains the rules to ensure the object is valid
/// </summary>
public sealed class ChangeNozzleFuelCommandValidator : AbstractValidator<ChangeNozzleFuelCommand>
{
    public ChangeNozzleFuelCommandValidator()
    {
        RuleFor(command => command.PumpNumber)
            .NotEmpty();

        RuleFor(command => command.NozzleId)
            .NotEmpty();

        RuleFor(command => command.Color)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty();

        RuleFor(command => command.Price)
            .NotEmpty();

    }
}

public class ChangeNozzleFuelCommandHandler(IPumpState pumpState, IMediator mediator) : ICommandHandler<ChangeNozzleFuelCommand, FuelChanged>
{
    public async Task<Result<FuelChanged>> Handle(ChangeNozzleFuelCommand request, CancellationToken cancellationToken)
    {
        var existingPump = await pumpState.Get(x => x.Number == request.PumpNumber);

        if (existingPump == null)
            return Result.Ok().WithValidationError("PumpNumber", $"Pump with No.{request.PumpNumber} not found");

        var existingNozzle = existingPump?.Nozzles?.FirstOrDefault(x => x.Number == request.NozzleId);

        if (existingNozzle == null)
            return Result.Ok().WithValidationError("NozzleId", $"Nozzle with Id {request.NozzleId} not found in Pump {request.PumpNumber}");


        var changed = existingNozzle.ChangeFuel(request);

        if (changed.IsSuccess)
        {
            existingPump.ChangeNozzle(existingNozzle);

            var saveResult = await pumpState.Update(existingPump.Id.ToString(), existingPump);
            if (saveResult.IsSuccess)
            {
                await mediator.Publish(changed.Value);

                return changed;
            }
        }

        return changed;
    }
}

