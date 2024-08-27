using ActionService.V1.Repositories;
using Siccar.Application;
using Siccar.Application.Models;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
#nullable enable

namespace ActionService.V1.Services
{
	public class TransactionRequestBuilder : ITransactionRequestBuilder
	{
		private readonly IFileRepository _fileRepository;
		private readonly IPayloadResolver _payloadResolver;
		private readonly IActionResolver _actionResolver;
		private readonly IWalletServiceClient _walletServiceClient;
		private readonly ICalculationService _calculationService;

		public TransactionRequestBuilder(IFileRepository fileRepository, IPayloadResolver payloadResolver, IActionResolver actionResolver, IWalletServiceClient walletServiceClient,
			ICalculationService calculationService)
		{
			_fileRepository = fileRepository;
			_payloadResolver = payloadResolver;
			_actionResolver = actionResolver;
			_walletServiceClient = walletServiceClient;
			_calculationService = calculationService;
		}

		public async Task<Transaction> BuildTransactionRequest(Blueprint blueprint, ActionSubmission actionSubmission)
		{
			var action = blueprint.Actions.FirstOrDefault(action => action.Id == 1);
			var trackingData = _payloadResolver.ResolveTrackingData(action!.Disclosures, actionSubmission.Data!);
			var instanceId = Guid.NewGuid().ToString();
			var updatedActionSubmissionWithFileTransactionId = await BuildAndSendFileTransactionRequests(blueprint, action!, actionSubmission);
			return await AssembleActionTransactionRequest(blueprint, action!, updatedActionSubmissionWithFileTransactionId, instanceId, trackingData);
		}

		public async Task<Transaction> BuildTransactionRequestFromPreviousTransaction(Blueprint blueprint, ActionSubmission actionSubmission, TransactionModel previousActionTx)
		{
			var instanceId = previousActionTx.MetaData!.InstanceId;
			var currentActionId = previousActionTx.MetaData.NextActionId;
			var action = blueprint.Actions.FirstOrDefault(action => action.Id == currentActionId);
			var trackingData = _payloadResolver.ResolveTrackingData(action!.Disclosures, actionSubmission.Data!);
			if (previousActionTx.MetaData.TrackingData != null)
				trackingData = CopyTrackingDataFromPreviousTransaction(trackingData, previousActionTx);
			var updatedActionSubmissionWithFileTransactionId = await BuildAndSendFileTransactionRequests(blueprint, action!, actionSubmission);
			return await AssembleActionTransactionRequest(blueprint, action!, updatedActionSubmissionWithFileTransactionId, instanceId, trackingData);
		}

		private List<string> AddRecipientsToRequest(Blueprint blueprint, Siccar.Application.Action action, int nextActionId)
		{
			var nextAction = _actionResolver.ResolveNextAction(nextActionId, blueprint);
			var nextParticipantWalletAddress = _actionResolver.ResolveParticipantWalletAddress(nextAction.Sender, blueprint);
			var listWalletAddresses = new List<string> { nextParticipantWalletAddress };
			foreach (var recipient in action.AdditionalRecipients)
			{
				var recipientWalletAddress = _actionResolver.ResolveParticipantWalletAddress(recipient, blueprint);
				if (recipientWalletAddress != nextParticipantWalletAddress)
					listWalletAddresses.Add(recipientWalletAddress);
			}
			return listWalletAddresses;
		}

		private async Task<Transaction> AssembleActionTransactionRequest(Blueprint blueprint, Siccar.Application.Action action, ActionSubmission actionSubmission,
			string instanceId, SortedList<string, string> trackingData)
		{
			var updatedSubmissionData = await _calculationService.RunActionCalculationsAsync(action, actionSubmission, instanceId);
			ITxFormat transaction = _payloadResolver.AddParticipantPayloadsToTransaction(blueprint, action.Disclosures, updatedSubmissionData, action.Sender);
			var isLastAction = !_actionResolver.IsFinalAction(action, blueprint, actionSubmission.Data, out var nextActionId);

			TransactionMetaData meta_data = new()
			{
				BlueprintId = actionSubmission.BlueprintId,
				InstanceId = instanceId,
				ActionId = action.Id,
				NextActionId = nextActionId,
				RegisterId = actionSubmission.RegisterId,
				TransactionType = TransactionTypes.Action,
				TrackingData = trackingData
			};

			transaction.SetPrevTxHash(actionSubmission.PreviousTxId);
			if (isLastAction)
				transaction.SetTxRecipients(AddRecipientsToRequest(blueprint, action, nextActionId).ToArray());
			transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
			return transaction.GetTxTransport().transport!;
		}

