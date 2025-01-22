using Optimus.Core.Domain.Aggregates.Product;
using Optimus.Core.Domain.Aggregates.Ticket.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Commands;

/// <summary>
/// Command to insert a new product in an existing ticket
/// </summary>
/// <param name="ProductId">The product unique identifier</param>
/// <param name="Quantity">The quantity of the product</param>
public record AddProductToTicketCommand(string ProductId, decimal Quantity) : ICommand<TicketProductsChanged>
{
    public string TicketId { get; set; }
}


/// <summary>
/// Command to remove an existing product from the ticket
/// </summary>
/// <param name="TicketId">The ticket to remove the product from</param>
/// <param name="ProductId">The product unique identifier</param>
public record RemoveProductFromTicketCommand(string TicketId, string ProductId) : ICommand<TicketUpdated>
{
    [JsonIgnore]
    public TicketAgg? Ticket { get; set; }

    [JsonIgnore]
    public ProductAgg? Product { get; set; }
};


/// <summary>
/// Command to remove an existing product from the ticket
/// </summary>
/// <param name="TicketId">The ticket to remove the product from</param>
/// <param name="ProductId">The product unique identifier</param>
public record ChangeProductQuantityOnTicketCommand(string TicketId, string ProductId, decimal Quantity) : ICommand<TicketUpdated>
{
    [JsonIgnore]
    public TicketAgg? Ticket { get; set; }

    [JsonIgnore]
    public ProductAgg? Product { get; set; }
};