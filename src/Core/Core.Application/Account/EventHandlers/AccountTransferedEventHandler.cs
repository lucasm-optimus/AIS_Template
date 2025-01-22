//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Services.SendGrid.Handlers
//{
//    public class AccountTransferedEventHandler : INotificationHandler<AccountTransferedEvent>
//    {
//        private readonly ILogger _logger;
//        private readonly IMailer _sendGridEmailer;
//        private readonly ISharepointService _sharepointService;

//        public AccountTransferedEventHandler(ILogger<AccountTransferedEventHandler> logger, IMailer mailer, ISharepointService sharepointService)
//        {
//            _logger = logger;
//            _sendGridEmailer = mailer;
//            _sharepointService = sharepointService;
//        }

//        public async Task Handle(AccountTransferedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Account {AccountId} transfered. Sending email to admins.", notification.Account.Id);

//            var adminEmails = await _sharepointService.GetAdminEmails();

//            string operationText = $"{notification.Account.FullName} with email address: \"{notification.Account.Email}\" has been transfered to department: \"{notification.Account.Department}\", and division: \"{notification.Account.Division}\" for the position: \"{notification.Account.JobTitle}\".";

//            EmailToSend email = new()
//            {
//                To = adminEmails,
//                Subject = "User account has been transfered",
//                TextBody = operationText
//            };

//            await _sendGridEmailer.SendEmail(email, notification.Account);
//        }
//    }
//}
