using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product;
using Optimus.States.Mongo;

namespace Optimus.States.Mongo.Repositories.Product;

public class ProductRepository : MongoRepositoryBase<ProductAgg>, IProductState
{
    public ProductRepository(IMongoDatabase database) : base(database, "Products")
    {

    }
}
