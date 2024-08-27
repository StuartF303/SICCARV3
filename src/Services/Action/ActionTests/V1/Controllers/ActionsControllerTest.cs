using ActionService.V1.Adaptors;
using ActionService.V1.Controllers;
using ActionService.V1.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Application.Validation;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using SiccarApplicationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Action = Siccar.Application.Action;

namespace ActionUnitTests.V1.Controllers
{
	public class ActionsControllerTest
	{
		private readonly ActionsController _underTest;
		private readonly IWalletServiceClient _fakeWalletServiceClient;
		private readonly IRegisterServiceClient _fakeRegisterServiceClient;
		private readonly IPayloadResolver _fakePayloadResolver;
		private readonly IActionResolver _fakeActionResolver;
		private readonly IHubContextAdaptor _fakeHubContext;
		private readonly ISchemaDataValidator _fakeSchemaValidator;
		private readonly ISustainabilityHttpServiceClient _fakeSusServiceClient;
		private readonly ITenantServiceClient _fakeTenantServiceClient;
		private readonly ITransactionRequestBuilder _fakeTransactionRequestBuilder;
		private readonly ILogger<ActionsController> _fakeLogger; 
		private readonly string _expectedOwner = Guid.NewGuid().ToString();
		private readonly string _expectedTenant = Guid.NewGuid().ToString();
		private const string _registerId = "0fb3b0c0-4972-402c-bbec-99bb41ab7d28";

		private readonly TestData testData = new();

		public ActionsControllerTest()
		{
			_fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
			_fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
			_fakePayloadResolver = A.Fake<IPayloadResolver>();
			_fakeActionResolver = A.Fake<IActionResolver>();
			_fakeHubContext = A.Fake<IHubContextAdaptor>();
			_fakeSchemaValidator = A.Fake<ISchemaDataValidator>();
			_fakeSusServiceClient = A.Fake<ISustainabilityHttpServiceClient>();
			_fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
			_fakeTransactionRequestBuilder = A.Fake<ITransactionRequestBuilder>();
			_fakeLogger = A.Fake<ILogger<ActionsController>>();

            var actionService = new ActionService.V1.Services.ActionService(_fakeWalletServiceClient, _fakeRegisterServiceClient, _fakePayloadResolver);
			_underTest = new ActionsController(actionService, _fakeRegisterServiceClient, _fakeWalletServiceClient, _fakeTenantServiceClient, _fakePayloadResolver,
				_fakeActionResolver, _fakeHubContext, _fakeSchemaValidator, _fakeSusServiceClient, _fakeTransactionRequestBuilder, _fakeLogger)
			{
				ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() }
			};

			var claim = new Claim("tenant", _expectedTenant);
			var claim2 = new Claim("sub", _expectedOwner);
			var claims = new ClaimsIdentity(new List<Claim> { claim, claim2 });
			_underTest.HttpContext.User = new ClaimsPrincipal(claims);
		}

