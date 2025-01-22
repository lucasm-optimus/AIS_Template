using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Domain.Aggregates.Ticket
{
    public class Attendant : Entity
    {
        public string Name { get; internal set; }
        public string CardId { get; internal set; }

        private Attendant(AttendantAgg attendant)
        {
            Name = attendant.Name;
            CardId = attendant.CardId;
        }

        public static Attendant Create(AttendantAgg attendant)
        {
            return new Attendant(attendant);
        }
    }
}
