using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Functions.Usecase.SOXReport;
using Tilray.Integrations.Services.CSV.Startup;
using Tilray.Integrations.Services.Salesforce.Service;

var builder = FunctionsApplication.CreateBuilder(args);

string ApplicationName = "Tilray Integration Functions";

var config = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Configuration.AddConfiguration(config);

var app = builder
    .ConfigureFunctionsWebApplication()
    .UseOutputBindingsMiddleware();

builder.RegisterCommonServices(ApplicationName);

app.UseMiddlewares();

builder.Build().Run();