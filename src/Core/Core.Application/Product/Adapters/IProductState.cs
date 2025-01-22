using Optimus.Core.Domain.Aggregates.Product;

namespace Optimus.Core.Application.Product.Adapters
{
    public interface IProductState : IState<ProductAgg>;
}
