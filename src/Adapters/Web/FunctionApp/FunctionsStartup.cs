using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Common.Stream;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Stream.Bus.Services;

namespace Tilray.Integrations.Functions
{
    public class FunctionsStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var orderDefaults = configuration.GetSection("OrderDefaults").Get<OrderDefaultsSettings>();
            if (orderDefaults != null)
            {
                services.AddSingleton(orderDefaults);
            }

            services.AddSingleton(sp =>
            {
                return sp.GetKeyedService<IStream>(nameof(AzureServiceBusService))!;
            });

            return services;
        }
    }
}
