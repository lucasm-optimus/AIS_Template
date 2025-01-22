using Optimus.Core.Common;
using Optimus.Services.Vehicles.APIPlacas.Adapters.States;
using Optimus.Services.Vehicles.APIPlacas.Service.Model;


namespace Optimus.Services.Vehicles.APIPlacas.EventHandlers
{
    public class VehiclePlateDownloadedHandler(IVehiclePlateState state) : IDomainEventHandler<VehiclePlateDownloaded>
    {
        public async Task Handle(VehiclePlateDownloaded notification, CancellationToken cancellationToken)
        {
            await state.AddOrUpdate(notification.placa, notification);
        }
    }
}
