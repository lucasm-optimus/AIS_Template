//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.EventHandlers
//{
//    public class AccountBlockedEventHandler : INotificationHandler<AccountBlockedEvent>
//    {
//        private readonly IStream _stream;
//        private readonly ILogger _logger;
//        private readonly IMailer _sendGridEmailer;
//        private readonly ISharepointService _sharepointService;

//        public AccountBlockedEventHandler(
//            ILogger<AccountBlockScheduledEventHandler> logger, 
//            IStream stream, 
//            IMailer mailer, 
//            ISharepointService sharepointService)
//        {
//            _stream = stream;
//            _logger = logger;
//            _sendGridEmailer = mailer;
//            _sharepointService = sharepointService;
//        }

//        public async Task Handle(AccountBlockedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Sending message to topic {DestinationTopic}", TOPICS.ADUserBlocked);
//            await _stream.SendEventAsync(notification, TOPICS.ADUserBlocked);

//            _logger.LogInformation("Account {AccountId} blocked. Sending email to admins.", notification.AccountId);

//            var adminEmails = await _sharepointService.GetAdminEmails();

//            string operationText = $"The account with email address: \"{notification.Email}\" has been disabled in active directory. Please ensure the right blocking in your respective system.";

//            EmailToSend email = new()
//            {
//                To = adminEmails,
//                Subject = "User account has been disabled in Active Directory",
//                TextBody = operationText
//            };

//            await _sendGridEmailer.SendEmail(email);
//        }
//    }
//}
