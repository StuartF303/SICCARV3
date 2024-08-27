using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Platform;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OData;
using Siccar.Registers.Core;
using Dapr;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Siccar.Platform.Registers.Core;
using Siccar.Common.Adaptors;
#nullable enable

namespace Siccar.Registers.RegisterService.V1
{
    /// <summary>
    /// API to create, control, query and act upon Registers Dockets
    /// </summary>
    [Authorize]
    [ApiController]
    [Route(Constants.RegisterAPIURL)]
    [Produces("application/json")]
    [SwaggerTag("Dockets are collections of Sealed Transactions", externalDocsUrl: "https://siccar.net/docs/Dockets")]

    public class DocketController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        private readonly IDaprClientAdaptor _daprClient;
        public const string StoreName = "statestore";
        public const string PubSub = "pubsub";
        private readonly ILogger<DocketController> Log;
        private readonly IRegisterRepository registerRepository;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        /// <summary>
        /// 
        /// </summary>
        public DocketController(ILogger<DocketController> logger, IRegisterRepository registerRepository, IDaprClientAdaptor daprClient)
        {
            Log = logger;
            this.registerRepository = registerRepository;
            _daprClient = daprClient;
        }

        /// <summary>
        /// Dockets - OData Query on Dockets
        /// </summary>
        /// <remarks>OData Query Dockets</remarks>
        /// <returns>One or more docket based on query</returns>
        [HttpGet("{RegisterId}/Dockets", Name = "GetDockets", Order = 0)]
        [ProducesResponseType(typeof(IEnumerable<Docket>), Status200OK)]
        [ProducesResponseType(typeof(string), Status404NotFound)]
        [EnableQuery(MaxTop = 50, AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Top | AllowedQueryOptions.Skip)]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<Docket>> GetDockets([FromRoute] string RegisterId)
        {
            try
            {
                if (String.IsNullOrEmpty(RegisterId))
                    return BadRequest("No Register Id");
                if (!await registerRepository.IsLocalRegisterAsync(RegisterId))
                    return NotFound();
            }
            catch (ODataException exception)
            {
                return BadRequest(exception.Message);
            }
			return Ok(await registerRepository.GetDocketsAsync(RegisterId));
		}

		/// <summary>
		/// Docket - Get by ID
		/// </summary>
		/// <remarks>
		/// Required a Header value that sets the correct registerId
		/// </remarks>
		/// <param name="RegisterId"></param>
		/// <param name="DocketId">The block height</param>
		/// <returns>The specified Docket</returns>
		[HttpGet("{RegisterId}/Dockets/{DocketId}", Name = "GetDocket", Order = 2)]
        [ProducesResponseType(typeof(Docket), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(MaxTop = 10, AllowedQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Top | AllowedQueryOptions.Skip)]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [Authorize(Roles = $"{Constants.WalletUserRole},{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<IActionResult> GetDocket([FromRoute] string RegisterId, [FromRoute] UInt64 DocketId)
        {
            
            var docket = await registerRepository.GetDocketAsync(RegisterId, DocketId);
            if (docket != null)
                return Ok(docket);
            return NotFound();
        }

        /// <summary>
        /// Docket - Allows autheticated peers to submit potential new Dockets
        /// </summary>
        /// <param name="RegisterId"></param>
        /// <param name="candidateDocket">The candidate block </param>
        /// <remarks>Put a Docket - Restricted to Peers</remarks>
        /// <returns>Not yet implemented</returns>
        [HttpPost("{RegisterId}/Dockets")]
        public async Task<IActionResult> PostDocket(string RegisterId, [FromBody] Docket candidateDocket)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                // OK this is just for the moment as we WOULD NOT WRITE THIS STRAIGH TO STORAGE!!!
                var submittedDocket = await registerRepository.InsertDocketAsync(candidateDocket); // we should return a realDocket if nolonger a candidate
                var reg = await registerRepository.GetRegisterAsync(RegisterId);
                reg.Height++; // coz we are agreeing to it being correct in consensus.
                await registerRepository.UpdateRegisterAsync(reg);
                // ^^^^^^^ TEMPORARY ^^^^^^

                var newUri = Constants.RegisterAPIURL + "/" + candidateDocket.RegisterId + "/Dockets/" + submittedDocket.Id;
                return Created(newUri, submittedDocket);
            }
        }

        /// <summary>
        /// Docket - Allows autheticated peers to submit potential new Dockets
        /// </summary>
        /// <remarks>Put a Docket - Restricted to Peers</remarks>
        /// <returns>Not yet implemented</returns>
        [Topic(PubSub, Topics.DocketConfirmedTopicName)]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [HttpPost("Dockets", Name = "NewHead", Order = 2)]
        public async Task<ActionResult<Docket>> ReceiveDocket(JsonDocument headDocket)
        {
            Docket head = new();

            //use SDK to publish the tx to a topic
            try
            {
                head = JsonSerializer.Deserialize<Docket>(headDocket.RootElement.GetRawText(), _jsonOptions)!;
            }
            catch (Exception er)
            {
                Log.LogError("Deserialisation Error : {err}", er.Message);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                // We have received from the validator only
                var submittedDocket = await registerRepository.InsertDocketAsync(head); // we should return a realDocket if nolonger a candidate
                var reg = await registerRepository.GetRegisterAsync(head.RegisterId);
                reg.Height++; // coz we are agreeing to it being correct in consensus.
                await registerRepository.UpdateRegisterAsync(reg);
                // ^^^^^^^ TEMPORARY ^^^^^^ - we need to better maintain the gloabl head 

                // now inform any local pariticpants about updates - we are currently calling it from Receive Transaction
                // InformLocalWallets(head.RegisterId, head.TransactionIds);

                var newUri = Constants.RegisterAPIURL + "/" + head.RegisterId + "/Dockets/" + submittedDocket.Id;
                return Created(newUri, submittedDocket);
            }
        }

        /// <summary>
        /// Docket - Allows autheticated peers to submit potential new Dockets
        /// </summary>
        /// <param name="DocketId"></param>
        /// <param name="candidateDocket">The candidate block </param>
        /// <remarks>Put a Docket - Restricted to Peers</remarks>
        /// <returns>Not yet implemented</returns>
        [HttpPut("{RegisterId}/Dockets/{DocketId}")]
        //        [Authorize(Policy = "Peers")]

        public async Task<IActionResult> PutDocket(string DocketId, [FromBody] Docket candidateDocket)
        {
            return await Task.Run(() => BadRequest("Not yet implemented"));
        }

        private async Task InformLocalWallets(string registerId, List<string> txIds)
        {
            // for each transcation send an update message to update awaiting clients
            // a Parallel.Foreach would be nice but whilest we get the basic right...

            foreach (var txId in txIds)
            {
                var tx = await registerRepository.GetTransactionAsync(registerId, txId);

                if (tx == null)
                    Log.LogWarning("WARN: Docket Received with TX not present : {registerId} : {txId}", registerId, txId);
                else
                {
                    var msg = new TransactionConfirmed()
                    {
                        TransactionId = txId,
                        MetaData = tx.MetaData,
                        PreviousTransactionId = tx.PrevTxId,
                        Sender = tx.SenderWallet,
                        ToWallets = tx.RecipientsWallets.ToList()
                    };
                    //Publish Transaction Update Event
                    await _daprClient.PublishEventAsync(RegisterConstants.PubSub, Topics.TransactionConfirmedTopicName, msg);
                }
            }
        }
    }
}
