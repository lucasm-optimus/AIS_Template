using Microsoft.Extensions.Configuration;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries;
using Tilray.Integrations.Services.Salesforce.Service.Queries;

namespace Tilray.Integrations.Functions.Usecase.SOXReport
{
    public class SOXReport_Rootstock
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly string _soxReportFilenamePrefix;
        private readonly string _reportDate;

        public SOXReport_Rootstock(ILoggerFactory loggerFactory, IMediator mediator, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<SOXReport_Rootstock>();
            _mediator = mediator;
            _soxReportFilenamePrefix = configuration["SOXReportFilenamePrefix"];
            _reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        [Function("SOXReport_RootStock")]
        public async Task Run([TimerTrigger("%SOXGenerationReportCron%")] TimerInfo myTimer)
        {
            _logger.LogInformation("Generating SOX Report");

            try
            {
                var soxReport = await _mediator.Send(new GenerateSOXReportQuery(_reportDate));
                if (soxReport == null)
                {
                    throw new Exception("SOX report generation failed.");
                }

                var csvFormat = await _mediator.Send(new ConvertToCSVFormatQuery(soxReport.Value));

                await _mediator.Send(new SaveReportToOneDriveCommand(csvFormat.Value, $"{_soxReportFilenamePrefix}_{_reportDate}.csv"));

                var entireQuery = SalesforceQueries.GetSOXReportQuery(_reportDate);
                await _mediator.Send(new SaveQueryToOneDriveCommand(entireQuery, $"{_soxReportFilenamePrefix}_{_reportDate}.txt"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during SOX Report generation: {ex.Message}");
            }
        }
    }
}
