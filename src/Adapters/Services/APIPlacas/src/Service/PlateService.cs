using Optimus.Services.Vehicles.APIPlacas.Adapters.States;
using Optimus.Services.Vehicles.APIPlacas.Service.Model;
using Optimus.Services.Vehicles.APIPlacas.Startup;
using Optimus.Core.Application.Vehicle.Adapters.Services;
using Optimus.Core.Domain.Aggregates.Vehicle;
using FluentResults;
using MediatR;
using System.Text.Json;

namespace Optimus.Services.Vehicles.APIPlacas.Service
{
    /// <summary>
    /// This class is the implementation of the IStream to connect to the Azure Service Bus.
    /// </summary>
    public class PlateService(IMediator mediator, IVehiclePlateState state, HttpClient client, APIPlacasSettings settings, ILogger logger) : IPlateService
    {
        public async Task<Result<VehicleAgg>> GetPlate(string plate)
        {
            plate = plate.Replace("-", string.Empty).Trim().ToUpper();

            var vehiclePlate = await state.Get(x => x.placa == plate);

            if (vehiclePlate == null)
            {
                var download = await DownloadFromApi(plate);
                if (download.IsSuccess)
                    vehiclePlate = download?.Value;
            }

            if (vehiclePlate == null)
                return Result.Fail("Plate not found in database and service");

            return VehicleAgg.Create(
                vehiclePlate.placa,
                vehiclePlate.placa_alternativa,
                vehiclePlate.MARCA,
                vehiclePlate.MODELO,
                vehiclePlate.SUBMODELO,
                vehiclePlate.segmento,
                vehiclePlate.extra?.tipo_veiculo,
                Int32.Parse(vehiclePlate.ano),
                vehiclePlate.extra?.municipio,
                vehiclePlate.extra?.uf,
                vehiclePlate.cor,
                vehiclePlate.extra?.combustivel,
                vehiclePlate.situacao,
                vehiclePlate.logo
            );
        }

        private async Task<Result<VehiclePlateDownloaded>> DownloadFromApi(string plate)
        {
            try
            {
                var response = await client.GetAsync($"consulta/{plate}/{settings.Token}");

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Fail("API Placas failed to respond").WithError(await response.Content.ReadAsStringAsync());
                }

                var result = JsonSerializer.Deserialize<VehiclePlateDownloaded>(await response.Content.ReadAsStringAsync());

                if (result is not null)
                {
                    result._id = plate;
                    result.date = DateTime.Now;

                    await mediator.Publish(result);

                    return Result.Ok(result);
                }

                return Result.Fail("Failure downloading plate data");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading plate");
                return Result.Fail("Failure downloading plate data").WithError(ex.Message);
            }
        }
    }
}
