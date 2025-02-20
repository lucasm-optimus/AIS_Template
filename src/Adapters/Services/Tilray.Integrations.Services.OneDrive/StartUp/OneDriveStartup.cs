using Tilray.Integrations.Core.Common.Startup;

namespace Tilray.Integrations.Services.OneDrive.Startup
{
    /// <summary>
    /// This class is responsible for registering the OneDriveService in the DI container.
    /// </summary>
    public class OneDriveStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("OneDriveSettings").Get<OneDriveSettings>();
            services.AddSingleton(settings ?? new OneDriveSettings());

            services.AddTransient<OneDriveAuthHandler>();
            services.AddHttpClient<OneDriveAuthHandler>();

            services.AddHttpClient<IOneDriveService, OneDriveService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<OneDriveSettings>();
                client.BaseAddress = new Uri($"https://{config.OneDriveHost}");
            })

            .AddHttpMessageHandler<OneDriveAuthHandler>();

            return services;
        }
    }
}