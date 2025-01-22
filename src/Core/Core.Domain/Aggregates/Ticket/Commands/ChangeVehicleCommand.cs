using Optimus.Core.Domain.Aggregates.Ticket.Events;
using Optimus.Core.Domain.Aggregates.Vehicle;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands;

/// <summary>
/// Updates the vehicle details in the ticket
/// </summary>
/// <param name="plate">The vehicle plate to be updated</param>
public record ChangeVehicleCommand(string TicketId, string Plate) : ICommand<TicketUpdated>
{
    [JsonIgnore]
    public TicketAgg Ticket { get; set; }
    [JsonIgnore]
    public VehicleAgg Vehicle { get; set; }
}
