using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Application.Startup
{
    public class ApplicationStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var orderDefaults = configuration.GetSection("OrderDefaults").Get<OrderDefaultsSettings>();
            services.AddSingleton(orderDefaults ?? new OrderDefaultsSettings());

            return services;
        }
    }
}
