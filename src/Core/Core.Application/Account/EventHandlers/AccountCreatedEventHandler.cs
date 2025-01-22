//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Services.SendGrid.Handlers
//{
//    public class AccountCreatedEventHandler : INotificationHandler<AccountCreatedEvent>
//    {
//        private readonly ILogger _logger;
//        private readonly IMailer _sendGridEmailer;
//        private readonly ISharepointService _sharepointService;

//        public AccountCreatedEventHandler(ILogger<AccountCreatedEventHandler> logger, IMailer mailer, ISharepointService sharepointService)
//        {
//            _logger = logger;
//            _sendGridEmailer = mailer;
//            _sharepointService = sharepointService;
//        }

//        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("New account {AccountEmail} created. Sending email to admins", notification.Account.Email);

//            var adminEmails = await _sharepointService.GetAdminEmails(notification.Account.Roles.Select(r => r.Application.Name));

//            string operationText = $"{notification.Account.FullName} with email address: \"{notification.Account.Email}\" will be joining the department: \"{notification.Account.Department}\", and division: \"{notification.Account.Division}\" for the position: \"{notification.Account.JobTitle}\".";

//            EmailToSend email = new()
//            {
//                To = adminEmails,
//                Subject = "New user has been added in Active Directory",
//                TextBody = operationText
//            };

//            await _sendGridEmailer.SendEmail(email, notification.Account);
//        }
//    }
//}
