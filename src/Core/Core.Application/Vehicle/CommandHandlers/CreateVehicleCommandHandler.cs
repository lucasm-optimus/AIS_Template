using Optimus.Core.Domain.Aggregates.Vehicle.Commands;
using Optimus.Core.Domain.Aggregates.Vehicle.Events;

namespace Optimus.Core.Application.Vehicle.CommandHandlers
{

    public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator()
        {
            RuleFor(command => command.plate)
                .NotEmpty()
                .WithMessage("The Plate can't be empty.");
        }
    }

    public class CreateVehicleCommandHandler : ICommandHandler<CreateVehicleCommand, VehicleCreated>
    {
        public Task<Result<VehicleCreated>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
