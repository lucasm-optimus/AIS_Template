using Optimus.Core.Domain.Aggregates.Vehicle;

namespace Optimus.Core.Application.Vehicle.Adapters.States
{
    public interface IVehicleState : IState<VehicleAgg>
    {
        ///// <summary>
        ///// Get one vehicle with the specified Id
        ///// </summary>
        ///// <param name="id">The vehicle Id</param>
        ///// <returns>An instance of a VehicleAgg hydrated</returns>
        //Result<VehicleAgg> GetVehicle(string id);

        ///// <summary>
        ///// Add a new vehicle to the state
        ///// </summary>
        ///// <param name="vehicle">The vehicle to be added</param>
        ///// <returns>The same instance of the recently added item</returns>
        //Task<Result<VehicleAgg>> AddAsync(VehicleAgg vehicle);

        ///// <summary>
        ///// Get one vehicle for the specified Attendant
        ///// </summary>
        ///// <param name="id">The vehicle Id</param>
        ///// <param name="attendantId">The attendant id</param>
        ///// <returns>An instance of a VehicleAgg hydrated</returns>
        //Result<VehicleAgg> GetVehicle(string id, string attendantId);

        ///// <summary>
        ///// Get a list of vehicles based on the query parameter
        ///// </summary>
        ///// <param name="queryMany">Query object with parameters</param>
        ///// <returns>A list of VehicleAgg hydrated</returns>
        //Result<IEnumerable<VehicleAgg>> GetVehicles(QueryManyBase queryMany);
    }
}
