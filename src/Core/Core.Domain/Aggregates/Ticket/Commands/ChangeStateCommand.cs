using Optimus.Core.Domain.Aggregates.Ticket.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands
{
    public abstract class ChangeStateCommand : ICommand<TicketUpdated>
    {
        public Guid TicketId { get; set; }

        [JsonIgnore]
        public TicketAgg? Ticket { get; set; }
    }

    public class AbandonCommand : ChangeStateCommand
    {
        public AbandonCommand(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
    public class RequestPaymentCommand : ChangeStateCommand
    {
        public RequestPaymentCommand(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
    public class ReopenCommand : ChangeStateCommand
    {
        public ReopenCommand(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
    public class PayCommand : ChangeStateCommand
    {
        public PayCommand(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
    public class FinishCommand : ChangeStateCommand
    {
        public FinishCommand(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
}
