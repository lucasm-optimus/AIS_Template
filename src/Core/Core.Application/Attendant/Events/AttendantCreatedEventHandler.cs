//using Optimus.Core.Application.Attendant.Stream;
//using Optimus.Core.Domain.Aggregates.Attendant.Events;
//using Becape.Core.Common.Stream;

//namespace Optimus.Core.Application.Attendant.Events;
//internal class AttendantCreatedEventHandler(IStream stream) : INotificationHandler<AttendantCreated>
//{
//    public Task Handle(AttendantCreated notification, CancellationToken cancellationToken)
//     =>   stream.SendEventAsync(notification, AttendantStream.AttendantCreated) ;
//}
