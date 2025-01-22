using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Application.Attendant.Queries.GetAll
{

    public class AttendantGetAllQueryHandler(IAttendantState state) : IQueryManyHandler<AttendantGetAll, AttendantAgg>
    {
        
        public async Task<Result<IEnumerable<AttendantAgg>>> Handle(AttendantGetAll request, CancellationToken cancellationToken)
        {
            try
            {
                var attendants = await state.GetAll();
                var models = Result.Ok(attendants);
                return models;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

}
