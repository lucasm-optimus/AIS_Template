using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

namespace Optimus.Core.Common.Middlewares;

public class LoggingMiddleware(ILogger logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            var state = new Dictionary<string, object>
            {
                ["InvocationId"] = context.InvocationId,
                ["TraceId"] = context.TraceContext.TraceParent
            };

            if (context.BindingContext.BindingData.TryGetValue("MessageId", out var messageId) && messageId != null)
                state.Add("MessageId", messageId);

            if (context.BindingContext.BindingData.TryGetValue("CorrelationId", out var correlationId) && correlationId != null)
                state.Add("CorrelationId", correlationId);

            using (logger.BeginScope(state))
            {
                foreach (var item in state)
                {
                    System.Diagnostics.Activity.Current?.AddBaggage(item.Key, item.Value.ToString());
                    System.Diagnostics.Activity.Current?.AddTag(item.Key, item.Value);
                }

                var httpContext = context.GetHttpContext();

                if (httpContext != null)
                {
                    httpContext.Response.Headers.Append("TraceId", context.TraceContext.TraceParent);
                    logger.LogDebug("[Web][Request][{method} {endpoint}]", httpContext.Request.Method, httpContext.Request.Path);
                }

                await next(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");

            var request = await context.GetHttpRequestDataAsync();
            var response = request!.CreateResponse();
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            var errorMessage = new { InvocationId = context.InvocationId, Message = "An unhandled exception occurred. Please try again later", Exception = ex.Message };
            string responseBody = JsonSerializer.Serialize(errorMessage);

            await response.WriteStringAsync(responseBody);

            Console.WriteLine("Exception occurred");
            context.GetInvocationResult().Value = response;
        }
    }
}

public static class LoggingMiddlewareExtensions
{
    public static FunctionsApplicationBuilder UseLoggingMiddleware(
        this FunctionsApplicationBuilder app)
    {
        app.UseMiddleware<LoggingMiddleware>();

        return app;
    }
}
