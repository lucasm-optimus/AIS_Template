using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump;

namespace Optimus.States.Mongo.Repositories.Pump
{
    public class PumpRepository : MongoRepositoryBase<PumpAgg>, IPumpState
    {
        public PumpRepository(IMongoDatabase database) : base(database, "Pumps")
        {

        }
    }
}
