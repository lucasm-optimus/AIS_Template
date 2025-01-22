using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ChangePaymentCommandValidator : AbstractValidator<ChangePaymentCommand>
    {
        public ChangePaymentCommandValidator(ITicketState ticketState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    instance.Ticket = await ticketState.Get(propValue);
                    return instance.Ticket != null;
                });

            RuleFor(command => command.Payment)
                .NotEmpty()
                .WithMessage("Invalid payment")
                .ChildRules(v => { 
                    RuleFor(c => c.Payment.Id)
                        .NotEmpty()
                        .WithMessage("Invalid payment id");

                    RuleFor(c => c.Payment.Name)
                        .NotEmpty()
                        .WithMessage("Invalid payment name");

                    RuleFor(c => c.Payment.Type)
                        .NotEmpty()
                        .WithMessage("Invalid payment type");
                });
        }
    }

    public class ChangePaymentCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<ChangePaymentCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(ChangePaymentCommand command, CancellationToken cancellationToken)
        {
            var changed = command.Ticket.ChangePayment(command.Payment);

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
