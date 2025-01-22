using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;

namespace Optimus.Core.Common.Middlewares;

public class ResultTypeFilter(ILogger<ResultTypeFilter> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            logger.LogDebug("[ResultTypeFilter][Pre Request]");

            await next(context);

            var result = context.GetInvocationResult().Value;
            var httpContext = context.GetHttpContext();

            if (result is IResult<object> resultObject)
            {
                if (!resultObject.IsSuccess)
                    result = ValidationProblemDetails(httpContext, StatusCodes.Status400BadRequest, resultObject);


                if (result != null)
                    result = Results.Ok(resultObject.Value);
                else
                    result = Results.NotFound(resultObject);
            }
            else
            {
                logger.LogInformation($"[ResultTypeFilter][Post Request][Type not managed][{result.GetType().Name}]");

            }
            context.GetInvocationResult().Value = result ?? NotFound(httpContext);
        }
        catch (Exception ex)
        {
            throw new FormatException("Error formating result", ex);
        }
    }

    protected ProblemDetails NotFound(HttpContext context, IResult<object> result = null)
    {
        var statusCode = StatusCodes.Status404NotFound;

        logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][{StatusText}]", typeof(Result).Name, Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode));

        return FormatProblemDetails(context, statusCode, result);
    }


    private ProblemDetails FormatProblemDetails(HttpContext context, int statusCode, IResult<object> result)
    {
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails()
        {
            Title = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode),
            Status = statusCode,
            Detail = result?.Errors?.FirstOrDefault()?.Message ?? ""
        };

        problem.Extensions.Add("traceId", context.TraceIdentifier);

        if (result is not null && result.Errors is not null)
            foreach (var error in result.Errors)
            {
                var errorMessages = error.Metadata.Select(x => x.Key?.ToString() ?? string.Empty).ToArray();
                problem.Extensions.Add(error.Message, errorMessages);
            }

        return problem;
    }


    private ProblemDetails ValidationProblemDetails(HttpContext context, int statusCode, IResult<object> result)
    {
        var errors = result.Errors.OfType<ExceptionalError>().ToList(); // ?.SelectMany(x => x.Reasons.OfType<ExceptionalError>()).ToList();
        if (errors.Any())
        {
            logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][Validation with exceptions]", result.GetType().Name);

            return FormatProblemDetails(context, StatusCodes.Status500InternalServerError, result);
        }

        logger.LogDebug("[ResultTypeFilter][Post Request][Type {RequestType}][Validation failed]", result.GetType().Name);

        context.Response.StatusCode = statusCode;

        var validationDetails = new ValidationProblemDetails()
        {
            Status = statusCode,
            Title = "Validation errors occurred",
            Detail = result.Errors?.FirstOrDefault()?.Message ?? ""
        };

        validationDetails.Extensions.Add("traceId", context.TraceIdentifier);

        foreach (var error in result.Errors)
        {
            var errorMessages = error.Metadata.Select(x => x.Key?.ToString() ?? string.Empty).ToArray();
            validationDetails.Errors.Add(error.Message, errorMessages);
        }

        return validationDetails;
    }
}