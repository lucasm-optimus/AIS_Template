
namespace Optimus.Core.Domain.Aggregates.Ticket
{
    public class Payment : Entity
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }

    }
}
