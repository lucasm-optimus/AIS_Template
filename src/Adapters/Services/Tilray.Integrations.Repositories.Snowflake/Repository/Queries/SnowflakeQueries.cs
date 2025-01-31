namespace Tilray.Integrations.Repositories.Snowflake.Repository.Queries;

internal static class SnowflakeQueries
{
    internal const string GetAcctCode = @"SELECT ""AcctCode"" FROM ""OACT"" WHERE ""Segment_0"" = :Segment_0 AND ""Segment_1"" = :Segment_1 AND ""EDBLSourceID"" = :EDBLSourceID";

    internal const string GetGrpoDetails = @"
        SELECT ""OpenQty"", ""Price"", ""LineTotal""
        FROM ""PDN1""
        WHERE ""BaseDocNum"" = :BaseDocNum
            AND ""BaseEntry"" = :BaseEntry
            AND ""DocEntry"" = :DocEntry
            AND ""LineNum"" = :LineNum
            AND ""EDBLSourceID"" = :EDBLSourceID";
}
