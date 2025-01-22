using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ChangeTicketDriverCommandValidator : AbstractValidator<ChangeTicketDriverCommand>
    {
        public ChangeTicketDriverCommandValidator(ITicketState ticketState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    instance.Ticket = await ticketState.Get(propValue);
                    return instance.Ticket != null;
                });

            RuleFor(command => command.CPF)
                .NotEmpty()
                .IsValidCPF()
                .WithMessage("Invalid driver document");
        }
    }

    public class ChangeTicketDriverCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<ChangeTicketDriverCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(ChangeTicketDriverCommand command, CancellationToken cancellationToken)
        {
            var driverChanged = command.Ticket.ChangeDriver(command);

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
