using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;


namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ChangeProductQuantityOnTicketCommandValidator : AbstractValidator<ChangeProductQuantityOnTicketCommand>
    {
        public ChangeProductQuantityOnTicketCommandValidator(IProductState productState, ITicketState ticketState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Ticket = await ticketState.Get(propValue);
                    return instance.Ticket != null;
                }).DependentRules(() =>
                {
                    RuleFor(command => command.ProductId)
                        .NotEmpty()
                        .Must((instance, property, token) =>
                        {
                            var exists = instance?.Ticket?.Products?.Any(x => x.Id == property);
                            return exists == true;
                        })
                        .WithMessage("Ticket does not contains this product");
                });

            RuleFor(command => command.ProductId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Product = await productState.Get(propValue);
                    return instance.Product != null;
                });

            RuleFor(command => command.Quantity)
                .GreaterThan(0);
        }
    }

    public class ChangeProductQuantityOnTicketCommandHandler(ITicketState ticketState, IMediator mediator) : ICommandHandler<ChangeProductQuantityOnTicketCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(ChangeProductQuantityOnTicketCommand command, CancellationToken cancellationToken)
        {
            var changeResult = command.Ticket!.ChangeProduct(command.Product!, command.Quantity);

            if (changeResult.IsSuccess)
            {
                var update = await ticketState.Update(command.TicketId, command.Ticket);
                if (update.IsSuccess)
                    await mediator.Publish(changeResult.Value, cancellationToken);

                changeResult.WithErrors(update.Errors);
            }

            return changeResult;
        }
    }
}
