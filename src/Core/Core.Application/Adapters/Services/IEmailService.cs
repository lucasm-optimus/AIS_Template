namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string body);
}
