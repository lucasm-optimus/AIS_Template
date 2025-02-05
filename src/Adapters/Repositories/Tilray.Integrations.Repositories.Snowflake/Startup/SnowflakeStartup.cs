using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.Adapters.Repositories;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Repositories.Snowflake.Repository;

namespace Tilray.Integrations.Repositories.Snowflake.Startup
{
    public class SnowflakeStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            var snowflakeSettings = configuration.GetSection("Snowflake").Get<SnowflakeSettings>();
            var connectionString = $"account={snowflakeSettings.Account};user={snowflakeSettings.Username};password={snowflakeSettings.Password};warehouse={snowflakeSettings.Warehouse};db={snowflakeSettings.Database};schema={snowflakeSettings.Schema};role={snowflakeSettings.Role};";

            services.AddScoped<ISnowflakeRepository>(provider => new SnowflakeRepository(connectionString, snowflakeSettings));

            return services;
        }
    }
}
