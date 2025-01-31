namespace Tilray.Integrations.Repositories.Snowflake;

/// <summary>
/// Provides base functionality for executing queries against Snowflake.
/// </summary>
public class RepositoryBase(string connectionString) : IState
{
    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, Func<IDataReader, T> map, params DbParameter[] parameters)
    {
        var results = new List<T>();
        using var connection = new SnowflakeDbConnection { ConnectionString = connectionString };
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddRange(parameters);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    public async Task<T> ExecuteScalarAsync<T>(string query, params DbParameter[] parameters)
    {
        using var connection = new SnowflakeDbConnection { ConnectionString = connectionString };
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddRange(parameters);

        var result = await command.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value)
            return default;
        if (result is T t)
            return t;
        else
            return (T)Convert.ChangeType(result, typeof(T));
    }
}
