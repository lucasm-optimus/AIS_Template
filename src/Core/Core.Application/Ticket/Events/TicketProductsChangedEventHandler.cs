//using Optimus.Core.Common.Stream;
//using Optimus.Core.Application.Ticket.Adapters.Stream;
//using Optimus.Core.Domain.Aggregates.Ticket.Events;

//namespace Optimus.Core.Application.Ticket.Events
//{
//    internal class TicketProductsChangedEventHandler(IStream stream) : INotificationHandler<TicketProductsChanged>
//    {
//        public Task Handle(TicketProductsChanged notification, CancellationToken cancellationToken)
//        {
//            return stream.SendEventAsync(notification, TicketStream.TicketUpdated);
//        }
//    }
//}
