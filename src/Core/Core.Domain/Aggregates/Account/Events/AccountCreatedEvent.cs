
namespace AutoMais.Core.Domain.Aggregates.Account.Events
{
    public class AccountCreatedEvent : MediatR.INotification, IDomainEvent
    {
        public AccountAgg Account { get; private set; }
        public AccountCreatedEvent(AccountAgg account)
        {
            Account = account;
        }
    }
}
