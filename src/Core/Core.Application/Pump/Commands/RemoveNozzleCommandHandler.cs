using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Application.Pump.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public class RemoveNozzleCommandValidator : AbstractValidator<RemoveNozzleCommand>
    {
        public RemoveNozzleCommandValidator()
        {
            RuleFor(command => command.PumpNumber)
                .NotEmpty();

            RuleFor(command => command.NozzleNumber)
                .NotEmpty();
        }
    }

    public class RemoveNozzleCommandHandler(IPumpState state, IMediator mediator) : ICommandHandler<RemoveNozzleCommand, NozzleRemoved>
    {
        public async Task<Result<NozzleRemoved>> Handle(RemoveNozzleCommand request, CancellationToken cancellationToken)
        {
            var pump = await state.Get(x => x.Number == request.PumpNumber);
            if (pump == null)
                return Result.Ok().WithValidationError("Number", $"Pump with No.{request.PumpNumber} not found");

            var nozzleRemoved = pump.RemoveNozzle(request.NozzleNumber);

            if (nozzleRemoved.IsSuccess)
            {
                await state.Update(pump.Id.ToString(), pump);
                await mediator.Publish(nozzleRemoved.Value);
                return Result.Ok(nozzleRemoved.Value);
            }

            return nozzleRemoved;
        }
    }
}
