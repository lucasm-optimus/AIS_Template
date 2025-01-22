using Optimus.Core.Domain.Aggregates.Ticket;

namespace Optimus.Core.Application.Ticket.Queries
{
    public record TicketGetOne : IQuery<TicketAgg>
    {
        public TicketGetOne(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

}
