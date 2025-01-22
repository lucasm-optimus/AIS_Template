using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Application.Pump.State;
using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Ticket;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class CreateTicketForAttendantCommandValidator : AbstractValidator<CreateTicketForAttendantCommand>
    {
        public CreateTicketForAttendantCommandValidator(IPumpState pumpState, IAttendantState attendantState)
        {
            RuleFor(command => command.CardId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    instance.Attendant = await attendantState.Get(x => x.CardId == propValue);
                    return instance.Attendant != null;
                });
        }
    }

    public class CreateTicketForAttendantCommandHandler(ITicketState ticketState, IMediator mediator, IPumpState pumpState) : ICommandHandler<CreateTicketForAttendantCommand, TicketCreated>
    {
        public async Task<Result<TicketCreated>> Handle(CreateTicketForAttendantCommand command, CancellationToken cancellationToken)
        {
            command.Pump = await pumpState.Get(x => x.Number == command.PumpNumber && x.Nozzles.Any(n => n.Number == command.NozzleNumber));
            command.Nozzle = command?.Pump?.Nozzles?.FirstOrDefault(x => x.Number == command.NozzleNumber);

            if (command?.Pump == null || command?.Pump?.Nozzles == null)
            {
                var nozzle = await mediator.Send(CreateNozzleCommand.Create(1, command.NozzleNumber));
                command.Pump = await pumpState.Get(x => x.Number == command.PumpNumber);
                command.Nozzle = command.Pump.Nozzles.FirstOrDefault(x => x.Number == command.NozzleNumber);

                if (command.Pump == null || command.Pump.Nozzles == null)
                    return Result.Fail<TicketCreated>("Error creating Pump/Nozzle");
            }

            var ticket = await ticketState.GetOpenedTicket(command.CardId, command.PumpNumber, command.NozzleNumber);

            if (ticket.IsSuccess)
                return ticket.Value.Created();

            if (ticket.IsFailed)
                ticket = TicketAgg.Create(command, Domain.Aggregates.Ticket.Attendant.Create(command.Attendant!));

            if (ticket.IsSuccess)
            {
                var saved = await ticketState.Add(ticket.Value);
                if (saved.IsSuccess)
                {
                    await mediator.Publish(ticket.Value.Created());
                    return ticket.Value.Created();
                }
            }

            return Result.Fail<TicketCreated>("Ticket creation failed").WithErrors(ticket.Errors);
        }
    }
}
