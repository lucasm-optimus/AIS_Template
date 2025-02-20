using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Tilray.Integrations.Core.Common;

namespace Tilray.Integrations.Core.Domain.Aggregates.SOXReport.Commands
{
    public class SaveQueryToOneDriveCommand(string query, string fileName) : ICommand
    {
        public string Query { get; } = query;
        public string FileName { get; } = fileName;
    }
}
