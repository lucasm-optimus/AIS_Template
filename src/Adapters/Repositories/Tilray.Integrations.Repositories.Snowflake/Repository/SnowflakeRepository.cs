using Snowflake.Data.Client;
using Tilray.Integrations.Core.Application.Adapters.Repositories;
using Tilray.Integrations.Repositories.Snowflake.Repository.Queries;
using Tilray.Integrations.Repositories.Snowflake.Startup;

namespace Tilray.Integrations.Repositories.Snowflake.Repository
{
    public class SnowflakeRepository : RepositoryBase, ISnowflakeRepository
    {
        private readonly SnowflakeSettings _snowflakeSettings;
        public SnowflakeRepository(string connectionString, SnowflakeSettings snowflakeSettings) : base(connectionString)
        {
            _snowflakeSettings = snowflakeSettings;
        }

        public async Task<string> GetAcctCodeAsync(string segment0, string segment1)
        {
            var parameters = new[]
            {
                new SnowflakeDbParameter { ParameterName = "Segment0", Value = segment0 },
                new SnowflakeDbParameter { ParameterName = "Segment1", Value = segment1 },
                new SnowflakeDbParameter { ParameterName = "EDBLSourceId", Value = _snowflakeSettings.EDBLSourceId }
            };

            return await ExecuteScalarAsync<string>(SnowflakeQueries.GetAcctCode, parameters);
        }

        public async Task<IEnumerable<(decimal OpenQty, decimal Price, decimal LineTotal)>> GetInvoiceDetailsAsync(
            string baseDocNum, string baseEntry, string docEntry, string lineNum)
        {
            var parameters = new[]
            {
                new SnowflakeDbParameter { ParameterName = "BaseDocNum", Value = baseDocNum },
                new SnowflakeDbParameter { ParameterName = "BaseEntry", Value = baseEntry },
                new SnowflakeDbParameter { ParameterName = "DocEntry", Value = docEntry },
                new SnowflakeDbParameter { ParameterName = "LineNum", Value = lineNum },
                new SnowflakeDbParameter { ParameterName = "EDBLSourceID", Value = _snowflakeSettings.EDBLSourceId }
            };

            return await ExecuteQueryAsync(
                SnowflakeQueries.GetInvoiceDetails,
                reader => (
                    OpenQty: reader.GetDecimal(reader.GetOrdinal("OpenQty")),
                    Price: reader.GetDecimal(reader.GetOrdinal("Price")),
                    LineTotal: reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                ),
                parameters
            );
        }
    }
}
