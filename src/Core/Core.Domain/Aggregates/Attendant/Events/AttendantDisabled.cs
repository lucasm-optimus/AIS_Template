using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Attendant.Events;


public record AttendantDisabled : AttendantChanged;
public record AttendantEnabled : AttendantChanged;

public abstract record AttendantChanged : IDomainEvent
{
    [JsonIgnore]
    public AttendantAgg Attendant { get; private set; }

    public string AttendantId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? DisableDate { get; set; }
    public string CardId { get; private set; }
    public bool IsEnabled { get; private set; }


    public static AttendantEnabled CreateEnabled(AttendantAgg attendantAgg)
        => new AttendantEnabled
        {
            AttendantId = attendantAgg.Id,
            CardId = attendantAgg.CardId,
            Name = attendantAgg.Name,
            CreatedDate = attendantAgg.CreatedDate,
            DisableDate = attendantAgg.DisabledDate,
            IsEnabled = attendantAgg.IsEnabled,
            Attendant = attendantAgg
        };


    public static AttendantDisabled CreateDisabled(AttendantAgg attendantAgg)
        => new AttendantDisabled
        {
            AttendantId = attendantAgg.Id,
            CardId = attendantAgg.CardId,
            Name = attendantAgg.Name,
            CreatedDate = attendantAgg.CreatedDate,
            DisableDate = attendantAgg.DisabledDate,
            IsEnabled = attendantAgg.IsEnabled,
            Attendant = attendantAgg
        };
}
