using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket;

namespace Optimus.Core.Application.Ticket.Queries;

public class TicketsOfAttendantQueryHandler(ITicketState state) : IQueryManyHandler<TicketsOfAttendant, TicketAgg>
{
    public async Task<Result<IEnumerable<TicketAgg>>> Handle(TicketsOfAttendant request, CancellationToken cancellationToken)
    {
        var items = await state.GetPagedAsync(x => x.Attendant.CardId == request.CardId, request.Page, request.ItemsPerPage);

        return Result.Ok(items);
    }
}
