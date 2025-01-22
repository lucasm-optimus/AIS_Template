//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.EventHandlers
//{
//    public class AccountChangeRequestedEventHandler : INotificationHandler<AccountChangeRequestedEvent>
//    {
//        private readonly IStream _stream;
//        private readonly ILogger _logger;

//        public AccountChangeRequestedEventHandler(ILogger<AccountChangeRequestedEventHandler> logger, IStream stream)
//        {
//            _stream = stream;
//            _logger = logger;
//        }

//        public Task Handle(AccountChangeRequestedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Sending message to topic {DestinationTopic}", TOPICS.AccountChangeRequested);

//            return _stream.SendEventAsync(notification, TOPICS.AccountChangeRequested);
//        }
//    }
//}
