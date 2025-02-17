using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Tilray.Integrations.Core.Common.Startup;

public static class StartupExtensions
{
    static readonly Assembly[] _Assemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(a => a.GetName().Name.StartsWith("Tilray.Integrations"))
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
            //cfg.AddOpenBehavior(typeof(MediatrValidationBehavior<,>));
        });

        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var keyVaultUri = configuration["KeyVault:Uri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential(), new PrefixKeyVaultSecretManager("AISFunctions"));
        }

        builder.Services.RegisterStartupClasses(configuration);
    }


    public static IFunctionsWorkerApplicationBuilder UseMiddlewares(this IFunctionsWorkerApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        //app.UseMiddleware<ResultTypeFilter>();

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
                .Where(file => file.Contains("Tilray.Integrations"))
                .ToList();

        foreach (var dll in assemblies)
        {
            Assembly.LoadFrom(dll);
        }
    }

    private class PrefixKeyVaultSecretManager : KeyVaultSecretManager
    {
        private readonly string _prefix;

        public PrefixKeyVaultSecretManager(string prefix)
            => _prefix = $"{prefix}--";

        public override bool Load(SecretProperties properties)
            => properties.Name.StartsWith(_prefix);

        public override string GetKey(KeyVaultSecret secret)
            => secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
    }
}
