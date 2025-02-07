using Tilray.Integrations.Services.Rootstock.Service.MappingProfiles;

namespace Tilray.Integrations.Services.Rootstock.Startup;

/// <summary>
/// This classe is responsible for registering the RootstockService in the DI container.
/// </summary>
public class RootstockStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("RootstockAPI").Get<RootstockSettings>();
        services.AddSingleton(settings ?? new RootstockSettings());

        services.AddTransient<RootstockAuthHandler>();
        services.AddHttpClient<RootstockAuthHandler>();

        services.AddHttpClient<IRootstockService, RootstockService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<RootstockSettings>();
            client.BaseAddress = new Uri(config.BaseUrl);
        }).AddHttpMessageHandler<RootstockAuthHandler>();

        services.AddAutoMapper(typeof(RootstockSalesOrderMapper));

        return services;
    }
}
