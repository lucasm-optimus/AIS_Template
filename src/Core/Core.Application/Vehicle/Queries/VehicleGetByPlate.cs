using Optimus.Core.Application.Vehicle.Adapters.Services;
using Optimus.Core.Application.Vehicle.Adapters.States;
using Optimus.Core.Domain.Aggregates.Vehicle;

namespace Optimus.Core.Application.Vehicle.Queries
{
    public record VehicleGetByPlate(string Plate) : IQuery<VehicleAgg>;

    public class VehicleGetByPlateQueryHandler(IPlateService service) : IQueryHandler<VehicleGetByPlate, VehicleAgg>
    {
        public async Task<Result<VehicleAgg>> Handle(VehicleGetByPlate request, CancellationToken cancellationToken)
        {
            var plate = request.Plate.Replace("-", string.Empty).Trim().ToUpper();

            return await service.GetPlate(plate);
        }
    }
}
