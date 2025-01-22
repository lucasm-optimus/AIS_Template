using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Attendant.Events;
#pragma warning disable 
public class AttendantCreated : IDomainEvent
{
    [JsonIgnore]
    public AttendantAgg Attendant { get; private set; }

    public string AttendantId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? DisableDate { get; set; }
    public string CardId { get; private set; }

    public static AttendantCreated Create(AttendantAgg attendantAgg)
        => new AttendantCreated
        {
            AttendantId = attendantAgg.Id,
            CardId = attendantAgg.CardId,
            Name = attendantAgg.Name,
            CreatedDate = attendantAgg.CreatedDate,
            DisableDate = attendantAgg.DisabledDate,
            Attendant = attendantAgg
        };

}
