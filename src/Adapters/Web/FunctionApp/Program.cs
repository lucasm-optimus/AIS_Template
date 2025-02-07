using Tilray.Integrations.Core.Common.Startup;

var builder = FunctionsApplication.CreateBuilder(args);

string ApplicationName = "Tilray Integration Functions";

var app = builder
    .ConfigureFunctionsWebApplication()
    .UseOutputBindingsMiddleware();

builder.RegisterCommonServices(ApplicationName);

app.UseMiddlewares();

builder.Build().Run();
