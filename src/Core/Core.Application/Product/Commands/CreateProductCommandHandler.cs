using Optimus.Core.Application.Product.Adapters;
using Optimus.Core.Domain.Aggregates.Product;
using Optimus.Core.Domain.Aggregates.Product.Commands;
using Optimus.Core.Domain.Aggregates.Product.Events;

namespace Optimus.Core.Application.Product.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class ProductAddCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public ProductAddCommandValidator()
        {
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

    public class CreateProductCommandHandler(IProductState productState) : ICommandHandler<CreateProductCommand, ProductCreated>
    {
        public async Task<Result<ProductCreated>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var result = ProductAgg.Create(request);
            var fail = Result.Fail<ProductCreated>("Product creation failed");

            if (result.IsSuccess)
            {
                var saveResult = await productState.Add(result.Value.Product);
                if (saveResult?.Value != null)
                {
                    var productCreated = saveResult?.Value?.Created() ?? fail;
                    return productCreated;
                }
            }

            return result;
        }
    }
}
