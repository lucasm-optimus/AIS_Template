namespace Tilray.Integrations.Repositories.Snowflake.Startup;

public class SnowflakeStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var snowflakeSettings = configuration.GetSection("Snowflake").Get<SnowflakeSettings>();
        if (snowflakeSettings != null &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Account) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Username) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Password) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Warehouse) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Database) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Schema) &&
            !string.IsNullOrWhiteSpace(snowflakeSettings.Role))
        {
            var connectionString = $"account={snowflakeSettings.Account};user={snowflakeSettings.Username};password={snowflakeSettings.Password};warehouse={snowflakeSettings.Warehouse};db={snowflakeSettings.Database};schema={snowflakeSettings.Schema};role={snowflakeSettings.Role};";
            services.AddScoped<ISnowflakeRepository>(provider => new SnowflakeRepository(connectionString, snowflakeSettings));
        }

        return services;
    }
}
