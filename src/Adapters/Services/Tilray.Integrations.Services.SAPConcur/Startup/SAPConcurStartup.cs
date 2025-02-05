using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.SAPConcur.Services;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Services.SAPConcur.Service;

namespace Tilray.Integrations.Services.SAPConcur.Startup
{
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
            }).AddHttpMessageHandler<SAPConcurAuthHandler>();

            return services;
        }
    }
}
