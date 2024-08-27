using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Siccar.Registers.RegisterService.Hubs
{
    [Authorize]
    public class RegistersHub : Hub
    {
        private readonly ILogger<RegistersHub> _logger;
        private const string GroupName = "registerClients";

        public RegistersHub(ILogger<RegistersHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // If we don't get a query string on connection, handle cleanly by setting a default value
            var ctx = Context.GetHttpContext();
            if (ctx == null)
            {
                return;
            }

            var registerId = ctx.Request.Query["registerId"].ToString();
            var group = string.IsNullOrEmpty(registerId) ? GroupName : registerId;

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            _logger.LogDebug("SignalR Connection {id}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception != null)
            {
                _logger.LogWarning("Faulted : {msg}", exception.Message);
            }
            else
            {
                _logger.LogDebug("Disconnect ConnectionId: {id}", Context.ConnectionId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeRegister(string registerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, registerId);
        }

        public async Task UnSubscribeRegister(string registerId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, registerId);
        }
    }
}
