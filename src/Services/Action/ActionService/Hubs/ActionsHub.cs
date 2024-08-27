using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Siccar.Platform;
using Microsoft.AspNetCore.Authorization;

namespace ActionService.Hubs
{
    [Authorize]
    public class ActionsHub : Hub
    {
        private readonly ILogger _logger;
        private const string groupName = "actionClients";

        public ActionsHub(ILogger<ActionsHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// On Connection Handler
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            // if we dont get a query string on connection, handle cleanly by setting a default value
            var ctx = Context.GetHttpContext();
            var group = string.IsNullOrEmpty(ctx.Request.Query["walletAddress"].ToString()) ? groupName : ctx.Request.Query["walletAddress"].ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            _logger.LogDebug("[ACTIONHUB] SignalR Connection {id}", Context.ConnectionId);

            await base.OnConnectedAsync();
        }
 
        /// <summary>
        /// Subscribes to evetnf for a wallet
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeWallet(string walletAddress)
        {
            // TODO:check the user has access to the address
            await Groups.AddToGroupAsync(Context.ConnectionId, walletAddress);

        }
        /// <summary>
        /// Unsubscribes to events for a wallet
        /// </summary>
        /// <returns></returns>
        public async Task UnSubscribeWallet(string walletAddress)
        {
            // TODO:check the user has access to the address
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, walletAddress);

        }

        /// <summary>
        /// On Disconnection Handler
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            if (exception != null)
                _logger.LogWarning("[ACTIONHUB] Faulted : {msg}", exception.Message ?? "Remote Close");
            else
                _logger.LogDebug("[ACTIONHUB] Disconnect ConnectionId: {id}", Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
