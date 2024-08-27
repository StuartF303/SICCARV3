using Siccar.Network.Peers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Threading;
using Siccar.Network.Router;
using Dapr.Client;
using Microsoft.AspNetCore.SignalR;

namespace Siccar.Network.Router.ConnectionHandlers
{
    /// <summary>
    /// WebAPIConneciton Handler uses SignalR and HTTP Methods to
    /// act as a transport between peers
    /// </summary>
    public class WebAPIConnectionHandler : TransportHandlerBase, IDisposable
    {
        //public string ConnectionId = "";

        public IHttpClientFactory _httpFactory = null;

        public IHubContext<PeerHub> _hubContext = null;

        public HubConnection _hubConnection = null;

        //private readonly JsonSerializerOptions jsOpts = new(JsonSerializerDefaults.Web);

        private readonly IPeerRouter _myRouter = null;


        /// <summary>
        /// We was born of Factory
        /// </summary>
        /// <param name="rounter"></param>
        /// <param name="httpFactory"></param>
        /// <param name="PeerHub"></param>
        /// <param name="logger"></param>
        public WebAPIConnectionHandler(IPeerRouter router,
            IHttpClientFactory httpFactory, IHubContext<PeerHub> PeerHub, DaprClient daprClient, ILogger<WebAPIConnectionHandler> logger) : base(router, daprClient, logger)
        {
            _hubContext = PeerHub;
            _httpFactory = httpFactory;
            _myRouter = router;
        }

        public override async Task OpenConnection(Peer remotePeer)
        {
            RemotePeer = remotePeer;
            Log.LogDebug("[P2P] Requesting to Join from {name}", RemotePeer.PeerName);

            var remoteHub = RemotePeer.URIEndPoint.AbsoluteUri + PeerConstants.PeerHubUri;

            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(remoteHub, (srOpts) =>
                    {
                        srOpts.HttpMessageHandlerFactory = (message) =>
                        {
                            if (_myRouter.Self.DisabledSSL)  // todo: We should only be doignt his in debug
                                if (message is HttpClientHandler clientHandler)
                                    // bypass SSL certificate checking 
                                    clientHandler.ServerCertificateCustomValidationCallback +=
                                        (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    })
                    .ConfigureLogging(c =>
                    {
                        c.SetMinimumLevel(LogLevel.Debug);
                    })
                    .Build();

            //   _RoutingSignalRTable.Add(_remotePeer.Id.ToString(), cn);
            _hubConnection.On<PeerMessage>("ReceiveMessage", (msg) =>
           {
               Task.Run(async () => { await base.ReceivedMessage(msg); });
           });

            //_hubConnection.On<string>("ReceiveReply", (msg) =>
            //{
            //    Task.Run(async () => { await ReceivedMessage(msg); });
            //});
            //_hubConnection.On<PeerMessage>("ReceiveMessage", (rx) =>
            //{
            //    Task.Run(async () => { await ReceivedTransaction(rx); });
            //});
            _hubConnection.On<Peer, PeerStatus>("Update", (remotePeer, status) =>
            {

                Log.LogDebug("[P2P] Update Requested : {name} ", remotePeer.PeerName);
                RemotePeer = router.ProbePeer(remotePeer).Result;
                //Task.Run(async () => { await ReceivedDocket(dx); });
            });
            _hubConnection.On<string>("Disconnect", (msg) =>
            {
                Log.LogWarning("[P2P] Connection Closed Remotely : {name} ", remotePeer.PeerName);

                this._hubConnection.StopAsync();

                _ = this.CloseConnection();

            });
            _hubConnection.Closed += async (error) =>
            {

                Log.LogWarning("[P2P] Connection Closed : {name} : {msg}", remotePeer.PeerName, error.Message);

                await CloseConnection();

            };

            try
            {
                await _hubConnection.StartAsync();
                remotePeer.Status = PeerStatus.Connected;
                this.Id = _hubConnection.ConnectionId;

                Log.LogDebug("[P2P] Successful request to connect : {name}  myID : {id}", remotePeer.PeerName, this.Id);
            }
            catch (HttpRequestException ex)
            {
                Log.LogInformation("[P2P] UNSUCCESSFUL requesting connection : {name} : {msg}", remotePeer.PeerName, ex.Message);
            }
            catch (TaskCanceledException taskErr)
            {
                Log.LogInformation("[P2P] UNSUCCESSFUL requesting connection : {name} : {msg} ", remotePeer.PeerName, taskErr.Message);
            }
        }

        public new Task<int> QueryConnection()
        {
            throw new NotImplementedException();
        }

        public override async Task UpdateConnection()
        {
            await this._hubConnection.SendAsync("Update", base.router.Self, PeerStatus.Connecting);
        }

        public override async Task SendMessage(PeerMessage message)
        {
            await this._hubConnection.SendAsync("ReceiveMessage", message);
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
            this._hubConnection.SendAsync("Disconnect");
            router = null;
            _httpFactory = null;
            _hubConnection = null;
            Log = null;
        }
    }
}
