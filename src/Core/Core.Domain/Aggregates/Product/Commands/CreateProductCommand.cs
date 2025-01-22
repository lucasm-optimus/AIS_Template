using Optimus.Core.Domain.Aggregates.Product.Events;

namespace Optimus.Core.Domain.Aggregates.Product.Commands
{
    public record CreateProductCommand : ICommand<ProductCreated>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal MaxItemsPerCart { get; set; }
    }

    public record UpdateProductCommand : ICommand<ProductUpdated>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal MaxItemsPerCart { get; set; }
        public bool IsEnabled { get; set; }
    }

    public record DisableProductCommand(string Id) : ICommand<ProductUpdated> { }
    public record EnableProductCommand(string Id) : ICommand<ProductUpdated> { }

}
