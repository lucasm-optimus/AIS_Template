using Optimus.Core.Application.Pump.State;
using Optimus.Core.Domain.Aggregates.Pump;

namespace Optimus.Core.Application.Pump.Queries
{
    public record NozzlesOfPump : QueryManyBase<Nozzle>
    {
        public NozzlesOfPump(int number, int pageSize, int pageNumber)
        {
            Number = number;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int Number { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class NozzelsOfPumpValidator : AbstractValidator<NozzlesOfPump>
    {
        public NozzelsOfPumpValidator()
        {
            RuleFor(command => command.Number)
                .NotEmpty()
                .WithMessage("The Pump Number can't be empty.");
        }
    }

    public class NozzlesOfPumpQueryHandler(IPumpState state) : IQueryManyHandler<NozzlesOfPump, Nozzle>
    {
        public async Task<Result<IEnumerable<Nozzle>>> Handle(NozzlesOfPump request, CancellationToken cancellationToken)
        {
            var items = await state.Get(x => x.Number == request.Number);

            return Result.Ok(items.Nozzles);
        }
    }
}
