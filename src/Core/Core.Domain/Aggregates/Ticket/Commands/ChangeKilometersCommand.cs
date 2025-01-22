using Optimus.Core.Domain.Aggregates.Ticket.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands
{
    /// <summary>
    /// Updates the ticket kilometers
    /// </summary>
    /// <param name="TicketId">Ticket id to be updated</param>
    /// <param name="CPF">Driver's CPF </param>
    public record ChangeKilometersCommand(string TicketId, int KMs) : ICommand<TicketUpdated>
    {
        [JsonIgnore]
        public TicketAgg Ticket { get; set; }
    }
}
