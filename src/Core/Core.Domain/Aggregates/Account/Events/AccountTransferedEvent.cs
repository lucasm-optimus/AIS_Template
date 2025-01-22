
namespace AutoMais.Core.Domain.Aggregates.Account.Events
{
    public class AccountTransferedEvent : MediatR.INotification, IDomainEvent
    {
        public AccountAgg Account { get; private set; }
        public AccountTransferedEvent(AccountAgg account)
        {
            Account = account;
        }
    }
}
