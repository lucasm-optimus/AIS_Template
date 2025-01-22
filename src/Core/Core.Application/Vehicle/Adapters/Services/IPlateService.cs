using Optimus.Core.Domain.Aggregates.Vehicle;

namespace Optimus.Core.Application.Vehicle.Adapters.Services
{
    public interface IPlateService
    {
        Task<Result<VehicleAgg>> GetPlate(string plate);
    }
}
