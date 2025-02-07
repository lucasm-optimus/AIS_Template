namespace Tilray.Integrations.Repositories.Snowflake.Startup;

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
