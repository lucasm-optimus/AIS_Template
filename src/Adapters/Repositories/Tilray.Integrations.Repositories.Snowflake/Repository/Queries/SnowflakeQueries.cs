namespace Tilray.Integrations.Repositories.Snowflake.Repository.Queries
{
    internal static class SnowflakeQueries
    {
        internal const string GetAcctCode = @"
                SELECT AcctCode
                FROM OACT
                WHERE Segment_0 = :Segment0
                  AND Segment_1 = :Segment1
                  AND EDBLSourceID = :EDBLSourceId";

        internal const string GetInvoiceDetails = @"
                SELECT OpenQty, Price, LineTotal
                FROM PDN1
                WHERE BaseDocNum = @BaseDocNum
                  AND BaseEntry = @BaseEntry
                  AND DocEntry = @DocEntry
                  AND LineNum = @LineNum
                  AND EDBLSourceID = @EDBLSourceID";
    }
}
