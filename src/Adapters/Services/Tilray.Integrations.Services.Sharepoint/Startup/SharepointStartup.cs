namespace Tilray.Integrations.Services.Sharepoint.Startup;

/// <summary>
/// This classe is responsible for registering the SharepointService in the DI container.
/// </summary>
public class SharepointStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("SharepointAPI").Get<SharepointSettings>();
        services.AddSingleton(settings ?? new SharepointSettings());

        services.AddScoped<ISharepointService, SharepointService>();

        var options = new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };

        ClientSecretCredential clientSecretCredential = new(settings.TenantId, settings.ClientId, settings.ClientSecret, options);

        services.AddSingleton(new GraphServiceClient(clientSecretCredential, settings.Scopes));
        return services;
    }
}
