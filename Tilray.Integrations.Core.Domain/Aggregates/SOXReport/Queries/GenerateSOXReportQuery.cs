using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Tilray.Integrations.Core.Common;

namespace Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Queries
{
    public class GenerateSOXReportQuery(string reportDate) : IQuery<List<AuditItem>>
    {
        public string ReportDate { get; } = reportDate;
    }
}
