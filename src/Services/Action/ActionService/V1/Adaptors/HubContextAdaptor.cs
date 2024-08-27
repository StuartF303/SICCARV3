using ActionService.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ActionService.V1.Adaptors
{
    public class HubContextAdaptor : IHubContextAdaptor
    {
        private readonly IHubContext<ActionsHub> _hubContext;
        public HubContextAdaptor(IHubContext<ActionsHub> hubContext)
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
