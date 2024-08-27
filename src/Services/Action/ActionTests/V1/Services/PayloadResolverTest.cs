using ActionService.Exceptions;
using ActionService.V1.Repositories;
using ActionService.V1.Services;
using FakeItEasy;
using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using SiccarApplicationTests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace ActionUnitTests.V1.Services
{
	public class PayloadResolverTest
	{
		private readonly IFileRepository _fakeFileRepo;
		private readonly IPayloadResolver _underTest;
		private readonly TestData testData = new();

		public PayloadResolverTest()
		{
			_fakeFileRepo = A.Fake<IFileRepository>();
			_underTest = new PayloadResolver(_fakeFileRepo);
		}

		public class BuildFilePayload : PayloadResolverTest
		{
			[Fact]
			public async Task ShouldGetFile()
			{
				var filename = "myfile.jpg";
				var fileSchemaId = "file1";
				var data = new Dictionary<string, object>() { { fileSchemaId, filename } };
				var blueprint = new Blueprint { Participants = new List<Participant> { testData.participant1(), testData.participant2() } };
				var disclosures = new List<Disclosure>() { new Disclosure()
				{
					ParticipantAddress = testData.participant1().WalletAddress,
					DataPointers = new List<string> { filename }
				} };
				await _underTest.AddFilePayloadToTransaction(blueprint, disclosures, filename, fileSchemaId);
				A.CallTo(() => _fakeFileRepo.GetFile(filename)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldBuildPayloadAddingWalletsForEachDisclosure()
			{
				var filename = "myfile.jpg";
				var fileSchemaId = "file1";
				var expected = new List<string> { testData.participant1().WalletAddress, testData.participant2().WalletAddress };
				var blueprint = new Blueprint { Participants = new List<Participant> { testData.participant1(), testData.participant2() } };
				var disclosures = new List<Disclosure>() { new Disclosure()
				{
					ParticipantAddress = expected[0],
					DataPointers = new List<string> { fileSchemaId }
				},
				new Disclosure()
				{
					ParticipantAddress = expected[1],
					DataPointers = new List<string> { fileSchemaId }
				} };
				A.CallTo(() => _fakeFileRepo.GetFile(filename)).Returns(Task.FromResult(new MemoryStream(new byte[4]) as Stream));
				var result = await _underTest.AddFilePayloadToTransaction(blueprint, disclosures, filename, fileSchemaId);
				IPayloadInfo[]? info = result.GetTxPayloadManager().GetPayloadsInfo().info;
				Assert.NotNull(info);
				Assert.Single(info);
				Assert.Contains(expected[0], info[0].GetPayloadWallets());
				Assert.Contains(expected[1], info[0].GetPayloadWallets());
			}
			[Fact]
			public async Task ShouldReturnFileBinaryInPayload()
			{
				var filename = "myfile.jpg";
				var fileSchemaId = "file1";
				var walletAddresses = new List<string> { testData.participant1().WalletAddress, testData.participant2().WalletAddress };
				var blueprint = new Blueprint { Participants = new List<Participant> { testData.participant1(), testData.participant2() } };
				var disclosures = new List<Disclosure>() { new Disclosure()
				{
					ParticipantAddress = walletAddresses[0],
					DataPointers = new List<string> { fileSchemaId }
				},
				new Disclosure()
				{
					ParticipantAddress = walletAddresses[1],
					DataPointers = new List<string> { fileSchemaId }
				} };
				A.CallTo(() => _fakeFileRepo.GetFile(filename)).Returns(Task.FromResult(new MemoryStream(new byte[4]) as Stream));
				var result = await _underTest.AddFilePayloadToTransaction(blueprint, disclosures, filename, fileSchemaId);
				IPayloadInfo[]? info = result.GetTxPayloadManager().GetPayloadsInfo().info;
				Assert.NotNull(info);
				Assert.Single(info);
				Assert.NotEmpty(info[0].GetPayloadHash()!);
				Assert.True(info[0].GetPayloadEncrypted());
			}
		}

		public class ResolveParticpantPayloads : PayloadResolverTest
		{
			[Fact]
			public void Should_Throw_WhenPartcipantNotFoundInBlueprint()
			{
				var senderId = Guid.NewGuid().ToString();
				var blueprint = new Blueprint { Participants = new List<Participant> { testData.participant1(), testData.participant2() } };
				var emptyDisclosures = new List<Disclosure>() { testData.disclosure1() };
				var data = new Dictionary<string, object>() { { "data1", "example test data" } };
				var jsonData = JsonDocument.Parse(JsonSerializer.Serialize(data));
				// this needs to at least try resolve a wallet that does not exist 
				Assert.Throws<PayloadResolverException>(() => _underTest.AddParticipantPayloadsToTransaction(testData.blueprint1(), emptyDisclosures, jsonData, senderId));
			}
			[Fact]
			public void Should_ReturnAllDataForSender()
			{
				var testDisclosures = new List<Disclosure> { new Disclosure(testData.walletAddress2, new List<string>() { "/data1" }) };
				var data = new Dictionary<string, string> { { "data1", "somedatavalue" } };
				var jsonData = JsonDocument.Parse(JsonSerializer.Serialize(data));
				var result = _underTest.AddParticipantPayloadsToTransaction(testData.blueprint1(), testDisclosures, jsonData, testData.walletAddress1);
				IPayloadInfo[]? info = result.GetTxPayloadManager().GetPayloadsInfo().info;
				Assert.NotNull(info);
				Assert.Single(info);
				Assert.Equal(testData.walletAddress1, info[0].GetPayloadWallets()[1]);
				Assert.True(info[0].GetPayloadEncrypted());
			}
			[Fact]
			public void Should_ReturnPayloadToWalletForEachUniqueDisclosure()
			{
				var testBlueprint = testData.blueprint1(); // take our clean know good
				testBlueprint.Participants.Add(testData.participant3()); // we need an extra player
				var data = new Dictionary<string, object>();
				var dataFieldId1 = "data1";
				var dataFieldId2 = "data2";
				data.Add(dataFieldId1, "somedatavalue");
				data.Add(dataFieldId2, 123);
				var disclosures = new List<Disclosure>
				{
					new Disclosure(testData.walletAddress2, new List<string> { "/" + dataFieldId1 }),
					new Disclosure(testData.walletAddress3, new List<string> { "/" + dataFieldId2 })
				};
				var jsonData = JsonDocument.Parse(JsonSerializer.Serialize(data));
				var result = _underTest.AddParticipantPayloadsToTransaction(testBlueprint, disclosures, jsonData, testData.walletAddress1);
				IPayloadInfo[]? info = result.GetTxPayloadManager().GetPayloadsInfo().info;
				Assert.NotNull(info);
				Assert.Equal(2, info.Length);
				Assert.Equal(info[0].GetPayloadWallets()[0], testData.walletAddress2);
				Assert.Equal(info[1].GetPayloadWallets()[0], testData.walletAddress3);
				/// TODO: other checks, such as:
				/// That payload 1 cannot be accessed by walletAddr3 etc
			}
			[Fact]
			public void Should_ReturnPayloadDataForEachUniqueDisclosure()
			{
				var testBlueprint = testData.blueprint1(); // take our clean know good
				testBlueprint.Participants.Add(testData.participant3()); // we need an extra player
				string dataFieldId1 = "data1";
				var dataFieldId2 = "data2";
				var disclosures = new List<Disclosure>
				{
					new Disclosure(testData.walletAddress2, new List<string> { "/" + dataFieldId1 }),
					new Disclosure(testData.walletAddress3, new List<string> { "/" + dataFieldId2 })
				};
				var data = new Dictionary<string, object>
				{
					{ dataFieldId1, "somedatavalue" },
					{ dataFieldId2, 123 }
				};
				var jsonData = JsonDocument.Parse(JsonSerializer.Serialize(data));
				var result = _underTest.AddParticipantPayloadsToTransaction(testBlueprint, disclosures, jsonData, testData.walletAddress1);
				string[] expected1 = new string[] { testData.walletAddress2, testData.walletAddress1 };
				string[] expected2 = new string[] { testData.walletAddress3, testData.walletAddress1 };

				IPayloadInfo[]? info = result.GetTxPayloadManager().GetPayloadsInfo().info;
				Assert.NotNull(info);
				Assert.Equal(2, info.Length);
				Assert.Equal(expected1, info[0].GetPayloadWallets());
				Assert.Equal(expected2, info[1].GetPayloadWallets());
			}
		}

		public class GetAllPreviousPayloadsForWalletAsync : PayloadResolverTest
		{
			private readonly IWalletServiceClient _fakeWalletServiceClient;

			public GetAllPreviousPayloadsForWalletAsync() { _fakeWalletServiceClient = A.Fake<IWalletServiceClient>(); }

			[Fact]
			public async Task Should_Call_DecryptOnAllTransactions()
			{
				var tx1 = new TransactionModel();
				var tx2 = new TransactionModel();
				var tx3 = new TransactionModel();
				var txList = new List<TransactionModel> { tx1, tx2, tx3 };
				var result = await _underTest.GetAllPreviousPayloadsForWalletAsync("", txList, _fakeWalletServiceClient);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx1, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx2, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx3, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Return_TransactionData()
			{
				var tx1 = new TransactionModel();
				var tx2 = new TransactionModel();
				var tx3 = new TransactionModel();
				var txList = new List<TransactionModel> { tx1, tx2, tx3 };
				var data1 = new Dictionary<string, object>();
				var data2 = new Dictionary<string, object>();
				var data3 = new Dictionary<string, object>();
				data1.Add("data1", "somedata");
				data2.Add("data2", "somedata");
				data3.Add("data3", "somedata");
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx1, A<string>.Ignored)).Returns(new byte[][] { JsonSerializer.SerializeToUtf8Bytes(data1) });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx2, A<string>.Ignored)).Returns(new byte[][] { JsonSerializer.SerializeToUtf8Bytes(data2) });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(tx3, A<string>.Ignored)).Returns(new byte[][] { JsonSerializer.SerializeToUtf8Bytes(data3) });
				var result = await _underTest.GetAllPreviousPayloadsForWalletAsync("", txList, _fakeWalletServiceClient);
				Assert.True(result.ContainsKey(data1.First().Key));
				Assert.True(result.ContainsKey(data2.First().Key));
				Assert.True(result.ContainsKey(data3.First().Key));
				Assert.Equal(data1["data1"], result["data1"].ToString());
				Assert.Equal(data2["data2"], result["data2"].ToString());
				Assert.Equal(data3["data3"], result["data3"].ToString());
			}
		}
	}
}
