using ActionService.V1.Repositories;
using ActionService.V1.Services;
using FakeItEasy;
using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace ActionUnitTests.V1.Services
{
	public class TransactionRequestBuilderTest
	{
		private readonly ITransactionRequestBuilder _underTest;
		private readonly IPayloadResolver _fakePayloadResolver;
		private readonly IActionResolver _fakeActionResolver;
		private readonly IFileRepository _fakeFileRepository;
		private readonly IWalletServiceClient _fakeWalletServiceClient;
		private readonly ICalculationService _fakeCalculationService;
		private const string _fileId = "file1";
		private const string _fileName = "file1.jpg";
		private const string _file2Id = "file2";
		private const string _file2Name = "file2.jpg";
		private const string _registerId = "0fb3b0c0-4972-402c-bbec-99bb41ab7d28";

		public TransactionRequestBuilderTest()
		{
			_fakePayloadResolver = A.Fake<IPayloadResolver>();
			_fakeFileRepository = A.Fake<IFileRepository>();
			_fakeActionResolver = A.Fake<IActionResolver>();
			_fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
			_fakeCalculationService = A.Fake<ICalculationService>();
			_underTest = new TransactionRequestBuilder(_fakeFileRepository, _fakePayloadResolver, _fakeActionResolver, _fakeWalletServiceClient, _fakeCalculationService);
		}

		public class BuildTransactionRequest : TransactionRequestBuilderTest
		{
			[Fact]
			public void Should_ResolveNextAction()
			{
				var walletAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse("{}");
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				int nextActionId;

				A.CallTo(() => _fakeActionResolver.IsFinalAction(A<Siccar.Application.Action>.Ignored, A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id), submittedData, out nextActionId))
					.Returns(false)
					.AssignsOutAndRefParameters(action.Id);

				_underTest.BuildTransactionRequest(blueprint, new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				});
				A.CallTo(() => _fakeActionResolver.ResolveNextAction(action.Id, A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id))).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_ResolveParticipantPayloadsAsync()
			{
				var walletAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse("{}");
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				A.CallTo(() => _fakeCalculationService.RunActionCalculationsAsync(action, A<ActionSubmission>._, A<string>._)).Returns(submittedData);
				await _underTest.BuildTransactionRequest(blueprint, new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				});
				A.CallTo(() => _fakePayloadResolver.AddParticipantPayloadsToTransaction(A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id), A<IEnumerable<Disclosure>>.Ignored, submittedData, senderId))
					.MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldBuildTransactinRequestFromBlueprintAsync()
			{
				var walletAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = "5b96d651-d349-451d-82e8-5bc4983d12b1";
				var submittedData = JsonDocument.Parse("{}");
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { walletAddress });
				TransactionMetaData meta_data = new()
				{
					BlueprintId = txId,
					NextActionId = 2,
					ActionId = 1,
					TransactionType = TransactionTypes.Action,
					RegisterId = _registerId,
				};
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				var actionSubmission = new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				};

				A.CallTo(() => _fakeActionResolver.ResolveNextAction(action.Id, blueprint)).Returns(new Siccar.Application.Action { Id = 2, Sender = senderId });
				A.CallTo(() => _fakeCalculationService.RunActionCalculationsAsync(action, actionSubmission, A<string>._)).Returns(submittedData);
				A.CallTo(() => _fakePayloadResolver.AddParticipantPayloadsToTransaction(
					A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id),
					A<IEnumerable<Disclosure>>.That.Matches(di => di.SequenceEqual(action.Disclosures)),
					submittedData,
					senderId))
					.Returns(transaction);
				A.CallTo(() => _fakeActionResolver.ResolveParticipantWalletAddress(A<string>.Ignored, A<Blueprint>.Ignored)).Returns(walletAddress);
				int nextActionId;
				A.CallTo(() => _fakeActionResolver.IsFinalAction(A<Siccar.Application.Action>.Ignored, A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id), submittedData, out nextActionId))
					.Returns(false)
					.AssignsOutAndRefParameters(meta_data.NextActionId);

				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				TransactionModel? model = TransactionFormatter.ToModel(result);
				Assert.Equal(transaction.GetTxPayloadManager().GetPayloadInfo(1).info!.GetPayloadWallets(), model!.Payloads[0].WalletAccess);
				Assert.Equal(walletAddress, model.RecipientsWallets.First());
				Assert.Equal(txId, model.PrevTxId);
				Assert.Equal(meta_data.Id, model?.MetaData?.Id);
				Assert.Equal(meta_data.ActionId, model?.MetaData?.ActionId);
				Assert.Equal(meta_data.TrackingData, model?.MetaData?.TrackingData);
				Assert.Equal(meta_data.TransactionType, model?.MetaData?.TransactionType);
				Assert.Equal(meta_data.BlueprintId, model?.MetaData?.BlueprintId);
				Assert.Equal(meta_data.NextActionId, model?.MetaData?.NextActionId);
				Assert.Equal(meta_data.RegisterId, model?.MetaData?.RegisterId);
			}
			[Fact]
			public async Task ShouldBuildTransactinRequestWithAdditionalRecipients()
			{
				var walletAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var expectedWallet = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = "5b96d651-d349-451d-82e8-5bc4983d12b1";
				var additionalRecipientId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse("{}");
				var action = new Siccar.Application.Action { Id = 1, AdditionalRecipients = new List<string> { additionalRecipientId }, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
				var blueprint = new Blueprint();
				blueprint.Actions.Add(action);
				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { walletAddress });
				TransactionMetaData meta_data = new()
				{
					BlueprintId = txId,
					NextActionId = 2,
					ActionId = 1,
					TransactionType = TransactionTypes.Action,
					RegisterId = _registerId,
				};
				var byteArray = new byte[][] { JsonSerializer.SerializeToUtf8Bytes(blueprint) };
				var actionSubmission = new ActionSubmission
				{
					BlueprintId = txId,
					Data = submittedData,
					PreviousTxId = txId,
					RegisterId = _registerId,
					WalletAddress = walletAddress
				};

				A.CallTo(() => _fakeCalculationService.RunActionCalculationsAsync(action, actionSubmission, A<string>._)).Returns(submittedData);
				A.CallTo(() => _fakeActionResolver.ResolveNextAction(action.Id, blueprint)).Returns(new Siccar.Application.Action { Id = 2, Sender = senderId });
				A.CallTo(() => _fakePayloadResolver.AddParticipantPayloadsToTransaction(
					A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id),
					A<IEnumerable<Disclosure>>.That.Matches(di => di.SequenceEqual(action.Disclosures)),
					submittedData,
					senderId))
					.Returns(transaction);

				A.CallTo(() => _fakeActionResolver.ResolveParticipantWalletAddress(A<string>.Ignored, A<Blueprint>.Ignored)).Returns(walletAddress);
				A.CallTo(() => _fakeActionResolver.ResolveParticipantWalletAddress(additionalRecipientId, A<Blueprint>.Ignored)).Returns(expectedWallet);
				int nextActionId;
				A.CallTo(() => _fakeActionResolver.IsFinalAction(A<Siccar.Application.Action>.Ignored, A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id), submittedData, out nextActionId))
					.Returns(false)
					.AssignsOutAndRefParameters(meta_data.NextActionId);

				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				Assert.Contains(expectedWallet, TransactionFormatter.ToModel(result)!.RecipientsWallets);
			}
		}

		public class BuildTransactionRequestFromPreviousTransaction : TransactionRequestBuilderTest
		{
			[Fact]
			public async void ShouldBuildTransactinRequestFromBlueprintUsingPreviousActionTx()
			{
				var walletAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var previousActionId = "4ffabbab4e763202462df1f59811944121588f0567f55bce581a0e99ebcf6606";
				var senderId = "5b96d651-d349-451d-82e8-5bc4983d12b1";
				var submittedData = JsonDocument.Parse("{}");
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = previousActionId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse("{}") } };
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

				ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				transaction.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { walletAddress });
				TransactionMetaData meta_data = new()
				{
					BlueprintId = txId,
					NextActionId = -1,
					ActionId = 1,
					TransactionType = TransactionTypes.Action,
					RegisterId = _registerId,
					InstanceId = Guid.NewGuid().ToString(),
				};

				A.CallTo(() => _fakeCalculationService.RunActionCalculationsAsync(action, actionSubmission, A<string>._)).Returns(submittedData);
				A.CallTo(() => _fakePayloadResolver.AddParticipantPayloadsToTransaction(
					A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id),
					A<IEnumerable<Disclosure>>.That.Matches(di => di.SequenceEqual(action.Disclosures)),
					submittedData,
					senderId))
					.Returns(transaction);

				var prevTx = new TransactionModel { MetaData = new TransactionMetaData { InstanceId = meta_data.InstanceId } };
				int nextActionId;
				A.CallTo(() => _fakeActionResolver.IsFinalAction(A<Siccar.Application.Action>.Ignored, A<Blueprint>.That.Matches(bp => bp.Id == blueprint.Id), submittedData, out nextActionId))
					.Returns(true)
					.AssignsOutAndRefParameters(-1);

				var result = await _underTest.BuildTransactionRequestFromPreviousTransaction(blueprint, actionSubmission, prevTx);
				TransactionModel? model = TransactionFormatter.ToModel(result);
				Assert.Equal(transaction.GetTxPayloadManager().GetPayloadInfo(1).info!.GetPayloadWallets(), model!.Payloads[0].WalletAccess);
				Assert.Empty(model.RecipientsWallets);
				Assert.Equal(previousActionId, model.PrevTxId);
				Assert.Equal(JsonSerializer.Serialize(meta_data), JsonSerializer.Serialize(model.MetaData));
			}
		}

		public class BuildFileTransactionRequests : TransactionRequestBuilderTest
		{
			[Fact]
			public async Task ShouldReturnEmptyListWhenNoFilesAreInSchema()
			{
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse("{}");
				var dataSchema = BuildDataSchemaWithoutFile();
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) } };
				var blueprint = new Blueprint();
				var actionSubmission = new ActionSubmission() { Data = submittedData };
				blueprint.Actions.Add(action);
				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(A<Transaction>._, A<string>._)).MustNotHaveHappened();
			}
			[Fact]
			public async Task ShouldCallPayloadResolverWithFileNamesAndIds()
			{
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse($$"""
				{
					"{{_fileId}}":{"fileName":"{{_fileName}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg", "fileTransactionId":null },
					"{{_file2Id}}": {"fileName":"{{_file2Name}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg", "fileTransactionId":null}
				}
				""");

				var dataSchema = BuildDataSchemaString(_fileId, _file2Id);
				var action = new Siccar.Application.Action { Id = 1, Blueprint = txId, Sender = senderId, Disclosures = new List<Disclosure>(), PreviousTxId = txId, DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) } };
				var blueprint = new Blueprint();
				var actionSubmission = new ActionSubmission() { Data = submittedData };
				blueprint.Actions.Add(action);
				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _fileName, _fileId)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _file2Name, _file2Id)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldBuildFilePayloadsWithExpectedWalletAddresses()
			{
				var senderAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var recipientAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse($$"""
				{
					"{{_fileId}}":{"fileName":"{{_fileName}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg",  "fileExtension": ".jpg","fileTransactionId":null },
					"{{_file2Id}}": {"fileName":"{{_file2Name}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg",  "fileExtension": ".jpg","fileTransactionId":null}
				}
				""");

				var dataSchema = BuildDataSchemaString(_fileId, _file2Id);
				var action = new Siccar.Application.Action
				{
					Id = 1,
					Blueprint = txId,
					Sender = senderId,
					Disclosures = new List<Disclosure>(),
					PreviousTxId = txId,
					DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) }
				};
				var blueprint = new Blueprint();
				var actionSubmission = new ActionSubmission() { Data = submittedData };
				ITxFormat tx1 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx1.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { senderAddress, recipientAddress });
				ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx2.GetTxPayloadManager().AddPayload(new byte[] { 2 }, new string[] { senderAddress });
				blueprint.Actions.Add(action);

				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _fileName, _fileId))
					.Returns(tx1);
				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _file2Name, _file2Id))
					.Returns(tx2);

				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest =>
					TransactionFormatter.ToModel(txRequest)!.Payloads[0].WalletAccess.Length == 2 &&
					TransactionFormatter.ToModel(txRequest)!.Payloads[0].WalletAccess[0].Equals(tx1.GetTxPayloadManager().GetPayloadInfo(1).info!.GetPayloadWallets()[0]) &&
					TransactionFormatter.ToModel(txRequest)!.Payloads[0].WalletAccess[1].Equals(tx1.GetTxPayloadManager().GetPayloadInfo(1).info!.GetPayloadWallets()[1])
					), A<string>._)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest =>
					TransactionFormatter.ToModel(txRequest)!.Payloads[0].WalletAccess.Length == 1 &&
					TransactionFormatter.ToModel(txRequest)!.Payloads[0].WalletAccess[0].Equals(tx2.GetTxPayloadManager().GetPayloadInfo(1).info!.GetPayloadWallets()[0])
					), A<string>._)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldCallDeleteFileAfterFileTransactionSent()
			{
				var senderAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var recipientAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse($$"""
				{
					"{{_fileId}}":{"fileName":"{{_fileName}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg", "fileTransactionId":null },
					"{{_file2Id}}": {"fileName":"{{_file2Name}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg", "fileTransactionId":null}
				}
				""");

				var dataSchema = BuildDataSchemaString(_fileId, _file2Id);
				var action = new Siccar.Application.Action
				{
					Id = 1,
					Blueprint = txId,
					Sender = senderId,
					Disclosures = new List<Disclosure>(),
					PreviousTxId = txId,
					DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) }
				};
				var blueprint = new Blueprint();
				var actionSubmission = new ActionSubmission() { Data = submittedData };
				ITxFormat tx1 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx1.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { senderAddress, recipientAddress });
				ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx2.GetTxPayloadManager().AddPayload(new byte[] { 2 }, new string[] { senderAddress });
				blueprint.Actions.Add(action);

				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _fileName, _fileId))
					.Returns(tx1);
				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _file2Name, _file2Id))
					.Returns(tx2);

				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				A.CallTo(() => _fakeFileRepository.DeleteFile(_fileName)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeFileRepository.DeleteFile(_file2Name)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldContainFileMetaDataInRequest()
			{
				var senderAddress = "ws1jx0g70a4cnu796zucchezx9zsd7dgfjt6c58vdrenw6ztk55ayxzsnnxluu";
				var recipientAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
				var txId = "ccde1299bbd3c204d4e1c7a2ee66a8a91d78095d8cadb1e5eddf89f18c446b83";
				var senderId = Guid.NewGuid().ToString();
				var submittedData = JsonDocument.Parse($$"""
				{
					"{{_fileId}}":{"fileName":"{{_fileName}}","fileType":"image/jpeg","fileSize":206297, "fileExtension": ".jpg", "fileTransactionId":null },
					"{{_file2Id}}": {"fileName":"{{_file2Name}}","fileType":"image/jpeg","fileSize":206296, "fileExtension": ".jpg","fileTransactionId":null}
				}
				""");

				var dataSchema = BuildDataSchemaString(_fileId, _file2Id);
				var action = new Siccar.Application.Action
				{
					Id = 1,
					Blueprint = txId,
					Sender = senderId,
					Disclosures = new List<Disclosure>(),
					PreviousTxId = txId,
					DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) }
				};
				var blueprint = new Blueprint() { Actions = new() { action } };
				var actionSubmission = new ActionSubmission() { Data = submittedData };
				ITxFormat tx1 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx1.GetTxPayloadManager().AddPayload(new byte[] { 1 }, new string[] { senderAddress, recipientAddress });
				ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
				tx2.GetTxPayloadManager().AddPayload(new byte[] { 2 }, new string[] { senderAddress });

				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _fileName, _fileId))
					.Returns(tx1);
				A.CallTo(() => _fakePayloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, _file2Name, _file2Id))
					.Returns(tx2);

				var result = await _underTest.BuildTransactionRequest(blueprint, actionSubmission);
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileName", _fileName))), A<string>._))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileType", "image/jpeg"))), A<string>._));
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileSize", "206297"))), A<string>._))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileExtension", ".jpg"))), A<string>._));
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileName", _file2Name))), A<string>._))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileType", "image/jpeg"))), A<string>._));
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileSize", "206296"))), A<string>._))
					.MustHaveHappenedOnceExactly();
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(
					A<Transaction>.That.Matches(txRequest => TransactionFormatter.ToModel(txRequest)!.MetaData!.TrackingData.Contains(new KeyValuePair<string, string>("fileExtension", ".jpg"))), A<string>._));
			}
		}

		private static string BuildDataSchemaString(string fileId1, string fileId2)
		{
			return $$"""
			{
				"$schema": "http://json-schema.org/draft-07/schema",
				"$id": "https://siccar.net/",
				"type": "object",
				"required": [
					"file"
				],
				"properties": {
					"{{fileId1}}": {
						"$id": "{{fileId1}}",
						"type": "object",
						"title": "file",
						"description": "Data",
						"required": [
							"fileName",
							"fileType",
							"fileSize"
						],
						"properties": {
							"fileName": {
								"$id": "fileName",
								"type": "string",
								"description": "The name of the file uploaded."
							},
							"fileType": {
								"$id": "fileType",
								"type": "string",
								"description": "The type of the file uploaded."
							},
							"fileSize": {
								"$id": "fileSize",
								"type": "integer",
								"description": "The size of the file uploaded in bytes."
							}
						}
					},
					"{{fileId2}}": {
						"$id": "{{fileId2}}",
						"type": "object",
						"title": "file2",
						"description": "Data",
						"required": [
							"fileName",
							"fileType",
							"fileSize"
						],
						"properties": {
							"fileName": {
								"$id": "fileName",
								"type": "string",
								"description": "The name of the file uploaded."
							},
							"fileType": {
								"$id": "fileType",
								"type": "string",
								"description": "The type of the file uploaded."
							},
							"fileSize": {
								"$id": "fileSize",
								"type": "integer",
								"description": "The size of the file uploaded in bytes."
							}
						}
					}
				}
			}
			""";
		}

		private static string BuildDataSchemaWithoutFile()
		{
			return $$"""
			{
				"$schema": "http://json-schema.org/draft-07/schema",
				"$id": "https://siccar.net/",
				"type": "object",
				"required": [
					"file"
				],
				"properties": {
					"dummyData": {
						"$id": "dummyData",
						"type": "string",
						"title": "dummyData",
						"description": "dummyData"
					}
				}
			}
			""";
		}
	}
}
