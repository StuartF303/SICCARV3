using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Siccar.Platform.Cryptography;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WalletService.Core;
using WalletService.Exceptions;
using WalletService.V1.Controllers;
using Xunit;
using WalletService.Core.Interfaces;
using WalletService.V1.Services;

namespace WalletService.UnitTests.V1.Controllers
{
	public class WalletsControllerTest
	{
		private readonly WalletsController _underTest;
		private readonly ILogger<WalletsController> _loggerMock;
		private readonly IDaprClientAdaptor _client;
		private readonly IWalletRepository _walletRepositoryMock;
		private readonly IWalletFactory _walletFactoryMock;
		private readonly IRegisterServiceClient _mockServiceClient;
		private readonly string expectedOwner = Guid.NewGuid().ToString();
		private readonly string expectedTenant = Guid.NewGuid().ToString();

		public WalletsControllerTest()
		{
			_loggerMock = A.Fake<ILogger<WalletsController>>();
			_walletRepositoryMock = A.Fake<IWalletRepository>();
			_client = A.Fake<IDaprClientAdaptor>();
			_mockServiceClient = A.Fake<IRegisterServiceClient>();
			_walletFactoryMock = A.Fake<IWalletFactory>();
			_underTest = new WalletsController(_client, _loggerMock, _walletRepositoryMock, _walletFactoryMock, _mockServiceClient)
			{
				ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext(),
				}
			};

			var claim = new Claim("tenant", expectedTenant);
			var claim2 = new Claim("sub", expectedOwner);

			var claims = new ClaimsIdentity(new List<Claim> { claim, claim2 });
			_underTest.HttpContext.User = new ClaimsPrincipal(claims);
		}

		public class Get : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallGetAllWallets_When_AllInTenantIsFalse()
			{
				_underTest.HttpContext.User.Identities.First().AddClaim(new(ClaimTypes.Role, Constants.WalletUserRole));

				var wallet1 = new Wallet { Name = "wallet1", Owner = expectedOwner, Tenant = expectedTenant };
				var wallet2 = new Wallet { Name = "wallet2", Owner = expectedOwner, Tenant = expectedTenant };

				A.CallTo(() => _walletRepositoryMock.GetAll(expectedOwner))
					.Returns(new List<Wallet> { wallet1, wallet2 });

				var response = await _underTest.Get(allInTenant: false);
				var result = response.Result as OkObjectResult;
				var resultObjects = result.Value as List<Wallet>;

				Assert.Equal(2, resultObjects.Count);
				Assert.Contains(resultObjects, wallet => wallet.Name == "wallet1");
				Assert.Contains(resultObjects, wallet => wallet.Name == "wallet2");
			}

