using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Application.Attendant.State;
public interface IAttendantState : IState<AttendantAgg>
{
    public Task<AttendantAgg> GetByCard(string cardId);
    public Task<IEnumerable<AttendantAgg>> GetAll();

}
