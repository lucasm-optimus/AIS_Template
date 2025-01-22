using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Application.Attendant.Queries.Get;

public class AttendantGetByCardQueryHandler(IAttendantState state) : IQueryHandler<AttendantGetByCard, AttendantAgg>
{
    public async Task<Result<AttendantAgg>> Handle(AttendantGetByCard request, CancellationToken cancellationToken)
    {
        return await state.GetByCard(request.CardId);
    }
}


