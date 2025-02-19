using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.Adapters.Services;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Services.Email.Service;

namespace Tilray.Integrations.Services.Email.Startup;

public class EmailStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        services.AddSingleton(emailSettings ?? new EmailSettings());

        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
