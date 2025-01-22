using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ChangeKilometersCommandValidator : AbstractValidator<ChangeKilometersCommand>
    {
        public ChangeKilometersCommandValidator(ITicketState ticketState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    instance.Ticket = await ticketState.Get(propValue);
                    return instance.Ticket != null;
                });

            RuleFor(command => command.KMs)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Invalid kilometers");
        }
    }

    public class ChangeKilometersCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<ChangeKilometersCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(ChangeKilometersCommand command, CancellationToken cancellationToken)
        {
            var driverChanged = command.Ticket.ChangeKilometers(command.KMs);

            if (driverChanged.IsSuccess)
            {
                var saved = await ticketState.Update(command.TicketId, driverChanged.Value.Ticket);

                if (saved.IsSuccess)
                    await mediator.Publish(driverChanged.Value);

                driverChanged.WithErrors(saved.Errors);
            }

            return driverChanged;
        }
    }
}
