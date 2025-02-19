namespace Tilray.Integrations.Services.Authorize.net.Startup;

public class AuthorizeNetStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var authorizeNetSettings = configuration.GetSection("AuthorizeNetAPI").Get<AuthorizeNetSettings>();
        services.AddSingleton(authorizeNetSettings ?? new AuthorizeNetSettings());

        services.AddScoped<IAuthorizeNetService, AuthorizeNetService>();

        return services;
    }
}
