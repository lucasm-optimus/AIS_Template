namespace Tilray.Integrations.Services.SAPConcur.Startup;

/// <summary>
/// This classe is responsible for registering the SAPConcurService in the DI container.
/// </summary>
public class SAPConcurStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("SAPConcurAPI").Get<SAPConcurSettings>();
        services.AddSingleton(settings ?? new SAPConcurSettings());

        services.AddTransient<SAPConcurAuthHandler>();
        services.AddHttpClient<SAPConcurAuthHandler>();

        services.AddHttpClient<ISAPConcurService, SAPConcurService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<SAPConcurSettings>();
            client.BaseAddress = new Uri(config.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<SAPConcurAuthHandler>();

        return services;
    }
}
