
using AutoMais.Core.Domain.Aggregates.Account.Commands;

namespace AutoMais.Core.Domain.Aggregates.Account.Events
{
    public interface IAccountBlockedEvent : MediatR.INotification, IDomainEvent
    {
        string AccountId { get; }
    }

    public class AccountBlockedEvent : IAccountBlockedEvent
    {
        public string AccountId { get; }
        public string Email { get; }

        public AccountBlockedEvent(string accountId, string email)
        {
            Email = email;
            AccountId = accountId;
        }
    }

    public class AccountBlockScheduled : IAccountBlockedEvent
    {
        public DateTime BlockingDate { get; private set; }
        public string AccountId { get; }
        public string Email { get; }

        public AccountBlockScheduled(string accountId, string email, DateTime blockingDate)
        {
            BlockingDate = blockingDate;
            AccountId = accountId;
            Email = email;
        }

        /// <summary>
        /// Converts this event into a BlockAccountCommand
        /// </summary>
        /// <returns>A BlockAccountCommand object</returns>
        public BlockAccountCommand ToCommand()
        {
            return new BlockAccountCommand(AccountId, Email, BlockingDate);
        }
    }
}
