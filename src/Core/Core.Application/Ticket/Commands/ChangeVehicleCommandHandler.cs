using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Application.Vehicle.Queries;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ChangeVehicleCommandValidator : AbstractValidator<ChangeVehicleCommand>
    {
        public ChangeVehicleCommandValidator(ITicketState ticketState, IMediator mediator)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, value, c) =>
                {
                    instance.Ticket = await ticketState.Get(value);
                    return instance.Ticket != null;
                }).DependentRules(() => {
                    RuleFor(x => x.Ticket.Vehicle)
                        .Must((i, v, c) => {
                            if (i?.Ticket?.Vehicle is null)
                                return true;

                            return i.Ticket.Vehicle.LastChangeDate.AddMinutes(5) < DateTime.Now;
                        }).WithMessage("Cannot change plate after 5 minutes");
                });

            RuleFor(command => command.Plate)
                .NotEmpty()
                .MustAsync(async (instance, value, c) =>
                {
                    var result = await mediator.Send(new VehicleGetByPlate(value));
                    if (result.IsSuccess)
                        instance.Vehicle = result.Value;

                    return instance.Vehicle != null;
                }).WithMessage("Plate does not exist");
        }
    }

    public class ChangeVehicleCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<ChangeVehicleCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(ChangeVehicleCommand command, CancellationToken cancellationToken)
        {
            var changed = command.Ticket.ChangeVehicle(command.Vehicle);

            if (changed.IsSuccess)
            {
                var saved = await ticketState.Update(command.TicketId, changed.Value.Ticket);

                if (saved.IsSuccess)
                    await mediator.Publish(changed.Value);

                changed.WithErrors(saved.Errors);
            }

            return changed;
        }
    }
}
