using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Domain.Aggregates;

namespace Tilray.Integrations.Services.CSV.Service
{
    public class CSVService : ICSVService
    {
        public Result<string> GenerateCsv(List<AuditItem> payload)
        {
            if (payload == null || !payload.Any())
            {
                return Result.Fail("The provided payload is null or empty.");
            }

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,Section,Action,CreatedBy,CreatedDate,Display,DelegateUser,ResponsibleNamespacePrefix");

            foreach (var item in payload)
            {
                var (displayFormatted, username, createdDate, section, action, delegateUser, responsibleNamespacePrefix) = HandleNullValues(item);
                var csvLine = $"{item.Id},{section},{action},{username},{createdDate},{displayFormatted},{delegateUser},{responsibleNamespacePrefix}";
                csvBuilder.AppendLine(csvLine);
            }

            return Result.Ok(csvBuilder.ToString());
        }

        private (string displayFormatted, string username, string createdDate, string section, string action, string delegateUser, string responsibleNamespacePrefix) HandleNullValues(AuditItem item)
        {
            var displayFormatted = item.Display?.Replace("\n", "  ")
                .Replace("\t", "  ")
                .Replace(",", " ") ?? string.Empty;

            var username = item.CreatedBy?.Username ?? "Unknown";
            var createdDate = item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown";
            var section = item.Section ?? "Unknown";
            var action = item.Action ?? "Unknown";
            var delegateUser = item.DelegateUser ?? "Unknown";
            var responsibleNamespacePrefix = item.ResponsibleNamespacePrefix ?? "Unknown";

            return (displayFormatted, username, createdDate, section, action, delegateUser, responsibleNamespacePrefix);
        }
    }
}
