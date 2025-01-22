using Optimus.Core.Domain.Aggregates.Attendant.Events;

namespace Optimus.Core.Domain.Aggregates.Attendant.Commands;

#pragma warning disable
public class CreateAttendantCommand : ICommand<AttendantCreated>
{
    public string CardId { get; set; }
    public string Name { get; set; }
}