		public class NotifyClientsOfNewTransaction : ActionsControllerTest
		{
			[Fact]
			public async Task ShouldCall_SendAsyncWithTransactionConfirmed()
			{
				var walletAddress = "ws1jle65qnan70zm4jeuxtvqk9vf3rwaqlzvhmlg5949jmry4g9r376sca3nhv";
				var expected = new TransactionConfirmed()
				{
					MetaData = new TransactionMetaData() { TransactionType = TransactionTypes.Action },
					TransactionId = "f44d1dbf2cf56cdb3aec0be52be1982514ebfd276e9e430b0f0c25f7bede7f68",
					ToWallets = new List<string> { walletAddress }
				};
				await _underTest.NotifyClientsOfNewTransaction(expected);
				A.CallTo(() => _fakeHubContext.SendToGroupAsync(walletAddress, "ReceiveAction", A<TransactionConfirmed>.That.Matches(txC => txC.TransactionId == expected.TransactionId))).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldCall_SendAsyncForEachWalletGroup()
			{
				var walletAddress = "ws1jle65qnan70zm4jeuxtvqk9vf3rwaqlzvhmlg5949jmry4g9r376sca3nhv";
				var walletAddress2 = "ws1jfk5jwvqvpnqr2nxaxlwq76falqymmz29q4rar3x05xkjzpqqqp2qcv08xa";
				var expected = new TransactionConfirmed()
				{
					MetaData = new TransactionMetaData() { TransactionType = TransactionTypes.Action },
					TransactionId = "f44d1dbf2cf56cdb3aec0be52be1982514ebfd276e9e430b0f0c25f7bede7f68",
					ToWallets = new List<string> { walletAddress, walletAddress2 }
				};
				await _underTest.NotifyClientsOfNewTransaction(expected);
				A.CallTo(() => _fakeHubContext.SendToGroupAsync(walletAddress, "ReceiveAction", A<TransactionConfirmed>.That.Matches(txC => txC.TransactionId == expected.TransactionId))).MustHaveHappenedOnceExactly()
					.Then(A.CallTo(() => _fakeHubContext.SendToGroupAsync(walletAddress2, "ReceiveAction", A<TransactionConfirmed>.That.Matches(txC => txC.TransactionId == expected.TransactionId))).MustHaveHappenedOnceExactly());
			}
			[Fact]
			public async Task ShouldCall_SustainabilityServiceClient()
			{
				var walletAddress = "ws1jle65qnan70zm4jeuxtvqk9vf3rwaqlzvhmlg5949jmry4g9r376sca3nhv";
				var walletAddress2 = "ws1jfk5jwvqvpnqr2nxaxlwq76falqymmz29q4rar3x05xkjzpqqqp2qcv08xa";
				var expected = new TransactionConfirmed()
				{
					MetaData = new TransactionMetaData() { TransactionType = TransactionTypes.Action },
					TransactionId = "f44d1dbf2cf56cdb3aec0be52be1982514ebfd276e9e430b0f0c25f7bede7f68",
					ToWallets = new List<string> { walletAddress, walletAddress2 }
				};
				await _underTest.NotifyClientsOfNewTransaction(expected);
				A.CallTo(() => _fakeSusServiceClient.SendPostRequest(expected)).MustHaveHappenedOnceExactly();
			}
		}

		public class GetAllBlueprints : ActionsControllerTest
		{
			[Fact]
			public async Task Should_Call_GetBlueprints_WithRegisterId()
			{
				var walletAddress = "ws1asdfasdflknki21323lkadfa";
				var result = await _underTest.GetStartingActions(_registerId, walletAddress);
				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(_registerId)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_CallWalletServiceDecrypt_ForEachBlueprint()
			{
				var testBlueprint = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var btTx1 = new TransactionModel()
				{
					TxId = "6771e4f258e3f9d429c80487a266356de4d3df47d5953f314ec02e934d43011c",
					PrevTxId = new string($"").PadLeft(64, '0'),
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint.Id,
						RegisterId = testData.registerId
					}
				};

				var testBlueprint2 = testData.blueprint1(); // does this cause an issue
				var btTx2 = new TransactionModel()
				{
					TxId = "b2de1fb4b860525427c022c25ac993c04ddda19cd00acc9395764417395a03a8",
					PrevTxId = new string($"").PadLeft(64, '0'),
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint2.Id,
						RegisterId = testData.registerId
					}
				};

