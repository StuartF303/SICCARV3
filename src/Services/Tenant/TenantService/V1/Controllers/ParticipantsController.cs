#nullable enable
using System;
using Microsoft.AspNetCore.Mvc;
using Siccar.Application;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Siccar.Platform.Tenants.V1.Controllers
{
    [Produces("application/json")]
    public class ParticipantsController : ODataController
    {
        private readonly IWalletServiceClient _walletServiceClient;
        private readonly IRegisterServiceClient _registerServiceClient;

        public ParticipantsController(
            IWalletServiceClient walletServiceClient,
            IRegisterServiceClient registerServiceClient
            
            )
        {
            _registerServiceClient = registerServiceClient;
            _walletServiceClient = walletServiceClient;
        }

        /// <summary>
        /// Gets the Transactions as specified by the RegsiterId.
        /// This could be alot of data so needs to be properly paged
        /// </summary>
        /// <returns>Participants information.</returns>
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("({ParameterValue})",Name = "GetParticipants", Order = 1)]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<IQueryable<Participant>>> GetParticipants([FromRoute] string ParameterValue)
        {

            return Ok((await GetPublishedParticipantsBase(ParameterValue)).AsQueryable());
        }

        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        [HttpGet("({ParameterValue})", Name = "GetParticipantById", Order = 2)]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = $"{Constants.RegisterCreatorRole},{Constants.RegisterMaintainerRole},{Constants.RegisterReaderRole}")]
        public async Task<ActionResult<Participant>> GetParticipantById([FromRoute] string ParameterValue, string key)
        {

            var transactions = await _registerServiceClient.GetParticipantTransactions(ParameterValue);

            if (transactions == null)
            {
                return NotFound(new Participant());
            }

            var payloadTasks = transactions.Select(transaction => _walletServiceClient.GetAccessiblePayloads(transaction));
            var payloads = (await Task.WhenAll(payloadTasks)).ToList();

            foreach (var payload in payloads)
            {
                var jsonString = Encoding.UTF8.GetString(payload[0]) ?? "{}";
                var participant = JsonSerializer.Deserialize<Participant>(jsonString);
                if (participant != null)
                {
                    if (participant.Id == key)
                    { 
                        return Ok(participant);
                    };
                }
            }
            return NotFound();
        }

        [ODataIgnored]
        private async Task<List<Participant>> GetPublishedParticipantsBase(string registerId)
        {
            var transactions = await _registerServiceClient.GetParticipantTransactions(registerId);

            if (transactions == null)
            {
                return new List<Participant>();
            }

            var payloadTasks = transactions.Select(transaction => _walletServiceClient.GetAccessiblePayloads(transaction));
            var payloads = (await Task.WhenAll(payloadTasks)).ToList();

            var participants = new List<Participant>();

            foreach (var payload in payloads)
            {
                var jsonString = Encoding.UTF8.GetString(payload[0]) ?? "{}";
                var participant = JsonSerializer.Deserialize<Participant>(jsonString);
                if (participant != null)
                {
                    participants.Add(participant);
                }
            }
            return participants;
        }
    }
}