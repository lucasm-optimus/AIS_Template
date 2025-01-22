using Optimus.Core.Common.Startup;
using Optimus.Services.Vehicles.APIPlacas.Service;
using Optimus.Core.Application.Vehicle.Adapters.Services;

namespace Optimus.Services.Vehicles.APIPlacas.Startup
{
    /// <summary>
    /// This classe is responsible for registering the ServiceBusStreamService in the DI container.
    /// </summary>
    public class StartupRegister : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("Services:APIPlacas").Get<APIPlacasSettings>();
            services.AddSingleton<APIPlacasSettings>(settings ?? new APIPlacasSettings());

            // Registre o HttpClient com Polly
            services.AddHttpClient<IPlateService, PlateService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<APIPlacasSettings>();

                client.BaseAddress = new Uri(config.ApiAddress);
                //client.DefaultRequestHeaders.Add("Authorization", $"{config.Token}");
                client.DefaultRequestHeaders.Host = client.BaseAddress.Host;
            });

            return services;
        }
    }
}
