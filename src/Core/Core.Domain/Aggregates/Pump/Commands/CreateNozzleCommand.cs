using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Domain.Aggregates.Pump.Commands
{
    public record CreateNozzleCommand() : ICommand<NozzleCreated>
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int PumpNumber { get; internal set; }

        public void ChangePumpNumber(int pumpNumber)
        {
            PumpNumber = pumpNumber;
        }

        public static CreateNozzleCommand Create(int nozzleNumber)
        {
            return Create(1, nozzleNumber);
        }

        public static CreateNozzleCommand Create(int pumpNumber, int nozzleNumber)
        {
            return new CreateNozzleCommand() { Number = nozzleNumber, PumpNumber = pumpNumber, Color = "Black", Description = "Nozzle created by system" };
        }
    }
}
