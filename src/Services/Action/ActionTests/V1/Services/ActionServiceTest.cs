
using ActionService.V1.Adaptors;
using ActionService.V1.Controllers;
using ActionService.V1.Services;
using Castle.Core.Internal;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Action = Siccar.Application.Action;

namespace ActionUnitTests.V1.Services
{
    public class ActionServiceTest
    {
        private readonly ActionService.V1.Services.ActionService _underTest;
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly IPayloadResolver _fakePayloadResolver;
        public ActionServiceTest()
        {
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _fakePayloadResolver = A.Fake<IPayloadResolver>();
            _underTest = new(_fakeWalletServiceClient, _fakeRegisterServiceClient, _fakePayloadResolver);
        }

        public class GetSubmittedActionData : ActionServiceTest
        {
            [Fact]
            public async Task Should_Return_ActionData_SpecifiedInDisclosure()
            {
                var recipientAddress = "ws1axdfasdfijosdfs";
                var txId = "blueprinttxid";
                var data = new Dictionary<string, object> { { "ExpectedData1", "SomeExpectedData" }, {"NotExpectedData", "NotExpected" } };
                var senderId = Guid.NewGuid().ToString();
                var recipientId = Guid.NewGuid().ToString();
                var rawTransaction = new TransactionModel() { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Action } };
                var disclosure = new Disclosure() { ParticipantAddress = recipientId, DataPointers = new List<string> { "ExpectedData1" } };
                var action = new Action { 
                    Id = 1, Blueprint = txId,
                    Sender = senderId, 
                    Disclosures = new List<Disclosure> { disclosure },
                    PreviousTxId = txId,
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } ,
                    RequiredActionData = new List<string>()
                };
                var blueprint = new Blueprint() { Participants = new List<Participant> { new Participant { Id = recipientId, WalletAddress = recipientAddress } } };
                blueprint.Actions.Add(action);
                var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
                A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(A<string>._, A<List<TransactionModel>>._, A<IWalletServiceClient>._)).Returns(data);

                var result = await _underTest.GetSubmittedActionData(action.Id, recipientAddress, rawTransaction);
                var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(result.PreviousData);
                Assert.Equal(action.Id, result.Id);
                Assert.Equal(data["ExpectedData1"], resultData["ExpectedData1"].ToString());
                Assert.False(resultData.ContainsKey("NotExpectedData"));
            }

            [Fact]
            public async Task Should_Return_ActionData_SpecifiedInRequiredActionData()
            {
                var recipientAddress = "ws1axdfasdfijosdfs";
                var txId = "blueprinttxid";
                var data = new Dictionary<string, object> { { "ExpectedData1", "SomeExpectedData" }, { "NotExpectedData", "NotExpected" } };
                var senderId = Guid.NewGuid().ToString();
                var recipientId = Guid.NewGuid().ToString();
                var rawTransaction = new TransactionModel() { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Action } };
                var action = new Action
                {
                    Id = 1,
                    Blueprint = txId,
                    Sender = senderId,
                    Disclosures = new List<Disclosure>(),
                    PreviousTxId = txId,
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") },
                    RequiredActionData = new List<string> { "ExpectedData1" }
                };
                var blueprint = new Blueprint() { Participants = new List<Participant> { new Participant { Id = recipientId, WalletAddress = recipientAddress } } };
                blueprint.Actions.Add(action);
                var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
                A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(A<string>._, A<List<TransactionModel>>._, A<IWalletServiceClient>._)).Returns(data);

                var result = await _underTest.GetSubmittedActionData(action.Id, recipientAddress, rawTransaction);
                var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(result.PreviousData);
                Assert.Equal(action.Id, result.Id);
                Assert.Equal(data["ExpectedData1"], resultData["ExpectedData1"].ToString());
                Assert.False(resultData.ContainsKey("NotExpectedData"));
            }

            [Fact]
            public async Task Should_Return_ActionData_NoData()
            {
                var recipientAddress = "ws1axdfasdfijosdfs";
                var txId = "blueprinttxid";
                var senderId = Guid.NewGuid().ToString();
                var recipientId = Guid.NewGuid().ToString();
                var rawTransaction = new TransactionModel() { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Action } };
                var action = new Action
                {
                    Id = 1,
                    Blueprint = txId,
                    Sender = senderId,
                    Disclosures = new List<Disclosure>(),
                    PreviousTxId = txId,
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") },
                    RequiredActionData = new List<string> { "ExpectedData1" }
                };
                var blueprint = new Blueprint() { Participants = new List<Participant> { new Participant { Id = recipientId, WalletAddress = recipientAddress } } };
                blueprint.Actions.Add(action);
                var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);

                var result = await _underTest.GetSubmittedActionData(action.Id, recipientAddress, rawTransaction);
                var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(result.PreviousData);
                Assert.Empty(resultData);
            }
        }
    }
}
