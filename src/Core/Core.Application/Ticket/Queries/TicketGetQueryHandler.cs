using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket;

namespace Optimus.Core.Application.Ticket.Queries;

public class TicketGetQueryHandler(ITicketState state) : IQueryHandler<TicketGetOne, TicketAgg>
{
    public async Task<Result<TicketAgg>> Handle(TicketGetOne request, CancellationToken cancellationToken)
    {
        //Validate if user can retrieve the desired information
        //Check if the information can be returned to the user...

        return await state.Get(request.Id);
    }
}
