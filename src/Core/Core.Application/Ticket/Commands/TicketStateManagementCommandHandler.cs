using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    public class ChangeTicketCommand : AbstractValidator<ChangeStateCommand>
    {
        public ChangeTicketCommand(ITicketState state)
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, value, c) =>
                {
                    instance.Ticket = await state.Get(x => x.Id == value.ToString());
                    return instance.Ticket is not null;
                });
        }
    }

    public class AbandonCommandValidator : AbstractValidator<AbandonCommand>
    {
        public AbandonCommandValidator(ITicketState state)
        {
            Include(new ChangeTicketCommand(state));
        }
    }
    public class RequestPaymentCommandValidator : AbstractValidator<RequestPaymentCommand>
    {
        public RequestPaymentCommandValidator(ITicketState state)
        {
            Include(new ChangeTicketCommand(state));
        }
    }
    public class ReopenCommandValidator : AbstractValidator<ReopenCommand>
    {
        public ReopenCommandValidator(ITicketState state)
        {
            Include(new ChangeTicketCommand(state));
        }
    }
    public class PayCommandValidator : AbstractValidator<PayCommand>
    {
        public PayCommandValidator(ITicketState state)
        {
            Include(new ChangeTicketCommand(state));
        }
    }
    public class FinishCommandValidator : AbstractValidator<FinishCommand>
    {
        public FinishCommandValidator(ITicketState state)
        {
            Include(new ChangeTicketCommand(state));
        }
    }



    public class TicketStateManagementCommandHandler(ITicketState state, IMediator mediator) :
        ICommandHandler<AbandonCommand, TicketUpdated>,
        ICommandHandler<RequestPaymentCommand, TicketUpdated>,
        ICommandHandler<ReopenCommand, TicketUpdated>,
        ICommandHandler<PayCommand, TicketUpdated>,
        ICommandHandler<FinishCommand, TicketUpdated>
    {
        public Task<Result<TicketUpdated>> Handle(FinishCommand request, CancellationToken cancellationToken)
        {
            var updated = request?.Ticket?.Finish();

            return ChangeState(updated);
        }

        public Task<Result<TicketUpdated>> Handle(PayCommand request, CancellationToken cancellationToken)
        {
            var updated = request?.Ticket?.Pay();

            return ChangeState(updated);
        }

        public Task<Result<TicketUpdated>> Handle(ReopenCommand request, CancellationToken cancellationToken)
        {
            var updated = request?.Ticket?.ReopenPayment();

            return ChangeState(updated);
        }

        public Task<Result<TicketUpdated>> Handle(RequestPaymentCommand request, CancellationToken cancellationToken)
        {
            var updated = request?.Ticket?.RequestPayment();

            return ChangeState(updated);
        }

        public Task<Result<TicketUpdated>> Handle(AbandonCommand request, CancellationToken cancellationToken)
        {
            var updated = request?.Ticket?.Abandon();

            return ChangeState(updated);
        }

        private async Task<Result<TicketUpdated>> ChangeState(Result<TicketUpdated>? updated)
        {
            if (updated == null)
                return Result.Fail<TicketUpdated>("Ticket cannot be changed");

            if (updated.IsFailed)
                return updated;

            var saved = await state.Update(updated.Value.Id, updated.Value.Ticket);
            if (saved.IsSuccess)
                await mediator.Publish(updated.Value);

            return updated.WithErrors(saved.Errors);
        }
    }
}
