using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Services.Salesforce.Service;

namespace Tilray.Integrations.Services.Salesforce.Satrtup
{
    public class SalesforceStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("SalesforceSettings").Get<SalesforceSettings>();
            services.AddSingleton(settings ?? new SalesforceSettings());
            
            services.AddTransient<SalesforceAuthHandler>();
            services.AddHttpClient<SalesforceAuthHandler>();
            services.AddScoped<ISalesforceService, SalesforceService>();

            services.AddHttpClient<ISalesforceService, SalesforceService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<SalesforceSettings>();
                
                client.BaseAddress = new Uri(config.BaseUrl);
            }).AddHttpMessageHandler<SalesforceAuthHandler>();


            return services;
        }

    }
}
