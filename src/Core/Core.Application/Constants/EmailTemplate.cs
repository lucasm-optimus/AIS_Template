namespace Tilray.Integrations.Core.Application.Constants;

public static class EmailTemplate
{
    public static (string Subject, string Body) GetCaptureAttemptedOnInvalidTransactionTemplate(string transactionId, string transactionStatus)
    {
        var subject = "Attempt to Capture Voided or Previously Captured Transaction";
        var body = $@"
            <html>
                <body>
                    <p><strong>Transaction Id:</strong> {transactionId}</p>
                    <p><strong>Status:</strong> {transactionStatus}</p>
                </body>
            </html>";

        return (subject, body);
    }

    public static (string Subject, string Body) GetCapturePaymentFailedTemplate(string transactionId, string message)
    {
        var subject = "Capture Payment Failed";
        var body = $@"
            <html>
                <body>
                    <p><strong>Transaction Id:</strong> {transactionId}</p>
                    <p><strong>Message:</strong> {message}</p>
                </body>
            </html>";

        return (subject, body);
    }
}
