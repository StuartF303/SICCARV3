using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Siccar.Network.Peers.Core;

namespace Siccar.Network.PeerService.Hubs
{

    /// <summary>
    /// A SignalR Service with local Peer info 
    /// </summary>
    public class AdminHub : Hub
    {

        private readonly IPeerRouter _router = null;
        private readonly ILogger Log = null;

        public AdminHub(IPeerRouter Router, ILogger<AdminHub> logger)
        {
            _router = Router;
            Log = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            Log.LogInformation("New Connection : {0} ", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }


        public string HubName()
        {
            return _router.Self.Id;
        }

        public async Task DirectMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task GetConnectionId()
        {
            return Clients.Caller.SendAsync("ReceiveMessage", Context.ConnectionId);
        }
    }
}