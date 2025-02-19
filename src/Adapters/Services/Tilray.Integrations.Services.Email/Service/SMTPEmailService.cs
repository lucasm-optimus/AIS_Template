using System.Net.Mail;
using System.Net;
using Tilray.Integrations.Core.Application.Adapters.Services;
using Tilray.Integrations.Services.Email.Startup;

namespace Tilray.Integrations.Services.Email.Service;

public class SmtpEmailService(EmailSettings emailSettings) : IEmailService
{
    public async Task SendEmailAsync(string subject, string body)
    {
        using var smtpClient = new SmtpClient(emailSettings.SmtpHost)
        {
            Port = emailSettings.SmtpPort,
            Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.SmtpPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings.SmtpUser),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var to in emailSettings.ToEmailsList())
        {
            mailMessage.To.Add(to);
        }

        foreach (var cc in emailSettings.CcEmailsList())
        {
            mailMessage.CC.Add(cc);
        }

        await smtpClient.SendMailAsync(mailMessage);
    }
}
