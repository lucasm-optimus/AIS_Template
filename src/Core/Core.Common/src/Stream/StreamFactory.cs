using Microsoft.Extensions.Configuration;

namespace Optimus.Core.Common.Stream
{
    internal class StreamSettings
    {
        public enum StreamType
        {
            AzureEventHub,
            AzureServiceBus,
            RabbitMQ
        }

        public StreamType Service { get; set; }
    }

    internal static class StreamFactory
    {
        public static IServiceCollection RegisterStreams(this IServiceCollection services, IConfiguration configuration)
        {
            //Load and register the Settings in a Singleton
            var settings = configuration.GetSection("StreamSettings").Get<StreamSettings>();

            if (settings != null)
            {
                services.AddSingleton<StreamSettings>(settings);

                services.AddScoped<IStream>(serviceProvider =>
                {
                    //Get the settings from memory
                    var env = serviceProvider.GetRequiredService<StreamSettings>();
                    //Return the service based on the settings
                    return serviceProvider.GetRequiredKeyedService<IStream>(env.Service.ToString());
                });
            }

            return services;
        }
    }
}
