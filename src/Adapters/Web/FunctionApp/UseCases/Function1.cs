using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp.UseCases
{
    public class ChangeUserName(ILogger<ChangeUserName> logger)
    {
        [Function("ChangeUserName")]
        public Result<object> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");
            return Result.Ok((object)new { Test = "C# HTTP trigger function processed a request." });
        }
    }
}
