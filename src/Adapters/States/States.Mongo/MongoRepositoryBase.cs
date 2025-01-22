using MongoDB.Bson;
using System.Linq.Expressions;

namespace Optimus.States.Mongo;

public abstract class MongoRepositoryBase<T> : IState<T> where T : class
{
    protected readonly IMongoCollection<T> db;

    protected MongoRepositoryBase(IMongoDatabase database)
    {
        db = database.GetCollection<T>(typeof(T).Name);
    }

    protected MongoRepositoryBase(IMongoDatabase database, string collectionName)
    {
        db = database.GetCollection<T>(collectionName);
    }

    public async Task<Result<T>> Add(T entity)
    {
        try
        {
            await db.InsertOneAsync(entity);
            return Result.Ok(entity);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message).WithError(e.Message);
        }
    }

    public async Task<Result<T>> AddOrUpdate(string id, T entity)
    {
        try
        {
            var result = await db.ReplaceOneAsync(
                filter: new BsonDocument("_id", id),
                options: new ReplaceOptions { IsUpsert = true },
                replacement: entity);

            if (result.ModifiedCount == 0)
            {
                return Result.Fail("Entity not updated");
            }

            return Result.Ok(entity);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message).WithError(e.Message);
        }
    }

    public async Task<Result> Delete(string id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var deleteResult = await db.DeleteOneAsync(filter);

            if (deleteResult.DeletedCount > 0)
                return Result.Ok();
            else
                return Result.Fail("Delete failed");
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<T> Get(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await db.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> Get(Expression<Func<T, bool>> filter)
    {
        return await db.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> filter)
    {
        return await db.Find(filter).ToListAsync();
    }

    public async Task<Result> Update(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);

        var updateResult = await db.ReplaceOneAsync(filter, entity);

        if (updateResult.MatchedCount > 0)
            return Result.Ok();
        else
            return Result.Fail("Update failed");
    }

    public async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, Expression<Func<T, object>>? sortBy = null, bool ascending = true)
    {
        var find = db.Find(filter);

        if (sortBy is not null)
        {
            var sort = ascending ? Builders<T>.Sort.Ascending(sortBy) : Builders<T>.Sort.Descending(sortBy);
            find.Sort(sort);
        }

        return await find
            .Skip(((pageNumber <= 0 ? 1 : pageNumber) - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
}
