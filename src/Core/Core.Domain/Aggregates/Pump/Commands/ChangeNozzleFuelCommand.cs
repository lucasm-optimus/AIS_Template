using Optimus.Core.Domain.Aggregates.Pump.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Pump.Commands;

public record ChangeNozzleFuelCommand : ICommand<FuelChanged>
{
    [JsonIgnore]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
    public int PumpNumber { get; set; }
    public int NozzleId { get; set; }
}
