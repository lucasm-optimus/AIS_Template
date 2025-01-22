using Optimus.Core.Domain.Aggregates.Ticket;

namespace Optimus.Core.Application.Ticket.Queries
{
    public record TicketsOfAttendant(string CardId) : QueryManyBase<TicketAgg>;

    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class TicketsOfAttendantValidator : AbstractValidator<TicketsOfAttendant>
    {
        public TicketsOfAttendantValidator()
        {
            RuleFor(command => command.CardId)
                .NotEmpty()
                .WithMessage("The CardId can't be empty.");
        }
    }
}
