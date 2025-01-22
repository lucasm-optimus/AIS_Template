
namespace Optimus.Core.Domain.Aggregates.Pump.Events
{
    public class PumpCreated : PumpAgg, IDomainEvent
    {
        public PumpCreated(PumpAgg pumpAgg)
        {
            this.Number = pumpAgg.Number;
            this.Description = pumpAgg.Description;
            this.Id = pumpAgg.Id;
            this.Nozzles = pumpAgg.Nozzles;
        }
    }
}
