using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Siccar.Registers.RegisterService.Hubs;
#nullable enable

namespace Siccar.Registers.RegisterService.V1.Adapters
{
    public class RegisterServiceHubContextAdapter : IHubContextAdapter
    {
        private readonly IHubContext<RegistersHub> _hubContext;
        public RegisterServiceHubContextAdapter(IHubContext<RegistersHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToGroupAsync(string group, string method, object arg1)
        {
            await _hubContext.Clients.Groups(group).SendAsync(method, arg1);
        }
        public async Task SendToAllAsync(string method, object arg1)
        {
            await _hubContext.Clients.All.SendAsync(method, arg1);
        }
    }
}
