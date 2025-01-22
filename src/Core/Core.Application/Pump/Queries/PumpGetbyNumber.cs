using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump;

namespace Optimus.Core.Application.Pump.Queries
{
    public record PumpGetbyNumber(int Number) : IQuery<PumpAgg>;

    public record PumpGetAll() : QueryManyBase<PumpAgg>;



    public class PumpGetQueryHandler(IPumpState state) : 
        IQueryHandler<PumpGetbyNumber, PumpAgg>,
        IQueryManyHandler<PumpGetAll, PumpAgg>
    {
        public async Task<Result<PumpAgg>> Handle(PumpGetbyNumber request, CancellationToken cancellationToken)
        {
            //Validate if user can retrieve the desired information
            //Check if the information can be returned to the user...

            return await state.Get(x => x.Number == request.Number);
        }

        public async Task<Result<IEnumerable<PumpAgg>>> Handle(PumpGetAll request, CancellationToken cancellationToken)
        {
            //Validate if user can retrieve the desired information
            //Check if the information can be returned to the user...

            return Result.Ok(await state.GetMany(x => true));
        }
    }
}
