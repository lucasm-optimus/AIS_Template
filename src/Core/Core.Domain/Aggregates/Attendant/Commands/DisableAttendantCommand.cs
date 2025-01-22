using Optimus.Core.Domain.Aggregates.Attendant.Events;
using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Attendant.Commands;

public record ChangeAttendantCommand<T>(string CardId, bool isEnabled) : ICommand<T> where T : IDomainEvent
{
    [JsonIgnore]
    public AttendantAgg Attendang { get; set; }
}

public record DisableAttendantCommand : ChangeAttendantCommand<AttendantDisabled>
{
    public DisableAttendantCommand(string attendantId) : base(attendantId, false)
    {

    }
}

public record EnableAttendantCommand : ChangeAttendantCommand<AttendantEnabled>
{
    public EnableAttendantCommand(string attendantId) : base(attendantId, true)
    {

    }
}
