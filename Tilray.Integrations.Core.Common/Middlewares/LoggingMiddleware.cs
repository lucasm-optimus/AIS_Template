namespace Tilray.Integrations.Core.Common.Middlewares;

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
                if (System.Diagnostics.Activity.Current != null)
                {
                    foreach (var item in state)
                    {
                        System.Diagnostics.Activity.Current.AddBaggage(item.Key, item.Value.ToString());
                        System.Diagnostics.Activity.Current.AddTag(item.Key, item.Value);
                    }
                }

                var httpContext = context.GetHttpContext();

                if (httpContext != null)
                {
                    httpContext.Response.Headers.Append("TraceId", context.TraceContext.TraceParent);
                    logger.LogDebug("[Web][Request][{method} {endpoint}]", httpContext.Request.Method, httpContext.Request.Path);
                }
                else
                {
                    logger.LogDebug("InvocationId: {InvocationId}, TraceId: {TraceId}", context.InvocationId, context.TraceContext.TraceParent);
                }

                await next(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            var errorMessage = new
            {
                context.InvocationId,
                Message = "An unhandled exception occurred. Please try again later",
                Exception = ex.Message
            };

            var request = await context.GetHttpRequestDataAsync();
            if (request != null)
            {
                var response = request.CreateResponse();
                response.StatusCode = HttpStatusCode.InternalServerError;

                string responseBody = JsonSerializer.Serialize(errorMessage);
                await response.WriteStringAsync(responseBody);

                context.GetInvocationResult().Value = response;
            }
            else
            {
                context.GetInvocationResult().Value = errorMessage;
            }
        }
    }
}
