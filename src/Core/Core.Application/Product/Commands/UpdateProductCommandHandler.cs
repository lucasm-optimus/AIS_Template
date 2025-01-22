using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product.Commands;
using Optimus.Core.Domain.Aggregates.Product.Events;

namespace Optimus.Core.Application.Product.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty();

            RuleFor(command => command.Name)
                .NotEmpty();

            RuleFor(command => command.Description)
                .NotEmpty();

            RuleFor(command => command.Price)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(command => command.Quantity)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(command => command.Price)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(command => command.MaxItemsPerCart)
                .GreaterThanOrEqualTo(0);
        }
    }

    public class UpdateProductCommandHandler(IProductState productState) : ICommandHandler<UpdateProductCommand, ProductUpdated>
    {
        public async Task<Result<ProductUpdated>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productState.Get(request.Id);

            if (product == null)
                return Result.Fail("Product not found").WithValidationError("Id", $"ProductId {request.Id} not found");

            Result<ProductUpdated> changed = product.Change(request);

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
