using Optimus.Core.Domain.Aggregates.Ticket.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands
{
    /// <summary>
    /// Updates the ticket payment
    /// </summary>
    /// <param name="TicketId">Ticket id to be updated</param>
    /// <param name="Payment">The payment to be used by this ticket</param>
    public record ChangePaymentCommand(string TicketId, Payment Payment) : ICommand<TicketUpdated>
    {
        [JsonIgnore]
        public TicketAgg Ticket { get; set; }
    }
}
