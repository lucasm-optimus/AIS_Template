using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

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

            return services;
        }
    }
}
