namespace Optimus.Core.Domain.Aggregates.Ticket.Events;

/// <summary>
/// Represents the successful event of changing the ticket product list
/// </summary>
public class TicketProductsChanged : IDomainEvent
{
    public string Id { get; private set; }
    public decimal TotalItems { get; private set; }
    public decimal TotalPrice { get; private set; }

    internal TicketProductsChanged(string ticketId, decimal totalItems, decimal totalPrice)
    {
        Id = ticketId;
        TotalItems = totalItems;
        TotalPrice = totalPrice;
    }

    public static TicketProductsChanged Create(TicketAgg ticket)
    {
        return new TicketProductsChanged(ticket.Id, ticket.ProductsTotalQuantity, ticket.ProductsTotalPrice);
    }
}
