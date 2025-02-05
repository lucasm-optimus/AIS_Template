using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Tilray.Integrations.Core.Application.Adapters.Services;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Services.Sharepoint.Service;
using Azure.Identity;

namespace Tilray.Integrations.Services.Sharepoint.Startup
{
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
}
