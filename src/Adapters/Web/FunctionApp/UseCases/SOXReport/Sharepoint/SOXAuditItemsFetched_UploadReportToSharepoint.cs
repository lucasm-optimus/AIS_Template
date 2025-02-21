using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Application.Constants;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;
using Tilray.Integrations.Functions.UseCases.Invoices.Sharepoint;
using Tilray.Integrations.Services.Rootstock.Service.Models;

namespace Tilray.Integrations.Functions.UseCases.SOXReport.Sharepoint
{
    public class SOXAuditItemsFetched_UploadReportToSharepoint(IMediator mediator)

    {

        [Function(nameof(SAPConcurInvoicesFetched_UploadInvoicesToSharepoint))]
        public async Task Run(
           [ServiceBusTrigger(Topics.SOXAuditItemsFetched, Subscriptions.UplaodSOXAuditItemsToSharepoint, Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
           ServiceBusMessageActions messageActions)
        {

            var AuditReportAgg = message.Body.ToString().ToObject<SOXReportAgg>();
            try
            {
                var uploadedReport= await mediator.Send(new UploadReportToOneDriveCommand(AuditReportAgg.ReportItems));
                if (uploadedReport.IsFailed)
                {
                    await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(uploadedReport.Errors));
                }

                var uploadedQuery = await mediator.Send(new UploadQueryToOneDriveCommand(AuditReportAgg.Query));

                if(uploadedQuery.IsFailed)
                {
                    await messageActions.DeadLetterMessageAsync(message, deadLetterReason: Helpers.GetErrorMessage(uploadedQuery.Errors));
                }
            }
            catch (Exception ex)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, deadLetterErrorDescription: ex.InnerException?.Message);
                throw;
            }
        }
    }
}
