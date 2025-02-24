using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Common.Startup;

namespace Tilray.Integrations.Storage.Blob.Startup;

public class BlobStartup : IStartupRegister
{
    public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("BlobSettings").Get<BlobSettings>();
        services.AddSingleton(settings ?? new BlobSettings());

        services.AddSingleton(new BlobServiceClient(configuration["AzureWebJobsStorage"]));
        services.AddScoped<IBlobService, BlobService>();

        return services;
    }
}
