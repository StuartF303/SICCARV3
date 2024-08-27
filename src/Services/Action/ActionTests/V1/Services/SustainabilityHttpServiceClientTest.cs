using ActionService.V1.Services;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using static ActionService.V1.Services.SustainabilityHttpServiceClient;

namespace ActionUnitTests.V1.Services
{
    public class SustainabilityHttpServiceClientTest
    {
        private ISustainabilityHttpServiceClient _underTest;
        private readonly IHttpClientAdaptor _fakeHttpClientAdaptor;
        private readonly IConfiguration _configuration;
        private const string _connectorEndpoint = "https://someendpoint.com";
        private const string _walletAddress = "ws1kajdsgfasijklksdfadsaeg";

        public SustainabilityHttpServiceClientTest()
        {
            var configDef = new Dictionary<string, string>
            {
                {"MakeSusMgrRequests", "true" },
                {"WalletAddress", _walletAddress },
                {"SusConnectorHttpEndpoint", "https://someendpoint.com" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDef)
                .Build();

            _fakeHttpClientAdaptor = A.Fake<IHttpClientAdaptor>();
            _underTest = new SustainabilityHttpServiceClient(_configuration, _fakeHttpClientAdaptor);
        }

        public class SendPostRequest : SustainabilityHttpServiceClientTest
        {
            [Fact]
            public async Task Should_Call_ClientAdaptorWhen_MakeSusMgrReequests_True()
            {
                var payload = new TransactionConfirmed() { MetaData = new TransactionMetaData(), ToWallets = new List<string>{ _walletAddress } };
                var requestdata = new SusConnectorHttpReqModel
                {
                    previousTxId = payload.TransactionId,
                    registerId = payload.MetaData.RegisterId,
                    walletAddress = _walletAddress
                };

                var expected = JsonSerializer.Serialize(requestdata, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                await _underTest.SendPostRequest(payload);

                A.CallTo(() => _fakeHttpClientAdaptor.PostAsync(_connectorEndpoint, expected)).MustHaveHappenedOnceExactly();
            }


            [Fact]
            public async Task Should_NotCall_ClientAdaptorWhen_MakeSusMgrReequests_False()
            {
                var payload = new TransactionConfirmed() { MetaData = new TransactionMetaData(), ToWallets = new List<string>{ _walletAddress } };
                var requestdata = new SusConnectorHttpReqModel
                {
                    previousTxId = payload.TransactionId,
                    registerId = payload.MetaData.RegisterId,
                    walletAddress = _walletAddress
                };

                var expected = JsonSerializer.Serialize(requestdata, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                var configDef = new Dictionary<string, string>
                {
                    { "MakeSusMgrRequests", "false" },
                    { "WalletAddress", _walletAddress },
                    { "SusConnectorHttpEndpoint", "https://someendpoint.com" }
                };

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(configDef)
                    .Build();
                _underTest = new SustainabilityHttpServiceClient(config, _fakeHttpClientAdaptor);

                await _underTest.SendPostRequest(payload);

                A.CallTo(() => _fakeHttpClientAdaptor.PostAsync(_connectorEndpoint, expected)).MustNotHaveHappened();
            }

            [Fact]
            public async Task ShouldNotCall_WhenToWalletsDoesNotContainTargetWallet()
            {
                var payload = new TransactionConfirmed() { MetaData = new TransactionMetaData(), ToWallets = new List<string>() };
                var requestdata = new SusConnectorHttpReqModel
                {
                    previousTxId = payload.TransactionId,
                    registerId = payload.MetaData.RegisterId,
                    walletAddress = _walletAddress
                };

                var expected = JsonSerializer.Serialize(requestdata, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                await _underTest.SendPostRequest(payload);

                A.CallTo(() => _fakeHttpClientAdaptor.PostAsync(_connectorEndpoint, expected)).MustNotHaveHappened();
            }

        }
    }
}
