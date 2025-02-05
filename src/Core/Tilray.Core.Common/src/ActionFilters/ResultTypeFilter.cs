namespace Tilray.Integrations.Core.Common.ActionFilters;

public class MinimumResultTypeFilter(ILogger<MinimumResultTypeFilter> logger) : IEndpointFilter, IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var content = new Dictionary<string, OpenApiMediaType>
        {
            ["application/json"] = new OpenApiMediaType
            {
                Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
            }
        };

        operation.Responses.TryAdd("400", new OpenApiResponse
        {
            Description = "Bad Request",
            Content = content
        });

        operation.Responses.TryAdd("404", new OpenApiResponse
        {
            Description = "Not found",
            Content = content
        });

        operation.Responses.TryAdd("500", new OpenApiResponse
        {
            Description = "Server error",
            Content = content
        });
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogDebug("[ResultTypeFilter][Pre Request]");

        var result = await next(context);

        if (context.HttpContext.Response.HasStarted)
            return null;

        if (result is IResult<object> resultObject)
        {
            if (!resultObject.IsSuccess)
                return ValidationProblemDetails(context, StatusCodes.Status400BadRequest, resultObject);

            return resultObject.Value ?? NotFound(context, resultObject);
        }

        logger.LogInformation($"[ResultTypeFilter][Post Request][Type not managed][{result.GetType().Name}]");
        return result ?? NotFound(context);
    }

    protected ProblemDetails NotFound(EndpointFilterInvocationContext context, IResult<object> result = null)
    {
        var statusCode = StatusCodes.Status404NotFound;

        logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][{StatusText}]", typeof(Result).Name, Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode));

        return FormatProblemDetails(context, statusCode, result);
    }


    private ProblemDetails FormatProblemDetails(EndpointFilterInvocationContext context, int statusCode, IResult<object> result)
    {
        context.HttpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails()
        {
            Title = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode),
            Status = statusCode,
            Detail = result?.Errors?.FirstOrDefault()?.Message ?? ""
        };

        problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

        if (result is not null && result.Errors is not null)
            foreach (var error in result.Errors)
            {
                var errorMessages = error.Metadata.Select(x => x.Key?.ToString() ?? string.Empty).ToArray();
                problem.Extensions.Add(error.Message, errorMessages);

                //// TODO: Implement a way to show exceptions
                //var reasons = error.Reasons.OfType<ExceptionalError>();
                //if (reasons.Any(x => x.Exception != null))
                //    foreach (var reason in reasons)
                //    {
                //        var ex = reason.Exception;
                //        problem.Extensions.Add("Exception", JsonSerializer.Serialize(new
                //        {
                //            Message = reason.Exception.Message,
                //            StackTrace = reason.Exception.StackTrace,
                //            InnerException = new
                //            {
                //                Message = reason?.Exception?.InnerException?.Message ?? ""
                //            }
                //        }));
                //    }
            }

        return problem;
    }


    private ProblemDetails ValidationProblemDetails(EndpointFilterInvocationContext context, int statusCode, IResult<object> result)
    {
        var errors = result.Errors.OfType<ExceptionalError>().ToList(); // ?.SelectMany(x => x.Reasons.OfType<ExceptionalError>()).ToList();
        if (errors.Any())
        {
            logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][Validation with exceptions]", result.GetType().Name);

            return FormatProblemDetails(context, StatusCodes.Status500InternalServerError, result);
        }

        logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][Validation failed]", result.GetType().Name);

        context.HttpContext.Response.StatusCode = statusCode;

        var validationDetails = new ValidationProblemDetails()
        {
            Status = statusCode,
            Title = "Validation errors occurred",
            Detail = result.Errors?.FirstOrDefault()?.Message ?? ""
        };

        validationDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

        foreach (var error in result.Errors)
        {
            var errorMessages = error.Metadata.Select(x => x.Key?.ToString() ?? string.Empty).ToArray();
            validationDetails.Errors.Add(error.Message, errorMessages);
        }

        return validationDetails;
    }
}
