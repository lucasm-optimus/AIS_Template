namespace Tilray.Integrations.Core.Common.States;

public interface IState
{
    Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, Func<System.Data.IDataReader, T> map, params System.Data.Common.DbParameter[] parameters);
    Task<T> ExecuteScalarAsync<T>(string query, params System.Data.Common.DbParameter[] parameters);
}
