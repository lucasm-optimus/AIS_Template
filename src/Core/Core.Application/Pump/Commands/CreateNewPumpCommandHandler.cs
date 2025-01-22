using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump;
using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Application.Pump.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class CreateNewPumpCommandValidator : AbstractValidator<CreateNewPumpCommand>
    {
        public CreateNewPumpCommandValidator()
        {
            RuleFor(command => command.Number)
                .NotEmpty();

            RuleFor(command => command.Description)
                .NotEmpty();
        }
    }

    public class CreateNewPumpCommandHandler(IPumpState state, IMediator mediator) : ICommandHandler<CreateNewPumpCommand, PumpCreated>
    {
        public async Task<Result<PumpCreated>> Handle(CreateNewPumpCommand request, CancellationToken cancellationToken)
        {
            var existing = await state.Get(x => x.Number == request.Number);

            if (existing != null)
                return Result.Ok().WithValidationError("Number", $"Pump with No.{request.Number} already exists");

            var pump = PumpAgg.Create(request);

            var fail = Result.Fail<PumpCreated>("Pump creation failed");

            if (pump != null && pump.IsSuccess)
            {
                var saveResult = await state.Add(pump.Value);
                if (saveResult?.Value != null)
                {
                    var pumpCreated = saveResult?.Value?.Created() ?? fail;

                    await mediator.Publish(pumpCreated.Value);

                    return pumpCreated;
                }
            }

            return fail;
        }
    }
}
