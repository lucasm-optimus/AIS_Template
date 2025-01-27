using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Optimus.FunctionApp.UseCases
{
    public class Function2
    {
        private readonly ILogger<Function2> _logger;

        public Function2(ILogger<Function2> logger)
        {
            _logger = logger;
        }

        [Function("Function2")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
