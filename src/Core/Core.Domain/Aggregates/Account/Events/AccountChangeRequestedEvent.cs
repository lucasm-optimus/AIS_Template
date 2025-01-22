using AutoMais.Core.Domain.Aggregates.Account.Commands;

namespace AutoMais.Core.Domain.Aggregates.Account.Events
{
    public class AccountChangeRequestedEvent : AccountChangeRequestedCommand, MediatR.INotification, IDomainEvent
    {

    }
}
