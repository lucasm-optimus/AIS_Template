using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product.Commands;
using Optimus.Core.Domain.Aggregates.Product.Events;

namespace Optimus.Core.Application.Product.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class DisableProductCommandValidator : AbstractValidator<DisableProductCommand>
    {
        public DisableProductCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty();
        }
    }

    public class DisableProductCommandHandler(IProductState productState) : ICommandHandler<DisableProductCommand, ProductUpdated>
    {
        public async Task<Result<ProductUpdated>> Handle(DisableProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productState.Get(request.Id);

            if (product == null)
                return Result.Fail("Product not found").WithValidationError("Id", $"ProductId {request.Id} not found");

            Result<ProductUpdated> changed = product.Disable();

            if (changed.IsSuccess)
            {
                var updated = await productState.Update(request.Id, changed.Value.Product);
                if (updated.IsSuccess)
                    return changed;
            }

            return changed;
        }
    }
}
