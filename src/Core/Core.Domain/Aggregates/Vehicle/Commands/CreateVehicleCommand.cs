using Optimus.Core.Domain.Aggregates.Vehicle.Events;

namespace Optimus.Core.Domain.Aggregates.Vehicle.Commands
{
    public record CreateVehicleCommand(string plate, string brand, string model, string color, VehicleTypeEnum type, string ownerId) : ICommand<VehicleCreated>;
}
