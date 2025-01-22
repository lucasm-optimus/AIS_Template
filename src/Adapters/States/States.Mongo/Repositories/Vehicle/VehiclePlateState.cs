using Optimus.Services.Vehicles.APIPlacas.Adapters.States;
using Optimus.Services.Vehicles.APIPlacas.Service.Model;

namespace Optimus.States.Mongo.Repositories.Vehicle
{
    public class VehiclePlateState : MongoRepositoryBase<VehiclePlateDownloaded>, IVehiclePlateState
    {
        public VehiclePlateState(IMongoDatabase database) : base(database, "Plates")
        {

        }
    }
}
