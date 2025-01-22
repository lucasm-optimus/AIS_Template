using Optimus.Core.Domain.Aggregates.Ticket.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands;

/// <summary>
/// Updates the drive details in the ticket
/// </summary>
/// <param name="TicketId">Ticket id to be updated</param>
/// <param name="CPF">Driver's CPF </param>
public record ChangeTicketDriverCommand(string TicketId, string CPF) : ICommand<TicketUpdated>
{
    [JsonIgnore]
    public TicketAgg Ticket { get; set; }
}
