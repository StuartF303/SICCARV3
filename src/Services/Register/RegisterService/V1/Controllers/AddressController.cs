using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
#nullable enable

namespace Siccar.Registers.RegisterService.V1
{
    /// <summary>
    /// Address Controller - maywell end up as its own Service
    /// takes a Post of a Wallet Address and the associated WalletID so when
    /// an inbound transaction is received an event can be raised.
    /// </summary>
    /// <summary>
    /// API to create, control, query and act upon Registers 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route(Constants.RegisterAPIURL)]

    // [ODataRoutePrefix(Constants.RegisterAPIURL)]
    [Produces("application/json")]
    [SwaggerTag("The Register Service needs informed of Local Wallet Addresses", externalDocsUrl: "https://siccar.net/docs/Address")]
    public class AddressController : ControllerBase
    {
        public const string StoreName = "registerstore";
        public const string PubSub = "pubsub";
        private readonly IDaprClientAdaptor _daprClient;


        public AddressController(IDaprClientAdaptor clientAdaptor) { _daprClient = clientAdaptor; }

        ///// <summary>
        ///// Get Address - Subscribe to events for a given Address 
        ///// </summary>
        ///// <remarks>
        /////  - OData Query Address
        /////  - Authentication Specific
        ///// </remarks>
        ///// <returns>One or more address based on query</returns>
        //[HttpGet("{RegisterId}/Address", Name = "GetAddress", Order = 0)]
        //[ProducesResponseType(typeof(IEnumerable<string>), Status200OK)]
        //[ProducesResponseType(typeof(string), Status404NotFound)]
        //public async Task<ActionResult<Docket>> GetAddresses([FromRoute] string RegisterId)
        //{
        //    try
        //    {
        //        // check the register is local first -- assumin ok for the moment

        //        if (String.IsNullOrEmpty(RegisterId))
        //            return BadRequest("No Register Id");

        //        //if (!await registerRepository.IsLocalRegisterAsync(RegisterId))
        //        //    return NotFound();


        //    }
        //    finally
        //    {

        //    }
        //    return Ok( );
        //}


        /// <summary>
        /// Post New Address - Called when a Wallet creates an new address, so as to locate local address/events
        /// </summary>
        /// <param name="newAddress"></param>
        /// <remarks>Put a Docket - Restricted to Peers</remarks>
        /// <returns>Not yet implemented</returns>
        [Topic(PubSub, Topics.WalletAddressCreationTopicName)]
        [Authorize(Policy = AuthenticationDefaults.DaprAuthorizationPolicy)]
        [HttpPost("Addresses")]
        public async Task<IActionResult> PostLocalAddress( WalletAddress newAddress)
        {
            await _daprClient.SaveStateAsync(StoreName, newAddress.Address, newAddress);
            return Ok();
        }
    }
}
