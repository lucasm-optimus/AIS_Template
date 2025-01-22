//using Optimus.Core.Common.Stream;
//using Optimus.Core.Application.Ticket.Adapters.Stream;
//using Optimus.Core.Domain.Aggregates.Ticket.Events;

//namespace Optimus.Core.Application.Ticket.Events
//{
//    internal class TicketCreatedEventHandler(IStream stream) : INotificationHandler<TicketCreated>
//    {
//        public Task Handle(TicketCreated notification, CancellationToken cancellationToken)
//        {
//            return stream.SendEventAsync(notification, TicketStream.TicketCreated);
//        }
//    }
//}
