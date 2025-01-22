using System.Linq.Expressions;

namespace Optimus.Core.Common.States;

public interface IState<T>
{
    Task<T> Get(string id);
    Task<T> Get(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> filter);

    Task<Result<T>> Add(T entity);
    Task<Result<T>> AddOrUpdate(string id, T entity);
    Task<Result> Update(string id, T entity);
    Task<Result> Delete(string id);
    Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, Expression<Func<T, object>>? sortBy = null, bool ascending = true);
}
