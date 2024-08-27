using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siccar.Registers.ValidationEngine
{
    public class Genesys
    {
        public const string StoreName = "statestore";
        public const string PubSubName = "pubsub";
        private readonly ILogger<Genesys> _logger;
        private readonly IDaprClientAdaptor Client;
        private readonly DocketBuilder Builder;
        private readonly string DocketHash = "0000000000000000000000000000000000000000000000000000000000000000";

        public Genesys(ILogger<Genesys> logger, DocketBuilder builder, IDaprClientAdaptor client)
        {
            _logger = logger;
            Builder = builder;
            Client = client;
        }

        public async Task ProcessGenesys(string RegisterId)
        {
            _logger.LogInformation("Performing Genesys Creation for RegisterId : {id}", RegisterId);
            var docket = Builder.GenerateDocket(new Docket()
            {
                Id = 0,
                RegisterId = RegisterId,
                PreviousHash = DocketHash,
                TransactionIds = new List<string>(),
                TimeStamp = DateTime.UtcNow,
                State = DocketState.Sealed
            });
            _logger.LogInformation("[GENESYS] Register: {id}, Signatries Count {count}", docket.RegisterId, docket.TransactionIds.Count);

            // so we should push this out to the network and consensus but for the moment ram them home... 
            await Client.PublishEventAsync(PubSubName, Topics.DocketConfirmedTopicName, docket);
        }
    }
}
