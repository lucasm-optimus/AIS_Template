using FluentResults;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.SAPConcur.Services;
using Tilray.Integrations.Services.SAPConcur.Startup;

namespace Tilray.Integrations.Services.SAPConcur.Service
{
    public class SAPConcurService(HttpClient client, SAPConcurSettings sapConcurSettings, ILogger<SAPConcurService> logger) : ISAPConcurService
    {
        public Task<Result<IEnumerable<EcomSalesOrder>>> GetSalesOrders()
        {
            // Implement http client to get sales orders from SAP Concur
            throw new NotImplementedException();
        }
    }
}