			[Fact]
			public async Task Should_CallGetAllWalletsInTenant_When_AllInTenantIsTrue()
			{
				_underTest.HttpContext.User.Identities.First().AddClaim(new(ClaimTypes.Role, Constants.TenantAdminRole));

				var wallet1 = new Wallet { Name = "wallet1", Owner = expectedOwner, Tenant = expectedTenant };
				var wallet2 = new Wallet { Name = "wallet2", Owner = expectedOwner, Tenant = expectedTenant };

				A.CallTo(() => _walletRepositoryMock.GetAllInTenant(expectedTenant))
					.Returns(new List<Wallet> { wallet1, wallet2 });

				var response = await _underTest.Get(allInTenant: true);

				var result = response.Result as OkObjectResult;
				var resultObjects = result.Value as List<Wallet>;

				Assert.Equal(2, resultObjects.Count);
				Assert.Contains(resultObjects, wallet => wallet.Name == "wallet1");
				Assert.Contains(resultObjects, wallet => wallet.Name == "wallet2");
			}
		}

		public class GetWallet : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallGetWallet()
			{
				var address = "ws1aasddofkjasdoifjapsdfa";
				var claim = new Claim("tenant", expectedOwner);
				var claim2 = new Claim("sub", expectedOwner);

				var claims = new ClaimsIdentity(new List<Claim> { claim, claim2 });
				_underTest.HttpContext.User = new ClaimsPrincipal(claims);

				await _underTest.GetWallet(address);

				A.CallTo(() => _walletRepositoryMock.GetWallet(address, expectedOwner));
			}
		}

		public class CreateWallet : WalletsControllerTest
		{

			[Fact]
			public async Task Should_Add_CurrentUserAs_Owner()
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet" };

				var response = await _underTest.CreateWallet(createRequest);

				A.CallTo(() => _walletRepositoryMock.SaveWallet(A<Wallet>.That.Matches(wallet => wallet.Owner == expectedOwner), expectedOwner));
			}

			[Fact]
			public async Task Should_Call_BuildWallet()
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet" };

				var response = await _underTest.CreateWallet(createRequest);

				A.CallTo(() => _walletFactoryMock.BuildWallet("test-wallet", null, null, expectedTenant, expectedOwner, true)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Publish_Address()
			{
				var expected = new WalletAddress
				{
					Address = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh",
					WalletId = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh"
				};

				var createRequest = new CreateWalletRequest
				{
					Name = "test-wallet",
					Mnemonic = "maid limit write faculty night beauty wash mushroom grace fashion immune swallow property similar sort payment crew notable tobacco disagree rate blind alter kit"
				};

				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, true))
					.Returns((
					new Wallet() { Address = expected.Address },
					null));

				var response = await _underTest.CreateWallet(createRequest);

				A.CallTo(() => _client.PublishEventAsync(
					WalletConstants.PubSub,
					Topics.WalletAddressCreationTopicName,
					A<WalletAddress>.That.Matches(w => w.Address == expected.Address)))
					.MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Have_Mnemonic()
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet" };
				var mnemonic = "network ocean gravity observe actor release skill lazy wheel oppose soon key spare debris mountain educate coin rival cruise human imitate come early gas";
				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, true)).Returns((new Wallet(), mnemonic));

				var response = await _underTest.CreateWallet(createRequest);
				var result = response.Result as AcceptedResult;
				var resultObject = result.Value as CreateWalletResponse;

				Assert.Equal(mnemonic, resultObject.Mnemonic);
			}

			[Fact]
			public async Task Should_Call_SaveWallet()
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet" };
				var mnemonic = "network ocean gravity observe actor release skill lazy wheel oppose soon key spare debris mountain educate coin rival cruise human imitate come early gas";
				var expected = new Wallet { Owner = expectedOwner };
				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, true)).Returns((expected, mnemonic));

				var result = await _underTest.CreateWallet(createRequest);

				A.CallTo(() => _walletRepositoryMock.SaveWallet(expected, expectedOwner)).MustHaveHappened();
			}
		}

		public class Rebuild : WalletsControllerTest
		{

			[Fact]
			public async Task Should_Add_CurrentUserAs_Owner()
			{
				var mnemonic = "network ocean gravity observe actor release skill lazy wheel oppose soon key spare debris mountain educate coin rival cruise human imitate come early gas";
				var createRequest = new CreateWalletRequest { Name = "test-wallet", Mnemonic = mnemonic };
				var registerId = Guid.NewGuid().ToString();

				var response = await _underTest.RebuildWallet(createRequest, registerId);

				A.CallTo(() => _walletRepositoryMock.SaveWallet(A<Wallet>.That.Matches(wallet => wallet.Owner == expectedOwner), expectedOwner));
			}

			[Theory]
			[InlineData(null)]
			[InlineData("")]
			public async Task Should_Throw_When_MnemonicNullOrEmpty(string mnemonic)
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet", Mnemonic = mnemonic };
				var registerId = Guid.NewGuid().ToString();

				var result = await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.RebuildWallet(createRequest, registerId));

				Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.Status);
			}

			[Fact]
			public async Task Should_Call_BuildWallet()
			{
				var createRequest = new CreateWalletRequest { Name = "test-wallet", Mnemonic = "mnemonic" };
				var registerId = Guid.NewGuid().ToString();

				var response = await _underTest.RebuildWallet(createRequest, registerId);

				A.CallTo(() => _walletFactoryMock.BuildWallet("test-wallet", createRequest.Mnemonic, registerId, expectedTenant, expectedOwner, false)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Publish_Address()
			{
				var registerId = Guid.NewGuid().ToString();

				var expected = new WalletAddress
				{
					Address = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh",
					WalletId = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh"
				};

				var createRequest = new CreateWalletRequest
				{
					Name = "test-wallet",
					Mnemonic = "maid limit write faculty night beauty wash mushroom grace fashion immune swallow property similar sort payment crew notable tobacco disagree rate blind alter kit"
				};

				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, false))
					.Returns((
					new Wallet() { Addresses = new List<WalletAddress> { expected } },
					null));

				var response = await _underTest.RebuildWallet(createRequest, registerId);

				A.CallTo(() => _client.PublishEventAsync(
					WalletConstants.PubSub,
					Topics.WalletAddressCreationTopicName,
					A<ICollection<WalletAddress>>.That.Matches(w => w.Contains(expected))))
					.MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Have_Mnemonic()
			{
				var mnemonic = "network ocean gravity observe actor release skill lazy wheel oppose soon key spare debris mountain educate coin rival cruise human imitate come early gas";
				var createRequest = new CreateWalletRequest { Name = "test-wallet", Mnemonic = mnemonic };
				var registerId = Guid.NewGuid().ToString();
				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, false)).Returns((new Wallet(), mnemonic));

				var response = await _underTest.RebuildWallet(createRequest, registerId);
				var result = response.Result as AcceptedResult;
				var resultObject = result.Value as CreateWalletResponse;

				Assert.Equal(mnemonic, resultObject.Mnemonic);
			}

			[Fact]
			public async Task Should_Call_SaveWallet()
			{
				var mnemonic = "network ocean gravity observe actor release skill lazy wheel oppose soon key spare debris mountain educate coin rival cruise human imitate come early gas";
				var createRequest = new CreateWalletRequest { Name = "test-wallet", Mnemonic = mnemonic };
				var registerId = Guid.NewGuid().ToString();
				var expected = new Wallet { Owner = expectedOwner };
				A.CallTo(() => _walletFactoryMock.BuildWallet(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, false)).Returns((expected, mnemonic));

				var result = await _underTest.RebuildWallet(createRequest, registerId);

				A.CallTo(() => _walletRepositoryMock.SaveWallet(expected, expectedOwner)).MustHaveHappened();
			}
		}

		public class UpdateWallet : WalletsControllerTest
		{
			[Fact]
			public async Task Should_Call_UpdateWallet()
			{
				var address = "ws1jcxhauekhfdh8t7t7yz6x9jt9pgx6ve5w967yn8vp0yq90scr5vrskem4mk";
				var updateRequest = new UpdateWalletRequest { Name = "test-wallet" };

				var result = await _underTest.UpdateWallet(address, updateRequest);

				A.CallTo(() => _walletRepositoryMock.UpdateWallet(address, expectedOwner, updateRequest)).MustHaveHappened();
			}
		}

		public class DeleteWallet : WalletsControllerTest
		{
			[Fact]
			public async Task Should_Call_DeleteWallet()
			{
				var address = "ws1jcxhauekhfdh8t7t7yz6x9jt9pgx6ve5w967yn8vp0yq90scr5vrskem4mk";

				var result = await _underTest.Delete(address);

				A.CallTo(() => _walletRepositoryMock.DeleteWallet(address, expectedOwner)).MustHaveHappened();
			}
		}

		public class CreateTransaction : WalletsControllerTest
		{

			[Fact]
			public async Task Should_AddToWalletAddress_ToTransaction()
			{
				var expected = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var senderWallet = new Wallet { Address = expected, PrivateKey = privateKey };
				A.CallTo(() => _walletRepositoryMock.GetWallet(expected, expectedOwner)).Returns(senderWallet);

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(Convert.FromBase64String("SGVsbG8K"), new string[] { expected });

				var result = await _underTest.SignTransaction(expected, transaction.GetTxTransport().transport);
				Assert.Equal(expected, result?.Value?.SenderWallet);
			}

			[Fact]
			public async Task Should_AddAllMetaData()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(Convert.FromBase64String("SGVsbG8K"), new string[] { sendingWallet });
				transaction.SetPrevTxHash("7a39929b2f38d6d4156451bb04eb5d1f43a19d9eceef55ad9af7bf69ee9a336a");
				TransactionMetaData meta_data = new()
				{
					InstanceId = Guid.NewGuid().ToString(),
					ActionId = 1,
					NextActionId = 2,
					BlueprintId = Guid.NewGuid().ToString(),
					RegisterId = "registerid",
				};
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				Assert.Equal(meta_data.RegisterId, result?.Value?.MetaData.RegisterId);
				Assert.Equal(meta_data.InstanceId, result?.Value?.MetaData.InstanceId);
				Assert.Equal(meta_data.NextActionId, result?.Value?.MetaData.NextActionId);
				Assert.Equal(meta_data.BlueprintId, result?.Value?.MetaData.BlueprintId);
				Assert.Equal(transaction.GetPrevTxHash().hash, result?.Value?.PrevTxId);
			}

			[Fact]
			public async Task Should_AddTransactionType_ToReturnedTransaction()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var expected = TransactionTypes.Blueprint;
				TransactionMetaData meta_data = new() { TransactionType = expected };

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(Convert.FromBase64String("SGVsbG8K"), new string[] { sendingWallet });
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				Assert.Equal(expected, result?.Value?.MetaData.TransactionType);
			}

			[Fact]
			public async Task Should_AddTransactionAddress_ToRegisterTransaction()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var destinationWalletAddress = "ws1jz37tgfdh62h8qawlz3d4glvxmdndanyrvpmrww8at409x28e7caqpwzpmn";
				var payload = Convert.FromBase64String("SGVsbG8K");

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetTxRecipients(new string[] { sendingWallet, destinationWalletAddress });
				IPayloadManager manager = transaction.GetTxPayloadManager();
				manager.AddPayload(payload, new string[] { sendingWallet });
				manager.AddPayload(payload, new string[] { destinationWalletAddress });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				Assert.Equal(2, result?.Value?.RecipientsWallets.Count());
				Assert.Contains(sendingWallet, result?.Value?.RecipientsWallets);
				Assert.Contains(destinationWalletAddress, result?.Value?.RecipientsWallets);
			}

			[Fact]
			public async Task Should_AddPayloads_ToReturnedTransaction()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var payload = Convert.FromBase64String("SGVsbG8K");

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				IPayloadManager manager = transaction.GetTxPayloadManager();
				manager.AddPayload(payload, new string[] { sendingWallet });
				manager.AddPayload(payload, new string[] { sendingWallet });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				Assert.Equal(2, result?.Value?.Payloads.Length);
				Assert.Equal((ulong)2, result?.Value?.PayloadCount);
			}

			[Fact]
			public async Task Should_AddWalletAddressToPayloads_ToRegisterTransaction()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var destinationWalletAddress = "ws1jz37tgfdh62h8qawlz3d4glvxmdndanyrvpmrww8at409x28e7caqpwzpmn";
				var payload = Convert.FromBase64String("SGVsbG8K");

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				IPayloadManager manager = transaction.GetTxPayloadManager();
				manager.AddPayload(payload, new string[] { sendingWallet });
				manager.AddPayload(payload, new string[] { destinationWalletAddress });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				var sendingAddressPayload = result?.Value?.Payloads.ToList().Find(payload => payload.WalletAccess.Contains(sendingWallet));
				var destinationAddressPayload = result?.Value?.Payloads.ToList().Find(payload => payload.WalletAccess.Contains(destinationWalletAddress));

				Assert.NotNull(sendingAddressPayload);
				Assert.Contains(sendingWallet, sendingAddressPayload.WalletAccess);
				Assert.NotNull(destinationAddressPayload);
				Assert.Contains(destinationWalletAddress, destinationAddressPayload.WalletAccess);
			}

			[Fact]
			public async Task Should_AddMutipleAddressesToSinglePayload_ToRegisterTransaction()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var destinationWalletAddress = "ws1jz37tgfdh62h8qawlz3d4glvxmdndanyrvpmrww8at409x28e7caqpwzpmn";
				var payloadData = Convert.FromBase64String("SGVsbG8K");

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(payloadData, new string[] { sendingWallet, destinationWalletAddress });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				Assert.NotEmpty(result?.Value?.Payloads);
				Assert.Contains(sendingWallet, result.Value.Payloads[0].WalletAccess);
				Assert.Contains(destinationWalletAddress, result.Value.Payloads[0].WalletAccess);
			}

			[Fact]
			public async Task Should_PublishTransactionCreatedEvent()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var payloadData = Convert.FromBase64String("SGVsbG8K");

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(payloadData, new string[] { sendingWallet });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				A.CallTo(() => _client.PublishEventAsync(
					WalletConstants.PubSub,
					Topics.TransactionPendingTopicName,
					A<TransactionModel>.That.Matches(tx => tx.SenderWallet == sendingWallet)))
					.MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_CreateTX_Which_IsValid()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var expected = TransactionTypes.Blueprint;
				TransactionMetaData meta_data = new() { TransactionType = expected };

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(payload, new string[] { sendingWallet });
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);
				var tx = TransactionFormatter.ToTransaction(result.Value);
				var cryptoTx = TransactionBuilder.Build(tx);
				var verifyStatus = cryptoTx.transaction.VerifyTx();
				Assert.Equal(Status.STATUS_OK, verifyStatus);
			}

			[Fact]
			public async Task ShouldCall_AddWalletTransactionForEachRecipientAndSender()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var recinpientWallet1 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
				var recinpientWallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var senderWallet = new Wallet { Address = sendingWallet, PrivateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM" };
				A.CallTo(() => _walletRepositoryMock.GetWallet(sendingWallet, expectedOwner)).Returns(senderWallet);

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetTxRecipients(new string[] { recinpientWallet1, recinpientWallet2 });
				transaction.GetTxPayloadManager().AddPayload(payload, new string[] { recinpientWallet1, recinpientWallet2 });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				A.CallTo(() => _walletRepositoryMock.AddWalletTransaction(recinpientWallet1, A<WalletTransaction>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.AddWalletTransaction(recinpientWallet2, A<WalletTransaction>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.AddWalletTransaction(sendingWallet, A<WalletTransaction>.Ignored)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task ShouldCall_AddWalletTransactionOnlyOnceWhenRecipient_IsAlsoSender()
			{
				SetUpTestForSuccess();
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var expected = TransactionTypes.Blueprint;
				TransactionMetaData meta_data = new() { TransactionType = expected };

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetTxRecipients(new string[] { sendingWallet });
				transaction.GetTxPayloadManager().AddPayload(payload, new string[] { sendingWallet });

				var result = await _underTest.SignTransaction(sendingWallet, transaction.GetTxTransport().transport);

				A.CallTo(() => _walletRepositoryMock.AddWalletTransaction(sendingWallet, A<WalletTransaction>.Ignored)).MustHaveHappenedOnceExactly();
			}

			private void SetUpTestForSuccess()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var fakeTask = Task.FromResult(new Wallet { Address = sendingWallet, PrivateKey = privateKey });
				A.CallTo(() => _walletRepositoryMock.GetWallet(sendingWallet, expectedOwner)).Returns(fakeTask);
			}
		}

		public class DecryptTransactionPayloads : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallGetWallet_UsingWalletAddress()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var payload = Convert.FromBase64String("SGVsbG8K");

				var transaction = BuildTransaction(sendingWallet, privateKey, payload);
				var request = await _underTest.DecryptTransactionPayloads(sendingWallet, transaction);

				A.CallTo(() => _walletRepositoryMock.GetWallet(sendingWallet, expectedOwner)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_ReturnListOfDecryptedPayloads()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				SetUpTestForSuccess(sendingWallet, privateKey);
				var payload = Convert.FromBase64String("SGVsbG8K");
				var transaction = BuildTransaction(sendingWallet, privateKey, payload);

				var request = await _underTest.DecryptTransactionPayloads(sendingWallet, transaction);

				Assert.Contains(payload, request.Value);

			}

			private void SetUpTestForSuccess(string sendingWallet, string privateKey)
			{
				var fakeTask = Task.FromResult(new Wallet { Address = sendingWallet, PrivateKey = privateKey });
				A.CallTo(() => _walletRepositoryMock.GetWallet(sendingWallet, expectedOwner)).Returns(fakeTask);
			}
		}

		public class IsValidTransaction : WalletsControllerTest
		{
			[Fact]
			public void Should_ReturnTrue_ForValidTransaction()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				SetUpTestForSuccess(sendingWallet, privateKey);
				var payload = Convert.FromBase64String("SGVsbG8K");
				var transaction = BuildTransaction(sendingWallet, privateKey, payload);

				var request = _underTest.IsValidTransaction(sendingWallet, transaction);

				Assert.True(request.Value);
			}

			[Fact]
			public void Should_ReturnFalse_ForInValidTransactionSignatureAsync()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var transaction = BuildTransaction(sendingWallet, privateKey, payload);

				var invalidSig = "3e3857dec2c2a9a01078a7f5609e6b67188428acd05a9e420175e6feaaf92b357bad612de5ad8c4d084ee8e2cd5aebec0c85f8d1e051024cc8af8f79fbbaa509";
				transaction.Signature = invalidSig;

				var request = _underTest.IsValidTransaction(sendingWallet, transaction);

				Assert.False(request.Value);
			}

			[Fact]
			public void Should_ReturnFalse_ForUnconfiguredTransactionAsync()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";

				var request = _underTest.IsValidTransaction(sendingWallet, new TransactionModel());

				Assert.False(request.Value);
			}

			private void SetUpTestForSuccess(string sendingWallet, string privateKey)
			{
				var fakeTask = Task.FromResult(new Wallet { Address = sendingWallet, PrivateKey = privateKey });
				A.CallTo(() => _walletRepositoryMock.GetWallet(sendingWallet, expectedOwner)).Returns(fakeTask);
			}
		}

		public class GetTransactionById : WalletsControllerTest
		{
			[Fact]
			public async Task Should_Call_RegisterServiceClient()
			{
				var txId = Guid.NewGuid().ToString();
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var expectedRegister = "someregid";
				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(null as Wallet);
				var claim = new Claim("Registers", expectedRegister);
				var claims = new ClaimsIdentity(new List<Claim> { claim });
				_underTest.HttpContext.User = new ClaimsPrincipal(claims);

				await _underTest.GetTransactionById(walletAddress, txId);

				A.CallTo(() => _mockServiceClient.GetTransactionById(expectedRegister, txId)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Throw_WhenWalletIsNull()
			{
				var txId = Guid.NewGuid().ToString();
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(null as Wallet);
				_ = await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.GetTransactionById(walletAddress, txId));
			}
		}

		public class GetWalletDelegates : WalletsControllerTest
		{
			[Fact]
			public async Task Should_Return_DelegatesFromWallet()
			{
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var expected = new WalletAccess();
				var wallet = new Wallet() { Delegates = new List<WalletAccess> { expected } };
				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, A<string>.Ignored)).Returns(wallet);

				var response = await _underTest.GetWalletDelegates(walletAddress);

				var result = response.Result as OkObjectResult;
				var resultObject = result.Value as List<WalletAccess>;
				Assert.Equal(expected, resultObject[0]);
			}
		}

		public class AddWalletDelegates : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallAddWalletDelegate()
			{

				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var expected = new List<WalletAccess>
				{
					new()
					{
						WalletId = walletAddress
					}
				};

				await _underTest.AddWalletDelegates(walletAddress, expected);

				A.CallTo(() => _walletRepositoryMock.AddDelegates(walletAddress, expected, expectedOwner)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task WalletAddressInUrl_DoesNotMatchAnAddressInBody_ShouldReturnBadRequest()
			{
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var accesses = new List<WalletAccess>
				{
					new()
					{
						WalletId = "differentwalletaddress"
					},
					new()
					{
						WalletId = walletAddress,
					}
				};

				var response = await _underTest.AddWalletDelegates(walletAddress, accesses);

				Assert.True(response.Result is BadRequestObjectResult);
			}
		}

		public class RemoveWalletDelegate : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallRemoveDelegate()
			{
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var access = new WalletAccess();

				var response = await _underTest.RemoveWalletDelegate(walletAddress, access.Subject);

				A.CallTo(() => _walletRepositoryMock.RemoveDelegate(walletAddress, access.Subject, expectedOwner)).MustHaveHappenedOnceExactly();
			}
		}

		public class UpdateWalletDelegate : WalletsControllerTest
		{
			[Fact]
			public async Task Should_CallUpdateDelegate()
			{
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var access = new WalletAccess();

				var response = await _underTest.UpdateWalletDelegate(walletAddress, access);

				A.CallTo(() => _walletRepositoryMock.UpdateDelegate(walletAddress, access, expectedOwner)).MustHaveHappenedOnceExactly();
			}
		}

		private static TransactionModel BuildTransaction(string sendingWallet, string sendingWalletPrivateKey, byte[] payload)
		{
			var transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			transaction.SetTxRecipients(new string[] { sendingWallet });
			var metaData = new TransactionMetaData()
			{
				ActionId = 1,
				NextActionId = 1,
				BlueprintId = "12342135432424",
				InstanceId = "123245235235253",
				RegisterId = Guid.NewGuid().ToString("N"),
				TransactionType = TransactionTypes.Action,
				//TrackingData = JsonDocument.Parse("{\"Data\":\"data\", \"other\":\"data\"}")
			};
			transaction.SetTxMetaData(JsonSerializer.Serialize(metaData));
			var payloadMgr = transaction.GetTxPayloadManager();
			payloadMgr.AddPayload(payload, new string[] { sendingWallet });
			transaction.SignTx(sendingWalletPrivateKey);
			return TransactionFormatter.ToModel(transaction.GetTxTransport().transport);
		}
	}
}
