using Optimus.Core.Domain.Aggregates.Pump;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket
{

    public class Pump : Entity
    {
        public int Number { get; private set; }

        public string Description { get; private set; }
        public Nozzle Nozzle { get; private set; }

        public static Pump Create(PumpAgg pump, Nozzle nozzle)
        {
            return new Pump()
            {
                Number = pump.Number,
                Description = pump.Description,
                Nozzle = nozzle
            };
        }
    }

    public class Supply : Entity
    {
        public DateTime StartedDate { get; private set; }
        public DateTime? FinishedDate { get; private set; }
        public DateTime? LastUpdatedDate { get; private set; }
        public SupplyStatus Status { get; private set; }
        public decimal Cost { get; private set; }
        public decimal Quantity { get; private set; }
        public Pump Pump{ get; private set; }

        public Result ChangeQuantityAndCost(decimal quantity, decimal cost)
        {
            Quantity = quantity;
            Cost = cost;
            Status = SupplyStatus.InProgress;
            return Result.Ok();
        }

        public Result IncreaseQuantity(decimal quantity)
        {
            Quantity += quantity;
            Status = SupplyStatus.InProgress;
            return Result.Ok();
        }

        public Result Finish()
        {
            Status = SupplyStatus.Finished;
            return Result.Ok();
        }

        internal static Supply Create(PumpAgg pump, Nozzle nozzle, decimal quantity, decimal cost)
        {
            return new Supply()
            {
                StartedDate = DateTime.Now,
                Status = SupplyStatus.NotStarted,
                Pump = Pump.Create(pump, nozzle),
                Quantity = quantity,
                Cost = cost
            };
        }
        internal static Supply Create(PumpAgg pump, Nozzle nozzle)
        {
            return new Supply()
            {
                StartedDate = DateTime.Now,
                Status = SupplyStatus.NotStarted,
                Pump = Pump.Create(pump, nozzle),
                Quantity = 0,
                Cost = 0
            };
        }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupplyStatus
    {
        NotStarted,
        Started,
        InProgress,
        Finished,
        Aborted
    }
}
