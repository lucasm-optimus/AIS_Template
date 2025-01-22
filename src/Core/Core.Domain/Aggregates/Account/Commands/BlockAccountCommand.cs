using AutoMais.Core.Domain.Aggregates.Account.Events;

namespace AutoMais.Core.Domain.Aggregates.Account.Commands
{
    /// <summary>
    /// Represents a feature to transfer an account and returns an Event
    /// </summary>
    public class BlockAccountCommand : MediatR.IRequest<Result<IAccountBlockedEvent>>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public DateTime BlockingDate { get; set; }

        public BlockAccountCommand(AccountChangeRequestedCommand command)
        {
            Id = command.Id;
            Email = command.Email;
            BlockingDate = command.BlockingDate;
        }

        public BlockAccountCommand(string accountId, string email, DateTime blockingDate)
        {
            Id = accountId;
            Email = email;
            BlockingDate = blockingDate;
        }

        public AccountAgg ToDomain()
        {
            return new AccountAgg(this);
        }
    }
}
