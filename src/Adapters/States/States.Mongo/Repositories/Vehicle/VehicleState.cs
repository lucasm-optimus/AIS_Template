using Optimus.Core.Application.Vehicle.Adapters.States;
using Optimus.Core.Domain.Aggregates.Vehicle;
using Optimus.States.Mongo;

namespace States.Mongo.Repositories.Vehicle
{
    public class VehicleState : MongoRepositoryBase<VehicleAgg>, IVehicleState
    {
        public VehicleState(IMongoDatabase database) : base(database, "Vehicles")
        {

        }
    }
}
