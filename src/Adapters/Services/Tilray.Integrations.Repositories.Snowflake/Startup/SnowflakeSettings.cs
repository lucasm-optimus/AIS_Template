namespace Tilray.Integrations.Repositories.Snowflake.Startup;

/// <summary>
/// This class is responsible for reading the Snowflake settings from the configuration file.
/// </summary>
public class SnowflakeSettings
{
    public string Account { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Warehouse { get; set; }
    public string Database { get; set; }
    public string Schema { get; set; }
    public string Role { get; set; }
    public string EDBLSourceId { get; set; }
}
