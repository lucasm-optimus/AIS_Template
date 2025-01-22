using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product;

namespace Optimus.Core.Application.Product.Queries;

public record ProductSearch(string query) : QueryManyBase<ProductAgg>;


public class ProductSearchQueryHandler(IProductState state) : IQueryManyHandler<ProductSearch, ProductAgg>
{
    public async Task<Result<IEnumerable<ProductAgg>>> Handle(ProductSearch request, CancellationToken cancellationToken)
    {
        var list = await state.GetMany(x => (x.Name.Contains(request.query) || x.Description.Contains(request.query)) && x.isEnabled == true);

        if (list.Any())
            return Result.Ok(list);

        return Result.Ok(new List<ProductAgg>().AsEnumerable());
    }
}