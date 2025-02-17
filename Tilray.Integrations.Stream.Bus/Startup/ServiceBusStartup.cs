using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.Adapters.Stream;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Stream.Bus.Services;

namespace Tilray.Integrations.Services.Rootstock.Startup;

/// <summary>
/// This class is responsible for reading the ServiceBus settings from the configuration file.
/// </summary>
public class ServiceBusStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>();
        if (settings == null)
            throw new Exception("ServiceBus settings not found in configuration.");

        services.AddSingleton(new ServiceBusClient(settings.ConnectionString));
        services.AddSingleton<IStreamService, StreamService>();

        return services;
    }
}
