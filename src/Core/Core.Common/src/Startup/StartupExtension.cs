using Optimus.Core.Common.ActionFilters;
using Optimus.Core.Common.Middlewares;
using Optimus.Core.Common.Stream;
using Optimus.Core.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;

namespace Optimus.Core.Common.Startup;

public static class StartupExtensions
{
    static readonly Assembly[] _Assemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(a => a.GetName().Name.StartsWith(typeof(StartupExtensions).Namespace.Split('.')[0]))
        .ToArray();


    public static void RegisterCommonServices(this IHostApplicationBuilder builder, string applicationName)
    {
        LoadAssemblies(); //This method forces to load all assemblies in the bin folder

        builder.AddLoggingServices(applicationName);

        builder.ConfigureHttpResiliency();

        //Register all validators founded in the Core.Application project
        builder.Services.AddValidatorsFromAssemblies(_Assemblies);

        //Here we will map all the Mediatr files to the Dependency Injection
        builder.Services.AddMediatR(cfg =>
        {
            //Register all handlers founded in the Core.Application project
            cfg.RegisterServicesFromAssemblies(_Assemblies);

            //Add the Validation Behavior to the Mediatr pipeline
            cfg.AddOpenBehavior(typeof(MediatrValidationBehavior<,>));
        });

        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        builder.Services.RegisterStartupClasses(configuration);
        builder.Services.RegisterStreams(configuration);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSwaggerGen(options =>
        {
            options.OperationFilter<MinimumResultTypeFilter>();
        });
    }


    public static IFunctionsWorkerApplicationBuilder UseOptimusMiddlewares(this IFunctionsWorkerApplicationBuilder app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ResultTypeFilter>();

        return app;
    }

    private static IHostApplicationBuilder AddLoggingServices(this IHostApplicationBuilder builder, string applicationName)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddSimpleConsole();
        builder.Logging.AddAzureWebAppDiagnostics();
        builder.Logging.AddApplicationInsights();

        builder.Logging.AddOpenTelemetry();

        builder.Services.AddTransient(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(applicationName);
        });

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        return builder;
    }

    private static Polly.CircuitBreaker.AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
    }

    private static void ConfigureHttpResiliency(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakPolicy());
        });
    }

    private static Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static void LoadAssemblies()
    {
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(file => file.Contains("Optimus."))
                .ToList();

        foreach (var dll in assemblies)
        {
            Assembly.LoadFrom(dll);
        }
    }

}
