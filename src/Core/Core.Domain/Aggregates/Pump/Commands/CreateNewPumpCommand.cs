using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Domain.Aggregates.Pump.Commands;

public record CreateNewPumpCommand : ICommand<PumpCreated>
{
    public int Number { get; init; }
    public string Description { get; init; }
}
