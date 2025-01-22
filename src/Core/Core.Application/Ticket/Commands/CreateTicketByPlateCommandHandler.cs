using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Application.Vehicle.Adapters.Services;
using Optimus.Core.Application.Vehicle.Adapters.States;
using Optimus.Core.Domain.Aggregates.Ticket;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class CreateTicketByPlateCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketByPlateCommandValidator(IAttendantState attendantState, IPlateService plateService)
        {
            RuleFor(command => command.CardId)
                .NotEmpty()
                .MustAsync(async (instance, value, cancellationToken) => {
                    instance.Attendant = await attendantState.Get(x => x.CardId == value);

                    return instance.Attendant != null;
                });

            RuleFor(command => command.Plate)
                .NotEmpty()
                .MustAsync(async (instance, value, cancellationToken) => {
                    var result = await plateService.GetPlate(value);
                    if (result.IsSuccess)
                        instance.Vehicle = result.Value;

                    return instance.Vehicle != null;
                })
                .WithMessage("Invalid plate");

        }
    }

    /// <summary>
    /// The command handler is the Application class responsible for connect multiple adapters and run the business logic in the domain
    /// </summary>
    public class CreateTicketByPlateCommandHandler(ITicketState ticketState, IPlateService plateService, IMediator mediator) : ICommandHandler<CreateTicketCommand, TicketCreated>
    {
        public async Task<Result<TicketCreated>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            if (request.Attendant is null)
                return Result.Fail<TicketCreated>("Ticket creation failed").WithValidationError("CardId", $"Attendant not found");

            var vehicle = await plateService.GetPlate(request.Plate);

            if (vehicle.IsFailed)
                return Result.Fail<TicketCreated>("Ticket creation failed").WithValidationError("Plate", $"Plate not found");

            var vehicleEntity = Domain.Aggregates.Ticket.Vehicle.Create(vehicle.Value);
            var ticketHasBeenCreated = TicketAgg.Create(vehicleEntity, Domain.Aggregates.Ticket.Attendant.Create(request.Attendant));

            if (ticketHasBeenCreated.IsSuccess)
            {
                var saveResult = await ticketState.Add(ticketHasBeenCreated.Value.Ticket);
                if (saveResult.IsSuccess)
                    await mediator.Publish(saveResult.Value.Created());

                ticketHasBeenCreated.WithErrors(saveResult?.Errors);
            }

            return ticketHasBeenCreated;
        }
    }

}
