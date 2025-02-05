using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optimus.Core.Common.Startup;
using Tilray.Integrations.Core.Application.Rootstock.Stream;
using Tilray.Integrations.Stream.Rootstock.ServiceBus;

namespace Tilray.Integrations.Stream.Rootstock.Startup
{
    /// <summary>
    /// This classe is responsible for registering the RootstockService in the DI container.
    /// </summary>
    public class RootstockServiceBusStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("RootstockServiceBus").Get<RootstockServiceBusSettings>();
            services.AddSingleton(settings ?? new RootstockServiceBusSettings());

            services.AddSingleton<IRootstockStream, RootstockStream>();

            return services;
        }
    }
}
