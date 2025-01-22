using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Product.Events
{
    public record ProductCreated : IDomainEvent
    {
        private ProductCreated(ProductAgg product)
        {
            Id = product.Id;
            Product = product;
            Name = product.Name;
        }
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        [JsonIgnore]
        public ProductAgg Product { get; internal set; }

        public static ProductCreated Create(ProductAgg product)
        {
            return new ProductCreated(product);
        }
    }

    public record ProductUpdated : IDomainEvent
    {
        private ProductUpdated(ProductAgg product)
        {
            Id = product.Id;
            Product = product;
            Name = product.Name;
        }

        public string Id { get; internal set; }
        public string Name { get; internal set; }

        [JsonIgnore]
        public ProductAgg Product { get; internal set; }

        public static ProductUpdated Create(ProductAgg product)
        {
            return new ProductUpdated(product);
        }
    }
}
