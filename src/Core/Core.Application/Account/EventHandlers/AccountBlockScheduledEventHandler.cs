//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.EventHandlers
//{
//    public class AccountBlockScheduledEventHandler : INotificationHandler<AccountBlockScheduled>
//    {
//        private readonly IStream _stream;
//        private readonly ILogger _logger;
//        private readonly IMailer _sendGridEmailer;
//        private readonly ISharepointService _sharepointService;

//        public AccountBlockScheduledEventHandler(
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

//        public async Task Handle(AccountBlockScheduled notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Sending message to topic {DestinationTopic}", TOPICS.ADUserBlockScheduled);
//            await _stream.SendEventAsync(notification, TOPICS.ADUserBlockScheduled, notification.BlockingDate);

//            _logger.LogInformation("Account {AccountId} has a scheduled block. Sending email to admins.", notification.AccountId);

//            var adminEmails = await _sharepointService.GetAdminEmails();

//            string operationText = $"The account with email address: \"{notification.Email}\" has been scheduled to be disabled in Active Airectory in {notification.BlockingDate} UTC.";

//            EmailToSend email = new()
//            {
//                To = adminEmails,
//                Subject = "User account block scheduled",
//                TextBody = operationText
//            };

//            await _sendGridEmailer.SendEmail(email);
//        }
//    }
//}
