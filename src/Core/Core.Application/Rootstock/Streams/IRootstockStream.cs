using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Application.Rootstock.Stream
{
    public interface IRootstockStream
    {
        Task SendMessageAsync(string queue, string message);
        Task SendMessageAsync(string queue, IEnumerable<string> messages);
    }
}
