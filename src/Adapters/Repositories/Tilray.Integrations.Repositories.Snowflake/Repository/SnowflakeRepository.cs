namespace Tilray.Integrations.Repositories.Snowflake.Repository;

public class SnowflakeRepository(string connectionString, SnowflakeSettings snowflakeSettings) : RepositoryBase(connectionString), ISnowflakeRepository
{
    public async Task<string> GetAcctCodeAsync(string segment0, string segment1)
    {
        var parameters = new[]
        {
            new SnowflakeDbParameter { ParameterName = "Segment_0", Value = segment0 },
            new SnowflakeDbParameter { ParameterName = "Segment_1", Value = segment1 },
            new SnowflakeDbParameter { ParameterName = "EDBLSourceID", Value = snowflakeSettings.EDBLSourceId }
        };

        return await ExecuteScalarAsync<string>(SnowflakeQueries.GetAcctCode, parameters);
    }

    public async Task<IEnumerable<GrpoDetails>> GetGrpoDetailsAsync(
        string baseDocNum, string baseEntry, string docEntry, string lineNum)
    {
        var parameters = new[]
        {
                new SnowflakeDbParameter { ParameterName = "BaseDocNum", Value = baseDocNum },
                new SnowflakeDbParameter { ParameterName = "BaseEntry", Value = baseEntry },
                new SnowflakeDbParameter { ParameterName = "DocEntry", Value = docEntry },
                new SnowflakeDbParameter { ParameterName = "LineNum", Value = lineNum },
                new SnowflakeDbParameter { ParameterName = "EDBLSourceID", Value = snowflakeSettings.EDBLSourceId }
        };

        return await ExecuteQueryAsync(
            SnowflakeQueries.GetGrpoDetails,
            reader => new GrpoDetails
            {
                OpenQty = reader.GetDecimal(reader.GetOrdinal("OpenQty")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
            },
            parameters
        );
    }
}
