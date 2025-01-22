using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Application.Attendant.Queries;
public record AttendantGetByCard : IQuery<AttendantAgg>
{
    public AttendantGetByCard(string id)
    {
        CardId = id.ToLower();
    }
    public string CardId { get; set; }
}
