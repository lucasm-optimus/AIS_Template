
var builder = FunctionsApplication.CreateBuilder(args);

string ApplicationName = "Optimus Info Integration Functions";

var app = builder
    .ConfigureFunctionsWebApplication()
    .UseOutputBindingsMiddleware();

builder.RegisterCommonServices(ApplicationName);

app.UseOptimusMiddlewares();

builder.Build().Run();