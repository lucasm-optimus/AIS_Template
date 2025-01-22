using Optimus.Core.Domain.Aggregates.Attendant;
using Optimus.Core.Domain.Aggregates.Pump;
using Optimus.Core.Domain.Aggregates.Ticket.Events;
using Optimus.Core.Domain.Aggregates.Vehicle;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands;

/// <summary>
/// Definition of the Command to create a new ticket with the required properties
/// </summary>
/// <param name="cardId">The Attendant's CardId who is creating the ticket</param>
/// <param name="Plate">The plate number of the vehicle to be associated with the ticket</param>
public record CreateTicketCommand(string CardId, string Plate) : ICommand<TicketCreated>
{
    [JsonIgnore]
    public AttendantAgg? Attendant { get; set; }

    [JsonIgnore]
    public VehicleAgg? Vehicle { get; set; }
}

/// <summary>
/// Definition of the Command to create a new ticket with the required properties
/// </summary>
/// <param name="cardId">The Attendant's CardId who is creating the ticket</param>
/// <param name="Plate">The plate number of the vehicle to be associated with the ticket</param>
public record CreateTicketForAttendantCommand(string CardId, int PumpNumber, int NozzleNumber) : ICommand<TicketCreated>
{
    [JsonIgnore]
    public PumpAgg? Pump { get; set; }

    [JsonIgnore]
    public Nozzle? Nozzle { get; set; }

    [JsonIgnore]
    public AttendantAgg? Attendant { get; set; }
}


/// <summary>
/// Definition of the Command to create a new ticket with the required properties
/// </summary>
/// <param name="cardId">The Attendant's CardId who is creating the ticket</param>
/// <param name="Plate">The plate number of the vehicle to be associated with the ticket</param>
public record AuthorizeRefuelingForTicketCommand(Guid TicketId, int PumpNumber, int NozzleNumber) : ICommand<TicketUpdated>
{
    [JsonIgnore]
    public PumpAgg? Pump { get; set; }

    [JsonIgnore]
    public Nozzle? Nozzle { get; set; }

    [JsonIgnore]
    public TicketAgg? Ticket { get; set; }
}