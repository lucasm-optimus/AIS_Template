using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Events;

namespace Tilray.Integrations.Core.Application.SalesOrdersPayments.EventHandlers;

public class CapturePaymentFailedEventHandler(IEmailService emailService)
    : INotificationHandler<CapturePaymentFailed>
{
    public async Task Handle(CapturePaymentFailed notification, CancellationToken cancellationToken)
    {
        var (subject, body) = EmailTemplate.GetCapturePaymentFailedTemplate(notification.TransactionId, notification.ErrorMessage);
        await emailService.SendEmailAsync(subject, body);
    }
}
