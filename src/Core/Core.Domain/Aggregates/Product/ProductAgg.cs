using Optimus.Core.Domain.Aggregates.Product.Commands;
using Optimus.Core.Domain.Aggregates.Product.Events;

namespace Optimus.Core.Domain.Aggregates.Product
{
    public class ProductAgg
    {
        public string Id { get; internal set; }
        public string Description { get; internal set; }
        public string Name { get; internal set; }
        public decimal Price { get; internal set; }
        public int Quantity { get; internal set; }
        public decimal MaxItemsPerCart { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
        public bool isEnabled { get; internal set; }

        /// <summary>
        /// Creates a new Aggregate with all business rules to be validated
        /// </summary>
        /// <param name="command"></param>
        private ProductAgg(CreateProductCommand command)
        {
            Id = Guid.NewGuid().ToString();
            Description = command.Description;
            Name = command.Name;
            Price = command.Price;
            Quantity = command.Quantity;
            MaxItemsPerCart = command.MaxItemsPerCart;
            CreatedDate = DateTime.Now;
            isEnabled = true;
        }

        public Result<ProductUpdated> Disable()
        {
            this.isEnabled = false;
            return Result.Ok(ProductUpdated.Create(this));
        }

        public Result<ProductUpdated> Enable()
        {
            this.isEnabled = true;
            return Result.Ok(ProductUpdated.Create(this));
        }

        public static Result<ProductCreated> Create(CreateProductCommand command)
        {
            var result = Result.Ok();
            //Add additional validations and return failures in Result if needed
            var agg = new ProductAgg(command);

            //if (command.Plate == "Lucas")
            //    result.WithError("Plate cannot be equals Lucas");

            return result.ToResult(agg.Created());
        }

        public Result<ProductUpdated> Change(UpdateProductCommand command)
        {
            var result = Result.Ok();
            //Add additional validations and return failures in Result if needed

            this.Name = command.Name;
            this.Description = command.Description;
            this.Price = command.Price;
            this.Quantity = command.Quantity;
            this.MaxItemsPerCart = command.MaxItemsPerCart;

            return result.ToResult(ProductUpdated.Create(this));
        }

        public ProductCreated Created()
        {
            return ProductCreated.Create(this);
        }
    }
}
