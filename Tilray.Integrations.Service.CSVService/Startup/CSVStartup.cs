using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tilray.Integrations.Core.Application.Adapters.Service;
using Tilray.Integrations.Core.Common.Startup;
using Tilray.Integrations.Services.CSV.Service;

namespace Tilray.Integrations.Services.CSV.Startup
{
    public class CSVStartup : IStartupRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICSVService, CSVService>();
            return services;
        }
    }
}
