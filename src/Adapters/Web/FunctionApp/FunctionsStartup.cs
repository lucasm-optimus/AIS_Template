using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Core.Domain.Aggregates.Invoices;

namespace Tilray.Integrations.Functions
{
    public class FunctionsStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection("OrderDefaults").Get<OrderDefaultsSettings>() is OrderDefaultsSettings orderDefaults)
            {
                services.AddSingleton(orderDefaults);
            }

            if (configuration.GetSection("GLAccounts").Get<GLAccountsSettings>() is GLAccountsSettings glAccounts)
            {
                services.AddSingleton(glAccounts);
            }

            return services;
        }
    }
}
