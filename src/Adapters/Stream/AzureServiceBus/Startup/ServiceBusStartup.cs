using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Stream.Bus.Services;

namespace Tilray.Integrations.Stream.Bus.Startup;

/// <summary>
/// This class is responsible for reading the ServiceBus settings from the configuration file.
/// </summary>
public class ServiceBusStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConnectionString = configuration["ServiceBusConnectionString"];
        if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
        {
            services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
        }

        services.AddKeyedSingleton<IStream, AzureServiceBusService>(nameof(AzureServiceBusService));

        return services;
    }
}
