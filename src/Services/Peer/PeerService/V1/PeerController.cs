using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Siccar.Network.Peers.Core;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Siccar.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Siccar.Network.PeerService.V1
{
    /// <summary>
    /// Peer Controller - provides basic interaction with a Peer in terms of understanding its operation
    /// and its state.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [SwaggerTag("Control surface for our local Network instance, routes data between Registers on Peers.", externalDocsUrl: "https://siccar.net/docs/Peers")]
    [Authorize]
    public class PeerController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        public const string PubSub = "pubsub";

        private readonly IPeerRouter router = null;

        /// <summary>
        /// PeerController Constructor with logger injection
        /// </summary>
        /// <param name="router">The System P2P Router</param>
        public PeerController(IPeerRouter router)
        {
            this.router = router;
        }
        
        /// <summary>
        /// Get details about this Peer
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Peer>> GetPeer()
        {
            return await Task.Run(() => Ok(this.router.Self));
        }

        /// <summary>
        /// Get and query what this Peer knows about its peers
        /// </summary>
        /// <returns></returns>
        [HttpGet("peers")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Peer>>> GetPeers()
        {
            return await Task.Run(() => Ok(this.router.ActivePeers));
        }

        /// <summary>
        /// When a peer posts to this peer it is requesting to link and join in the communications network
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> PostPeer(Peer requestor)
        {
            if (requestor.IPEndPoint == null)
            {
                if (Uri.CheckHostName(requestor.URIEndPoint.Host) == UriHostNameType.IPv4)
                {
                    requestor.IPEndPoint = IPAddress.Parse(requestor.URIEndPoint.Host); // actually should DNS resovle the IP 
                    requestor.IPSocket = requestor.URIEndPoint.Port;
                }
                else
                    requestor.IPEndPoint = IPAddress.Parse("0.0.0.0"); // Not a valid Socket
            }

            //if ( !((List<Peer>)router.ActivePeers).Contains<Peer>(requestor, new PeerComparer()) ) 
            // so we need to run some checks... and if we aint happy return a 404
            if (await router.AddPeer((Peer)requestor, true))  // force a refresh at the moment
                return Ok();

            // am considering if it should send a redirect if we could point it to a differnet peer

            return BadRequest("Not Added");
        }

        /// <summary>
        /// Adds support to host Register on this Network Peer
        /// </summary>
        /// <returns></returns>
        [HttpPost("HostRegister/{RegisterId}")]
        [Authorize(Roles = $"{Constants.InstallationAdminRole}")]
        public async Task<ActionResult> PostHostRegister(string RegisterId)
        {
            // ok there is a whole bunch of stuff requried here but just to get started...
            // todo check the register exisits and can be subscribed too
            if (!((List<string>)router.Self.Registers).Contains(RegisterId))
            {
                ((List<string>)router.Self.Registers).Add(RegisterId);

                // tell my peers
                await router.UpdatePeers();
                return Ok("Hosted");
            }
            else
                return BadRequest("Already Hosted");
        }
    }
}