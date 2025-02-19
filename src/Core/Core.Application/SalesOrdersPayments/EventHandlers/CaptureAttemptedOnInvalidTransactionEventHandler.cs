using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Events;

namespace Tilray.Integrations.Core.Application.SalesOrdersPayments.EventHandlers;

public class SalesOrderPaymentProcessedEventHandler(IEmailService emailService)
    : INotificationHandler<CaptureAttemptedOnInvalidTransaction>
{
    public async Task Handle(CaptureAttemptedOnInvalidTransaction notification, CancellationToken cancellationToken)
    {
        var (subject, body) = EmailTemplate.GetCaptureAttemptedOnInvalidTransactionTemplate(notification.TransactionId, notification.TransactionStatus);
        await emailService.SendEmailAsync(subject, body);
    }
}