		private static SortedList<string, string> CopyTrackingDataFromPreviousTransaction(SortedList<string, string> trackingData, TransactionModel previousActionTx)
		{
			trackingData ??= new SortedList<string, string>();
			var previousTrackingData = previousActionTx.MetaData!.TrackingData;

			//Combine any previous tracking data
			previousTrackingData.ToList().ForEach(dataKVP =>
			{
				if (string.IsNullOrEmpty(trackingData[dataKVP.Key]))
					trackingData[dataKVP.Key] = dataKVP.Value;
			});
			return trackingData;
		}

		private async Task<ActionSubmission> BuildAndSendFileTransactionRequests(Blueprint blueprint, Siccar.Application.Action action, ActionSubmission actionSubmission)
		{
			var containsDataSchemas = action.DataSchemas!.First().RootElement.TryGetProperty("properties", out var dataSchemas);
			if (!containsDataSchemas)
				return actionSubmission;
			var fileKeysAndNames = new Dictionary<string, FileMetaData>();
			var dataDict = JsonSerializer.Deserialize<SortedDictionary<string, JsonElement>>(actionSubmission.Data!.RootElement.GetRawText());
			foreach (var schema in dataSchemas.EnumerateObject())
			{
				var schemaProperty = JsonDocument.Parse(schema.Value.GetRawText());
				schemaProperty.RootElement.TryGetProperty("type", out var typeValue);
				var containsFileProperties = schemaProperty.RootElement.TryGetProperty("properties", out var fileProperties);
				if (typeValue.ToString() == "object" && containsFileProperties && fileProperties.TryGetProperty("fileName", out var _))
				{
					var fileSchemaId = schemaProperty.RootElement.GetProperty("$id");
					var actionSubmissionContainsFileMetaData = dataDict!.TryGetValue(fileSchemaId.ToString(), out var fileMetaData);
					if (actionSubmissionContainsFileMetaData && fileMetaData.GetRawText().Contains("fileName"))
					{
						var metaData = JsonSerializer.Deserialize<FileMetaData>(fileMetaData);
						fileKeysAndNames.Add(fileSchemaId.ToString(), metaData!);
					}
				}
			}

			foreach (var fileKeyAndName in fileKeysAndNames)
			{
				ITxFormat transaction = await _payloadResolver.AddFilePayloadToTransaction(blueprint, action.Disclosures, fileKeyAndName.Value.Name!, fileKeyAndName.Key);
				TransactionMetaData meta_data = new()
				{
					BlueprintId = actionSubmission.BlueprintId,
					ActionId = action.Id,
					RegisterId = actionSubmission.RegisterId,
					TransactionType = TransactionTypes.File,
					TrackingData = new SortedList<string, string>
					{
						{"fileName" , fileKeyAndName.Value.Name! },
						{"fileType" , fileKeyAndName.Value.Type! },
						{"fileSize" , fileKeyAndName.Value.Size.ToString()! },
						{"fileExtension" , fileKeyAndName.Value.Extension!.ToString() },
					}
				};
				transaction.SetPrevTxHash(actionSubmission.PreviousTxId);
				transaction.SetTxMetaData(JsonSerializer.Serialize(meta_data));
				var response = await _walletServiceClient.SignAndSendTransaction(transaction.GetTxTransport().transport, actionSubmission.WalletAddress);
				await _fileRepository.DeleteFile(fileKeyAndName.Value.Name);
				UpdateActionSubmissionWithFileTransactionId(dataDict!, fileKeyAndName.Key, response.TxId);
			}
			actionSubmission.Data = JsonDocument.Parse(JsonSerializer.Serialize(dataDict));
			return actionSubmission;
		}

		private static SortedDictionary<string, JsonElement> UpdateActionSubmissionWithFileTransactionId(SortedDictionary<string, JsonElement> dataDict, string fileKeyName, string transactionId)
		{
			var actionSubmissionContainsFileMetaData = dataDict.TryGetValue(fileKeyName, out var fileMetaData);
			if (actionSubmissionContainsFileMetaData)
			{
				var metaData = JsonSerializer.Deserialize<FileMetaData>(fileMetaData);
				metaData!.TransactionId = transactionId;
				dataDict[fileKeyName] = JsonDocument.Parse(JsonSerializer.Serialize(metaData)).RootElement;
			}
			return dataDict;
		}
	}
}
