using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapr;
using Dapr.Client;
using System.Linq;
using Siccar.Common;
using Siccar.Platform;
using Siccar.Network.Peers.Core;
using System.Text.Json;

namespace Siccar.Network.PeerService.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        public const string PubSub = "pubsub";
        private readonly ILogger<TransportController> Log = null;
        private IPeerRouter _router = null;
        private DaprClient _daprClient;

        /// <summary>
        /// The Communications Controller passes events between the Register Services (via Events) to 
        /// specific ctransport handlers which manage the peer connections
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="Router"></param>
        /// <param name="daprClient"></param>
        public TransportController(ILogger<TransportController> logger, IPeerRouter Router, DaprClient daprClient)
        {
            this.Log = logger;
            this._router = Router;
            this._daprClient = daprClient;
        }


        /// <summary>
        /// Retrieves a Transaction from this peer as specified by Register by the Transaction Id.
        /// </summary>
        /// <returns>Peer information. ***initial skeleton test only***</returns>
        [HttpGet("{RegisterId}/{TransactionId}")]
        public ActionResult<TransactionModel> Get(string RegisterId, string TransactionId)
        {
            // We are sending a remote host a current transaction - it might be in-flight or confirmed
            // that is it could be mempool or from the 

            if (!Request.Headers.TryGetValue("PeerId", out var PeerId))
            {
                Log.LogWarning("[P2P] Remote peer {0} did not provide a proper PeerID Header", Request.Host.Value);
                return BadRequest("Missing Proper Headers (PeerId)");
            }

            // first do we actually host the register the remote host is asking for? Coz it should not be asking!
            if (!_router.Self.Registers.Contains(RegisterId))
            {
                Log.LogWarning("[P2P] Remote {0} requested unhosted Register {1}", PeerId.First(), RegisterId);
                return NotFound();
            }

            

            return new TransactionModel();

        }

        /// <summary>
        /// Method for transmission of a new transaction.
        /// A new transaction is recevied from the message bus or posted directly 
        /// and we need to pass it on...
        /// </summary>
        /// <param name="transaction">Transaction info.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic(PubSub, Topics.TransactionPendingTopicName)]
        [HttpPost("transaction")]
        public async Task<ActionResult> Transaction(TransactionModel transaction)
        {


            //use SDK to publish the tx to a topic
           // no we received it  await daprClient.PublishEventAsync<Transaction>(PubSub, Topics.TransactionSubmittedTopicName, transaction);
            PeerMessage pm = new PeerMessage()
            {
                TxId = transaction.TxId,
                TxBody = JsonSerializer.Serialize<TransactionModel>(transaction) 
            };

            await _router.BroadcastMessage(transaction.MetaData.RegisterId, pm);

            return Ok();
        }

    }
}
