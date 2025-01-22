

using Optimus.Core.Domain.Aggregates.Product;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;

namespace Optimus.Core.Domain.Aggregates.Ticket
{
    public class Product : Entity
    {
        public required string Id { get; init; }

        public required string Name { get; init; }

        public required decimal Price { get; init; }

        public decimal Quantity { get; private set; }

        public decimal Total => Price * Quantity;


        internal void ChangeQuantity(decimal quantity)
        {
            Quantity = quantity;
        }

        public static Product Create(AddProductToTicketCommand command, ProductAgg product)
        {
            return new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = command.Quantity
            };
        }
    }
}
