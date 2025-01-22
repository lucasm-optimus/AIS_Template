using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.Core.Application.Attendant.Queries;
public record AttendantGetAll : QueryManyBase<AttendantAgg>
{
    public AttendantGetAll()
    {
        
    }
 
}


