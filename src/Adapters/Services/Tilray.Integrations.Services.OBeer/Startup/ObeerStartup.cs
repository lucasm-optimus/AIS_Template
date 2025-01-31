namespace Tilray.Integrations.Services.OBeer.Startup;

/// <summary>
/// This classe is responsible for registering the ObeerService in the DI container.
/// </summary>
public class ObeerStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("ObeerAPI").Get<ObeerSettings>();
        services.AddSingleton(settings ?? new ObeerSettings());

        services.AddHttpClient<IObeerService, ObeerService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<ObeerSettings>();
            client.BaseAddress = new Uri(config.BaseUrl);
        });

        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
