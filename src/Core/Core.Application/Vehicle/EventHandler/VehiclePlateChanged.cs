//using Optimus.Services.Vehicles.APIPlacas.Service.Model;
//using Optimus.Core.Application.Vehicle.Adapters.States;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Optimus.Core.Application.Vehicle.EventHandler
//{
//    internal class VehiclePlateChanged(IVehicleState state) : IDomainEventHandler<ConsultaPlaca>
//    {
//        public async Task Handle(ConsultaPlaca notification, CancellationToken cancellationToken)
//        {
//            await state.AddOrUpdate(notification.placa, notification);
//        }
//    }
//}

//TODO: Implementar um médoto para salvar o resultado da consulta de placa
