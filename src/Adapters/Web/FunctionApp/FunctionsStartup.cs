using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Functions
{
    public class FunctionsStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("OrderDefaults").Get<OrderDefaultsSettings>());
            services.AddSingleton(options => { return new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString")); });

            return services;
        }
    }
}
