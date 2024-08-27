using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Siccar.Application;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
#nullable enable

namespace ActionService.V1.Services
{
	public interface IActionService
	{
		public Task<Action> GetSubmittedActionData(int actionId, string walletAddress, TransactionModel transaction);
		Task<Action> GetAction(TransactionModel transaction, string walletAddress, string registerId, string transactionId, bool aggregatePreviousTransactionData);
	}

	public class ActionService : IActionService
	{
		private readonly IWalletServiceClient _walletServiceClient;
		private readonly IRegisterServiceClient _registerServiceClient;
		private readonly IPayloadResolver _payloadResolver;
		private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

		public ActionService(IWalletServiceClient walletServiceClient, IRegisterServiceClient registerServiceClient, IPayloadResolver payloadResolver)
		{
			_walletServiceClient = walletServiceClient;
			_registerServiceClient = registerServiceClient;
			_payloadResolver = payloadResolver;
		}

		public async Task<Action> GetSubmittedActionData(int actionId, string walletAddress, TransactionModel transaction)
		{
			Blueprint blueprint;
			var isBlueprintTransaction = transaction.MetaData!.TransactionType == TransactionTypes.Blueprint;
			if (isBlueprintTransaction)
				throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "Blueprint transaction has no submitted action data.");

			blueprint = (await GetBlueprintJsonAsync(transaction.MetaData.BlueprintId, walletAddress, transaction.MetaData.RegisterId))
				.Deserialize<Blueprint>(_jsonSerializerOptions)!;

			var action = blueprint.Actions.FirstOrDefault(a => a.Id == actionId);
			var allPrevTransactions = await _registerServiceClient.GetTransactionsByInstanceId(transaction.MetaData.RegisterId, transaction.MetaData.InstanceId);
			var previousData = await _payloadResolver.GetAllPreviousPayloadsForWalletAsync(walletAddress, allPrevTransactions, _walletServiceClient);
			var participantId = blueprint.Participants.First(participant => participant.WalletAddress == walletAddress).Id ??
				throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "Wallet address is not valid for the resolved blueprint.");

			List<string> requiredData = new();
			var disclosure = action?.Disclosures.ToList().Find(dis => dis.ParticipantAddress == participantId);
			if (disclosure != null)
				requiredData.AddRange(disclosure.DataPointers);
			requiredData.AddRange(action!.RequiredActionData);

			if (requiredData.Any())
				previousData = previousData.Where(data => requiredData.Contains(data.Key)).ToDictionary(x => x.Key, y => y.Value);
			action.PreviousData = JsonDocument.Parse(JsonSerializer.Serialize(previousData));
			return action;
		}

		public async Task<Action> GetAction(TransactionModel transaction, string walletAddress, string registerId, string transactionId, bool aggregatePreviousTransactionData)
		{
			var isBlueprintTransaction = transaction.MetaData!.TransactionType == TransactionTypes.Blueprint;
			var decryptedTransaction = await _walletServiceClient.DecryptTransaction(transaction, walletAddress);

			Blueprint blueprint;
			if (isBlueprintTransaction)
			{
				blueprint = JsonDocument.Parse(Encoding.UTF8.GetString(decryptedTransaction[0])).Deserialize<Blueprint>(_jsonSerializerOptions)!;
				var action = GetActionFromBlueprint(transaction, transactionId, blueprint);
				action.Blueprint = transactionId;
				return action;
			}
			else
			{
				blueprint = (await GetBlueprintJsonAsync(transaction.MetaData.BlueprintId, walletAddress, registerId)).Deserialize<Blueprint>(_jsonSerializerOptions)!;
				var action = GetActionFromBlueprint(transaction, transactionId, blueprint);
				action.Blueprint = transaction.MetaData.BlueprintId;

				if (!aggregatePreviousTransactionData)
				{
					var combinedKvpData = new Dictionary<string, object>();
					foreach (var bytes in decryptedTransaction)
					{
						var payloadKvp = JsonSerializer.Deserialize<Dictionary<string, object>>(bytes, new JsonSerializerOptions(JsonSerializerDefaults.Web));
						payloadKvp?.ToList().ForEach(dataKVP => combinedKvpData[dataKVP.Key] = dataKVP.Value);
					}
					action.PreviousData = JsonDocument.Parse(JsonSerializer.Serialize(combinedKvpData));
					return action;
				}

				var allPrevTransactions = await _registerServiceClient.GetTransactionsByInstanceId(registerId, transaction.MetaData.InstanceId);
				var previousData = await _payloadResolver.GetAllPreviousPayloadsForWalletAsync(walletAddress, allPrevTransactions, _walletServiceClient);
				var requiredList = action.RequiredActionData.ToList();
				if (requiredList.Any())
				{
					requiredList.Add("comment");
					previousData = previousData.Where(data => requiredList.Contains(data.Key)).ToDictionary(x => x.Key, y => y.Value);
				}
				action.PreviousData = JsonDocument.Parse(JsonSerializer.Serialize(previousData));
				return action;
			}
		}

		private static Action GetActionFromBlueprint(TransactionModel transaction, string transactionId, Blueprint blueprint)
		{
			var finalActionId = -1;
			if (transaction.MetaData!.NextActionId == finalActionId)
				return blueprint.Actions.FirstOrDefault(a => a.Id == transaction.MetaData.ActionId)!;
			var action = blueprint.Actions.FirstOrDefault(a => a.Id == transaction.MetaData.NextActionId);
			action!.PreviousTxId = transactionId;
			return action;
		}

		private async Task<JsonDocument> GetBlueprintJsonAsync(string txId, string walletAddress, string registerId)
		{
			//TODO we should use the BlueprintService to get the blueprint
			var blueprintTx = await _registerServiceClient.GetTransactionById(registerId: registerId, txId: txId);
			var blueprintBytes = await _walletServiceClient.DecryptTransaction(blueprintTx, walletAddress);
			var blueprintStr = Encoding.UTF8.GetString(blueprintBytes[0]);
			var blueprint = JsonDocument.Parse(blueprintStr);
			return blueprint;
		}
	}
}
