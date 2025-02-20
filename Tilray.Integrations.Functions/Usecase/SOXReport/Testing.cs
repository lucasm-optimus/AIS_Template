using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates;

namespace Tilray.Integrations.Functions.Usecase.SOXReport
{
    public class Testing
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly string _soxReportFilenamePrefix;
        private readonly string _reportDate;

        public Testing(ILoggerFactory loggerFactory, IMediator mediator, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Testing>();
            _mediator = mediator;
            _soxReportFilenamePrefix = configuration["SOXReportFilenamePrefix"]; 
            _reportDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        [Function("Testing")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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
