using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class RemoveProductFromTicketCommandValidator : AbstractValidator<RemoveProductFromTicketCommand>
    {
        public RemoveProductFromTicketCommandValidator(ITicketState ticketState, IProductState productState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Ticket = await ticketState.Get(propValue);
                    return instance.Ticket != null;
                });

            RuleFor(command => command.ProductId)
                .NotEmpty()
                .MinimumLength(6)
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Product = await productState.Get(propValue);
                    return instance.Product != null;
                });
        }
    }

    public class RemoveProductFromTicketCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<RemoveProductFromTicketCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(RemoveProductFromTicketCommand command, CancellationToken cancellationToken)
        {
            var addResult = command.Ticket!.RemoveProduct(command);

            if (addResult.IsSuccess)
            {
                var update = await ticketState.Update(command.TicketId, command.Ticket);
                if (update.IsSuccess)
                    await mediator.Publish(addResult.Value, cancellationToken);

                addResult.WithErrors(update.Errors);
            }

            return addResult;
        }
    }
}
