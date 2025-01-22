using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product;

namespace Optimus.Core.Application.Product.Queries;

public record ProductGetOne(string Id) : IQuery<ProductAgg>;

public class ProductGetOneQueryHandler(IProductState state) : IQueryHandler<ProductGetOne, ProductAgg>
{
    public async Task<Result<ProductAgg>> Handle(ProductGetOne request, CancellationToken cancellationToken)
    {
        //Validate if user can retrieve the desired information
        //Check if the information can be returned to the user...

        return await state.Get(request.Id);
    }
}