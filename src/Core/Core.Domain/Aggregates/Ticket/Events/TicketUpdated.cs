using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket.Events
{
    public class TicketUpdated : IDomainEvent
    {
        public string Id { get; private set; }
        public string Code { get; private set; }
        public TicketStatusEnum Status { get; private set; }

        public decimal ProductsPrice { get; private set; }
        public decimal ProductsQuantity { get; private set; }

        public decimal FuelVolume { get; private set; }
        public decimal FuelPrice { get; private set; }
        public decimal TotalCost { get; private set; }

        public DateTime LastUpdatedDate { get; private set; }


        [JsonIgnore] //Make this property non-serializable
        public TicketAgg Ticket { get; private set; }

        public static TicketUpdated Create(TicketAgg ticket)
        {
            return new TicketUpdated
            {
                Id = ticket.Id,
                Code = ticket.Code,
                Ticket = ticket,
                ProductsPrice = ticket.ProductsTotalPrice,
                ProductsQuantity = ticket.ProductsTotalQuantity,
                FuelVolume = ticket.FuelTotalVolume,
                FuelPrice = ticket.FuelTotalPrice,
                TotalCost = ticket.TotalCost,
                Status = ticket.Status,
                LastUpdatedDate = ticket.LastUpdatedDate
            };
        }
    }
}
