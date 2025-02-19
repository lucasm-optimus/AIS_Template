using Tilray.Integrations.Services.Sharepoint.Service.MappingProfiles;

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

        if (settings != null &&
            !string.IsNullOrWhiteSpace(settings.TenantId) &&
            !string.IsNullOrWhiteSpace(settings.ClientId) &&
            !string.IsNullOrWhiteSpace(settings.ClientSecret) &&
            settings.Scopes != null && settings.Scopes.Length > 0)
        {
            var options = new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };
            var clientSecretCredential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret, options);

            services.AddSingleton(new GraphServiceClient(clientSecretCredential, settings.Scopes));
        }

        services.AddAutoMapper(typeof(SharepointInvoiceMapper));
        return services;
    }
}
