using Azure.Messaging.ServiceBus;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Tilray.Integrations.Core.Common.Startup;

var builder = FunctionsApplication.CreateBuilder(args);

builder
    .ConfigureFunctionsWebApplication();
//.UseOutputBindingsMiddleware()
//.UseMiddlewares();

string ApplicationName = "Tilray Integration Functions";
builder.RegisterCommonServices(ApplicationName);

builder.Services.AddSingleton(options => { return new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString")); });

builder.Build().Run();