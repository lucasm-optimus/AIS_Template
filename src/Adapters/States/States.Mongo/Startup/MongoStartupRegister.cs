using Optimus.Core.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Reflection;

namespace Optimus.States.Mongo.Startup
{
    public class MongoSettings : IApplication
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class MongoStartupRegister : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            // Set up MongoDB conventions
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, t => true);

            // Bind MongoSettings from configuration
            var settings = configuration.GetSection("StateSettings:MongoDB").Get<MongoSettings>();
            services.AddSingleton(settings ?? new MongoSettings());

            // Register MongoClient
            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoSettings = sp.GetRequiredService<MongoSettings>();
                return new MongoClient(mongoSettings.ConnectionString);
            });

            // Register IMongoDatabase
            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var mongoSettings = sp.GetRequiredService<MongoSettings>();
                return client.GetDatabase(mongoSettings.DatabaseName);
            });

            var startupRegisterType = typeof(IState<>);

            var assembly = Assembly.GetExecutingAssembly();
            var repositoryTypes = assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(startupRegisterType.Name) != null)
                .ToList();

            foreach (var repositoryType in repositoryTypes)
            {
                var interfaces = repositoryType.GetInterfaces()
                    .Where(@interface => @interface != startupRegisterType && @interface.Name != startupRegisterType.Name)
                    .ToList();

                if (interfaces.Count != 1)
                {
                    throw new InvalidOperationException($"Repository '{repositoryType.Name}' must implement only one interface that implements IState<>");
                }

                services.AddScoped(interfaces[0], repositoryType);
            }

            return services;
        }
    }
}