				var bpBytes = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());
				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(testData.registerId)).Returns(new List<TransactionModel> { btTx1, btTx2 });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new byte[][] { bpBytes });
				var result = await _underTest.GetStartingActions(testData.registerId, testData.walletAddress1);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx1, testData.walletAddress1)).MustHaveHappenedOnceExactly()
					.Then(A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx2, testData.walletAddress1)).MustHaveHappenedOnceExactly());
			}
			[Fact]
			public async Task ShouldOnlyReturnActions_WithSenderMatchingParticipantWallet()
			{
				var participant1 = new Participant
				{
					Id = Guid.NewGuid().ToString(),
					WalletAddress = "wallet1"
				};
				var blueprint1 = new Blueprint
				{
					Id = "testBpId",
					Title = "Test Blueprint 1",
					Description = "A test blueprint",
					Version = 1,
					Actions = new List<Action> { new()
					{
						Id = 1,
						Sender = participant1.Id
					} },
					Participants = new List<Participant> { participant1 },
					DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") }
				};
				var blueprint1Transaction = new TransactionModel
				{
					TxId = "b2de1fb4b860525427c022c25ac993c04ddda19cd00acc9395764417395a03a8",
					PrevTxId = new string($"").PadLeft(64, '0'),
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = blueprint1.Id,
						RegisterId = testData.registerId
					}
				};
				var blueprint1Bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint1));
				var participant2 = new Participant
				{
					Id = Guid.NewGuid().ToString(),
					WalletAddress = "wallet2"
				};

				// Blueprint 2 has a starting action with a sender wallet that isn't owned, so shouldn't be returned
				var blueprint2 = new Blueprint
				{
					Id = "testBpId2",
					Title = "Test Blueprint 2",
					Description = "A test blueprint 2",
					Version = 1,
					Actions = new List<Action> { new()
					{
						Id = 1,
						Sender = participant2.Id
					} },
					Participants = new List<Participant> { participant2 },
					DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") }
				};
				var blueprint2Transaction = new TransactionModel
				{
					TxId = "b2de1fb4b860525427c022c25ac993c04ddda19cd00acc9395764417395a03a9",
					PrevTxId = new string($"").PadLeft(64, '0'),
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = blueprint2.Id,
						RegisterId = testData.registerId
					}
				};
				var blueprint2Bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint2));

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(testData.registerId)).Returns(new List<TransactionModel> {blueprint1Transaction, blueprint2Transaction});
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.That.Matches(t=> t.Id == blueprint1Transaction.Id), blueprint1.Participants.First().WalletAddress)).Returns(new byte[][] { blueprint1Bytes });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.That.Matches(t=> t.Id == blueprint2Transaction.Id), blueprint1.Participants.First().WalletAddress)).Returns(new byte[][] { blueprint2Bytes });
				A.CallTo(() => _fakeWalletServiceClient.ListWallets(false)).Returns(new List<Wallet>{new() {Address = blueprint1.Participants.First().WalletAddress} });

				var result = await _underTest.GetStartingActions(testData.registerId, blueprint1.Participants.First().WalletAddress);
				Assert.Single(result);
				Assert.Equal(blueprint1Transaction.TxId, result.First().Blueprint);
			}
		}

		public class GetAllAsync : ActionsControllerTest
		{
			[Fact]
			public async Task Should_Call_GetPendingTrasanctions_WithWalletAddress()
			{
				await _underTest.GetAllAsync(null, testData.walletAddress1);
				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Call_GetTransactionById_ForEachUnqiqueBlueprintId()
			{
				var testBlueprint = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var btTx1 = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint.Id,
						RegisterId = testData.registerId
					}
				};
				var testBlueprint2 = testData.blueprint1(); // does this cause an issue
				var btTx2 = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint2.Id,
						RegisterId = testData.registerId
					}
				};
				var bpBytes = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(testData.registerId)).Returns(new List<TransactionModel> { btTx1, btTx2 });

				var pendingTx1 = new PendingTransaction
				{
					Id = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData { BlueprintId = testBlueprint.Id, RegisterId = testData.registerId }
				};
				var pendingTx2 = new PendingTransaction
				{
					Id = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData { BlueprintId = testData.blueprint1().Id, RegisterId = testData.registerId }
				};

				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).Returns(new List<PendingTransaction> { pendingTx1, pendingTx2 });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new byte[][] { bpBytes });
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(new TransactionModel() { });

				var fakePayload = new byte[][] { bpBytes };
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(fakePayload);
				await _underTest.GetAllAsync(testData.registerId, testData.walletAddress1);
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testBlueprint.Id)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testData.blueprint1().Id)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Call_ReturnActions_ForEachBlueprint()
			{
				var testBlueprint = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var btTx1 = new TransactionModel()
				{
					TxId = testData.blueprintTxId,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint.Id,
						RegisterId = testData.registerId
					}
				};
				var testBlueprint2 = testData.blueprint1(); // does this cause an issue
				var btTx2 = new TransactionModel()
				{
					TxId = "2b904c90e01d5f0ee4f71754c8872848b17942a01515e99e816c193e6d70f430",
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint2.Id,
						RegisterId = testData.registerId
					}
				};
				var bpBytes = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(testData.registerId)).Returns(new List<TransactionModel> { btTx1, btTx2 });

				var pendingTx1 = new PendingTransaction
				{
					Id = testData.blueprintTxId,
					MetaData = new TransactionMetaData { BlueprintId = testBlueprint.Id, RegisterId = testData.registerId }
				};
				var pendingTx2 = new PendingTransaction
				{
					Id = "2b904c90e01d5f0ee4f71754c8872848b17942a01515e99e816c193e6d70f430",
					MetaData = new TransactionMetaData { BlueprintId = testData.blueprint1().Id, RegisterId = testData.registerId }
				};

				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).Returns(new List<PendingTransaction> { pendingTx1, pendingTx2 });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new byte[][] { bpBytes });
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(new TransactionModel() { });

				var fakePayload = new byte[][] { bpBytes };
				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).Returns(new List<PendingTransaction> { pendingTx1, pendingTx2 });
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, btTx1.TxId)).Returns(btTx1);
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, btTx2.TxId)).Returns(btTx2);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx1, testData.walletAddress1)).Returns(fakePayload);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx2, testData.walletAddress1)).Returns(fakePayload);

				var result = await _underTest.GetAllAsync(testData.registerId, testData.walletAddress1);
				Assert.Contains(result, action => action.Id == result[0].Id);
				Assert.Contains(result, action => action.Id == result[1].Id);
			}
			[Fact]
			public async Task Should_ReturnDifferentPreviousTxIds_WhenActionIdIsTheSame()
			{
				var testBlueprint = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var btTx1 = new TransactionModel()
				{
					TxId = testData.blueprintTxId,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint.Id,
						RegisterId = testData.registerId
					}
				};
				var testBlueprint2 = testData.blueprint1(); // does this cause an issue
				var btTx2 = new TransactionModel()
				{
					TxId = "2b904c90e01d5f0ee4f71754c8872848b17942a01515e99e816c193e6d70f430",
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = testBlueprint2.Id,
						RegisterId = testData.registerId
					}
				};
				var bpBytes = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(testData.registerId)).Returns(new List<TransactionModel> { btTx1, btTx2 });

				var pendingTx1 = new PendingTransaction
				{
					Id = testData.blueprintTxId,
					MetaData = new TransactionMetaData { BlueprintId = testBlueprint.Id, RegisterId = testData.registerId, NextActionId = 1 }
				};
				var pendingTx2 = new PendingTransaction
				{
					Id = "2b904c90e01d5f0ee4f71754c8872848b17942a01515e99e816c193e6d70f430",
					MetaData = new TransactionMetaData { BlueprintId = testData.blueprint1().Id, RegisterId = testData.registerId, NextActionId = 1 }
				};

				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).Returns(new List<PendingTransaction> { pendingTx1, pendingTx2 });
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new byte[][] { bpBytes });
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(new TransactionModel() { });

				var fakePayload = new byte[][] { bpBytes };
				A.CallTo(() => _fakeWalletServiceClient.GetWalletTransactions(testData.walletAddress1)).Returns(new List<PendingTransaction> { pendingTx1, pendingTx2 });
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, btTx1.TxId)).Returns(btTx1);
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, btTx2.TxId)).Returns(btTx2);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx1, testData.walletAddress1)).Returns(fakePayload);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(btTx2, testData.walletAddress1)).Returns(fakePayload);

				var result = await _underTest.GetAllAsync(testData.registerId, testData.walletAddress1);
				Assert.True(result[0].PreviousTxId != result[1].PreviousTxId);
			}
		}

		public class GetById : ActionsControllerTest
		{
			[Fact]
			public async Task Should_GetTransactionById()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				await _underTest.GetById(testData.walletAddress1, testData.registerId, "");
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, dataTx.Id)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_DecryptTargetTransaction()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					TxId = testData.blueprintTxId,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Blueprint,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				await _underTest.GetById(testData.walletAddress1, testData.registerId, testData.blueprintTxId);
				// Why does it think it happens twice?
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(dataTx, testData.walletAddress1)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_ReturnFirstActionFromBlueprint()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				var result = await _underTest.GetById(testData.walletAddress1, testData.registerId, testData.blueprintTxId);
				Assert.Equal(1, result.Id); //the first action
				Assert.Equal(bp.Id, result.Blueprint); // from the same blueprint
			}
			[Fact]
			public async Task Should_GetBlueprintTransactionWhenTargetTransaction_IsAnAction()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				await _underTest.GetById(testData.walletAddress1, testData.registerId, testData.blueprintTxId);
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testData.blueprintTxId)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_ReturnReceivedDataInAction()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				var data = new Dictionary<string, object> {	{ "data1", "somedata" } };
				var expected = data;
				var databytes = JsonSerializer.SerializeToUtf8Bytes(data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testData.blueprintTxId)).Returns(new TransactionModel());

				var blueprint = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new byte[][] { databytes })
					.Once().Then.Returns(new byte[][] { blueprint });
				A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(A<string>.Ignored, A<List<TransactionModel>>.Ignored, _fakeWalletServiceClient))
					.Returns(expected);

				var result = await _underTest.GetById(testData.walletAddress1, testData.registerId, "");
				var resultStr = result.PreviousData.RootElement.GetRawText();
				Assert.Equal(JsonSerializer.Serialize(expected), resultStr);
			}
			[Fact]
			public async Task Should_DecryptBlueprintTransactionWhenTargetTransaction_IsAnAction()
			{
				var bp = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = bp.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				var expected = new TransactionModel();
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, bp.Id)).Returns(expected);
				await _underTest.GetById(testData.walletAddress1, testData.registerId, "");
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(expected, testData.walletAddress1)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task AggregatePreviousTransactionDataFalse_ShouldNotAggregatePreviousTransactionData()
			{
				var testBlueprint = JsonSerializer.Deserialize<Blueprint>(testData.simpleBlueprintStr());
				var dataTx = new TransactionModel()
				{
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action,
						BlueprintId = testBlueprint.Id,
						RegisterId = testData.registerId
					}
				};
				SetupTestForSuccess(dataTx, null);
				var data = new Dictionary<string, object> { { "data1", "somedata" } };
				var data2 = new Dictionary<string, object> { { "data2", "somedata2" } };
				var expectedData = new Dictionary<string, object>
				{
					{ "data1", "somedata" },
					{ "data2", "somedata2" }
				};
				var expected = expectedData;
				var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				var dataBytes2 = JsonSerializer.SerializeToUtf8Bytes(data2, new JsonSerializerOptions(JsonSerializerDefaults.Web));

				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testData.blueprintTxId)).Returns(new TransactionModel());
				var blueprint = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, testData.walletAddress1)).Returns(new[] { dataBytes, dataBytes2 })
					.Once().Then.Returns(new[] { blueprint });
				A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(A<string>.Ignored, A<List<TransactionModel>>.Ignored, _fakeWalletServiceClient))
					.Returns(expected);

				var result = await _underTest.GetById(testData.walletAddress1, testData.registerId, "", false);
				var resultStr = result.PreviousData.RootElement.GetRawText();
				Assert.Equal(JsonSerializer.Serialize(expected), resultStr);
				A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(A<string>.Ignored, A<List<TransactionModel>>.Ignored, A<IWalletServiceClient>.Ignored))
					.MustNotHaveHappened();
			}

			private void SetupTestForSuccess(TransactionModel tx, byte[][] decryptedBlueprintBytes)
			{
				tx ??= new TransactionModel() { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
				var blueprint = Encoding.UTF8.GetBytes(testData.simpleBlueprintStr());
				decryptedBlueprintBytes ??= new byte[][] { blueprint };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(tx);
				if (tx.MetaData.TransactionType == TransactionTypes.Blueprint)
					A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(decryptedBlueprintBytes);
				else
				{
					A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(Array.Empty<byte[]>())
						.Once().Then.Returns(decryptedBlueprintBytes);
				}
			}
		}

		public class RejectAction : ActionsControllerTest
		{
			[Fact]
			public async Task Should_GetPreviousTransaction()
			{
				var expected = new ActionSubmission
				{
					RegisterId = Guid.NewGuid().ToString("N"),
					BlueprintId = Guid.NewGuid().ToString(),
					PreviousTxId = "fb57e2728d7f77d9920a9a7a0cf1e27d03e62053d224082cef2f34c060794cbd",
				};
				var rawTransaction = new TransactionModel { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Action } };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(expected.RegisterId, expected.PreviousTxId)).Returns(rawTransaction);
				await _underTest.RejectAction(expected);
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(expected.RegisterId, expected.PreviousTxId)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_ThrowWhenPreviousTx_IsRejection()
			{
				var expected = new ActionSubmission
				{
					RegisterId = Guid.NewGuid().ToString("N"),
					BlueprintId = Guid.NewGuid().ToString(),
					PreviousTxId = "fb57e2728d7f77d9920a9a7a0cf1e27d03e62053d224082cef2f34c060794cbd",
				};
				var rawTransaction = new TransactionModel { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Rejection } };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(expected.RegisterId, expected.PreviousTxId)).Returns(rawTransaction);
				await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.RejectAction(expected));
			}
			[Fact]
			public async Task Should_ThrowWhenPrevTx_IsNotActionOrRejectionTX()
			{
				var expected = new ActionSubmission
				{
					RegisterId = Guid.NewGuid().ToString("N"),
					BlueprintId = Guid.NewGuid().ToString(),
					PreviousTxId = "fb57e2728d7f77d9920a9a7a0cf1e27d03e62053d224082cef2f34c060794cbd",
				};
				var rawTransaction = new TransactionModel { MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(expected.RegisterId, expected.PreviousTxId)).Returns(rawTransaction);
				await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.RejectAction(expected));
			}
			[Fact]
			public async Task Should_CallCreateAndSendTransaction()
			{
				var actionSubmission = new ActionSubmission
				{
					RegisterId = Guid.NewGuid().ToString("N"),
					BlueprintId = Guid.NewGuid().ToString(),
					PreviousTxId = "fb57e2728d7f77d9920a9a7a0cf1e27d03e62053d224082cef2f34c060794cbd",
					Data = JsonDocument.Parse("{}"),
					WalletAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj"
				};
				var rawTransaction = new TransactionModel
				{
					TxId = actionSubmission.PreviousTxId,
					SenderWallet = "ws1j5lpz5gwn9vj6dqtyjeh6tkfusl48jkycn2ql2c3yprwhqr40q9xq49mzlr",
					MetaData = new TransactionMetaData
					{
						ActionId = 2,
						NextActionId = 3,
						TransactionType = TransactionTypes.Action,
						InstanceId = Guid.NewGuid().ToString(),
						RegisterId = actionSubmission.RegisterId
					}
				};
				TransactionMetaData meta_data = new()
				{
					NextActionId = 2,
					ActionId = 3,
					BlueprintId = actionSubmission.BlueprintId,
					InstanceId = rawTransaction.MetaData.InstanceId,
					RegisterId = rawTransaction.MetaData.RegisterId,
					TransactionType = TransactionTypes.Rejection
				};

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetPrevTxHash(rawTransaction.TxId);
				transaction.SetTxRecipients(new string[] { rawTransaction.SenderWallet });
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
				transaction.GetTxPayloadManager().AddPayload(JsonSerializer.SerializeToUtf8Bytes(actionSubmission.Data), new string[] { rawTransaction.SenderWallet });
				TransactionModel expected = TransactionFormatter.ToModel(transaction.GetTxTransport().transport);

				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(actionSubmission.RegisterId, actionSubmission.PreviousTxId)).Returns(rawTransaction);
				await _underTest.RejectAction(actionSubmission);
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).MetaData.TransactionType == expected.MetaData.TransactionType
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).PrevTxId == expected.PrevTxId
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).MetaData.ActionId == expected.MetaData.ActionId
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).MetaData.NextActionId == expected.MetaData.NextActionId
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).RecipientsWallets.Count() == expected.RecipientsWallets.Count()
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr =>
					TransactionFormatter.ToModel(txr).Payloads[0].WalletAccess[0].Equals(expected.Payloads[0].WalletAccess[0])
					), actionSubmission.WalletAddress))
					.MustHaveHappenedOnceExactly();
			}
		}

		public class PostResponse : ActionsControllerTest
		{
			[Fact]
			public async Task Should_DecryptBlueprintTransaction()
			{
				SetupTestForSuccess(testData.registerId);
				var actionSubmission = new ActionSubmission
				{
					BlueprintId = testData.blueprintTxId,
					Data = JsonDocument.Parse("{}"),
					PreviousTxId = testData.blueprintTxId,
					RegisterId = testData.registerId,
					WalletAddress = testData.walletAddress1
				};
				var expected = new TransactionModel();
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(testData.blueprint1()) };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(testData.registerId, testData.blueprintTxId)).Returns(expected);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
				await _underTest.PostResponse(actionSubmission);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(expected, testData.walletAddress1)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_ReturnBadRequest_WhenActionDataIsInvalid()
			{
				SetupTestForSuccess();
				var validationMessage = "some validation message";
				var walletAddress = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
				var txId = "blueprinttxid";
				var senderId = "senderid";
				var submittedData = JsonDocument.Parse("{}");
				var action = new Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
				A.CallTo(() => _fakeSchemaValidator.ValidateSchemaData(A<string>.Ignored, A<string>.Ignored)).Returns((false, validationMessage));
				var response = await _underTest.PostResponse(new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				});
				Assert.IsType<BadRequestObjectResult>(response.Result);
				var badObject = (BadRequestObjectResult)response.Result;
				var message = (string)badObject.Value;
				Assert.Equal(message, validationMessage);
			}
			[Fact]
			public async Task ShouldThrow_WhenUserIsNotAuthorisedForRegister() { await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.PostResponse(new())); }
			[Fact]
			public async Task Should_CreateAndSendTransaction()
			{
				SetupTestForSuccess();
				var walletAddress = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
				var txId = "blueprinttxid";
				var senderId = "senderid";
				var submittedData = JsonDocument.Parse("{}");
				var action = new Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				var actionSubmission = new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				};
				TransactionMetaData meta_data = new()
				{
					BlueprintId = txId,
					NextActionId = 2,
					ActionId = 1,
					TransactionType = TransactionTypes.Action,
					RegisterId = _registerId,
				};

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetPrevTxHash(txId);
				transaction.SetTxRecipients(new string[] { walletAddress });
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
				transaction.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { walletAddress });
				Transaction tx = transaction.GetTxTransport().transport;

				A.CallTo(() => _fakeTransactionRequestBuilder.BuildTransactionRequest(A<Blueprint>.Ignored, actionSubmission)).Returns(tx);
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
				await _underTest.PostResponse(actionSubmission);
				RunCreateAndSendAssertions(tx, walletAddress, meta_data.NextActionId);
			}
			[Fact]
			public async Task Should_CreateAndSendTransactionWhenPreviousTxIsAction()
			{
				SetupTestForSuccess();
				var walletAddress = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
				var txId = "blueprinttxid";
				var senderId = "senderid";
				var previousActionId = "prevactionid";
				var submittedData = JsonDocument.Parse("{}");
				var action = new Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				var actionSubmission = new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = previousActionId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				};
				TransactionMetaData meta_data = new()
				{
					BlueprintId = txId,
					NextActionId = 2,
					ActionId = 1,
					TransactionType = TransactionTypes.Action,
					RegisterId = _registerId,
				};

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.SetPrevTxHash(previousActionId);
				transaction.SetTxRecipients(new string[] { walletAddress });
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
				transaction.GetTxPayloadManager().AddPayload(new byte[] { 0x01 }, new string[] { walletAddress });
				Transaction tx = transaction.GetTxTransport().transport;

				var prevTx = new TransactionModel { MetaData = new TransactionMetaData { InstanceId = string.Empty } };
				A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(_registerId, previousActionId)).Returns(prevTx);
				A.CallTo(() => _fakeTransactionRequestBuilder.BuildTransactionRequestFromPreviousTransaction(A<Blueprint>.Ignored, actionSubmission, prevTx)).Returns(tx);
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
				await _underTest.PostResponse(actionSubmission);
				RunCreateAndSendAssertions(tx, walletAddress, meta_data.NextActionId);
			}

			private void RunCreateAndSendAssertions(Transaction tx, string walletAddress, int nextActionId)
			{
				var expected = TransactionFormatter.ToModel(tx);
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).MetaData.TransactionType == expected.MetaData.TransactionType), walletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).PrevTxId == expected.PrevTxId), walletAddress)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).MetaData.ActionId == expected.MetaData.ActionId), walletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).MetaData.NextActionId == nextActionId), walletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).RecipientsWallets.Count() == expected.RecipientsWallets.Count()), walletAddress))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txr => TransactionFormatter.ToModel(txr).Payloads[0].WalletAccess[0] == expected.Payloads[0].WalletAccess[0]), walletAddress))
					.MustHaveHappenedOnceExactly();
			}

			private void SetupTestForSuccess(string registerId = _registerId)
			{
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(new Blueprint()) };
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, A<string>.Ignored)).Returns(byteArray);
				A.CallTo(() => _fakeSchemaValidator.ValidateSchemaData(A<string>.Ignored, A<string>.Ignored)).Returns((true, "validation error:"));
				A.CallTo(() => _fakeTenantServiceClient.GetTenantById(A<string>._))
					.Returns(new Tenant() { Id = _expectedTenant, Registers = new List<string>() { registerId } });
			}
		}
	}
}
