namespace Tilray.Integrations.Core.Common.Middlewares;

/// <summary>
/// This middleware catches any exceptions during function invocations and
/// returns a response with 500 status code for http invocations.
/// </summary>
internal sealed class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing invocation {InvocationId}", context.InvocationId);

            var httpReqData = await context.GetHttpRequestDataAsync();

            if (httpReqData != null)
            {
                var problem = new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = ex.Message,
                    Title = "Error processing your request"
                };

                // Create an instance of HttpResponseData with 500 status code.
                var newHttpResponse = httpReqData.CreateResponse(HttpStatusCode.InternalServerError);
                await newHttpResponse.WriteAsJsonAsync(problem);

                var invocationResult = context.GetInvocationResult();

                var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
                if (httpOutputBindingFromMultipleOutputBindings is not null)
                    httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
                else
                    invocationResult.Value = newHttpResponse;
            }
        }
    }

    private OutputBindingData<HttpResponseData> GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context)
    {
        // The output binding entry name will be "$return" only when the function return type is HttpResponseData
        var httpOutputBinding = context.GetOutputBindings<HttpResponseData>()
            .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");

        return httpOutputBinding;
    }
}
