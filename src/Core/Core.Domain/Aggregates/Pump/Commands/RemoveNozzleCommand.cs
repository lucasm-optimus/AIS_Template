using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Domain.Aggregates.Pump.Commands
{
    public record RemoveNozzleCommand(int PumpNumber, int NozzleNumber) : ICommand<NozzleRemoved>
    {
    }
}
