using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Domain.Aggregates;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries;

namespace Tilray.Integrations.Functions.UseCases.SOXReport.Rootstock
{


    public class GetAuditItemsFromRootstock(IMediator mediator)
    {
        /// <summary>
        /// This function is responsible for fetching invoices from SAP Concur.
        /// </summary>
        [Function("GetAuditItemsFromRootstock")]
        [ServiceBusOutput(Topics.SOXAuditItemsFetched, Connection = "ServiceBusConnectionString")]
        public async Task<SOXReportAgg> Run([TimerTrigger("%GetAuditItemsFromRootstockCRON%")] TimerInfo myTimer)
        {
            var result = await mediator.Send(new GenerateSOXReportQuery());
            if (result.IsSuccess)
            {
                return result.Value;
            }

            return new SOXReportAgg();
        }
    }
}