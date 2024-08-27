using ActionService.Exceptions;
using ActionService.V1.Repositories;
using Json.Pointer;
using Siccar.Application;
using Siccar.Application.Constants;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
#nullable enable

namespace ActionService.V1.Services
{
	public class PayloadResolver : IPayloadResolver
	{
		private readonly IFileRepository _fileRepository;

		public PayloadResolver(IFileRepository fileRepository) { _fileRepository = fileRepository; }

		public ITxFormat AddParticipantPayloadsToTransaction(Blueprint blueprint, IEnumerable<Disclosure> disclosures, JsonDocument data, string senderAddress)
		{
			ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			IPayloadManager manager = transaction.GetTxPayloadManager();
			foreach (var disclosure in disclosures.ToList().Where(d => d.ParticipantAddress != DisclosureConstants.TrackingData))
			{
				(Status reault, UInt32 id, bool[]? wallets) = manager.AddPayload(BuildDataPayload(disclosure.ParticipantAddress, disclosures.ToList(), data));
				if (disclosure.ParticipantAddress != DisclosureConstants.PublicData)
					manager.AddPayloadWallets(id, new string[] { ResolveWallet(disclosure.ParticipantAddress, blueprint), ResolveWallet(senderAddress, blueprint) });
			}
			return transaction;
		}

		public async Task<ITxFormat> AddFilePayloadToTransaction(Blueprint blueprint, IEnumerable<Disclosure> disclosures, string fileName, string fileSchemaId)
		{
			byte[] fileBinary;
			using (Stream SourceStream = await _fileRepository.GetFile(fileName))
			{
				var memoryStream = new MemoryStream();
				SourceStream.CopyTo(memoryStream);
				fileBinary = memoryStream.ToArray();
			}
			List<string> payloadWallets = new();
			foreach (Disclosure disclosure in disclosures)
			{
				if (disclosure.DataPointers.Contains(fileSchemaId))
					payloadWallets.Add(ResolveWallet(disclosure.ParticipantAddress, blueprint));
			}
			ITxFormat transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			transaction.GetTxPayloadManager().AddPayload(fileBinary, payloadWallets.ToArray());
			return transaction;
		}

		public async Task<Dictionary<string, object>> GetAllPreviousPayloadsForWalletAsync(string walletAddress, List<TransactionModel> previousTransactions,
			IWalletServiceClient walletServiceClient)
		{
			var previousTransactionData = new List<byte[]>();
			foreach (var transaction in previousTransactions)
			{
				var bytes = await walletServiceClient.DecryptTransaction(transaction, walletAddress);
				if (bytes != null)
					previousTransactionData.AddRange(bytes);
			}
			var combinedKvpData = new Dictionary<string, object>();
			foreach (var bytes in previousTransactionData)
			{
				var payloadKvp = JsonSerializer.Deserialize<Dictionary<string, object>>(bytes, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				payloadKvp?.ToList().ForEach(dataKVP => combinedKvpData[dataKVP.Key] = dataKVP.Value);
			}
			return combinedKvpData;
		}

		private static string ResolveWallet(string participantId, Blueprint blueprint)
		{
			if (participantId.StartsWith("ws1"))
				return participantId;
			var participantAddress = blueprint.Participants.Find(p => p.Id == participantId)?.WalletAddress;
			participantAddress = participantAddress ?? throw new PayloadResolverException($"Participant with Id: {participantId}, could not be found in the blueprint.");
			return participantAddress;
		}

		private static byte[] BuildDataPayload(string participantId, List<Disclosure> disclosures, JsonDocument data)
		{
			var dataList = new Dictionary<string, object>();
			List<string>? dataPointers = disclosures.Find(d => d.ParticipantAddress == participantId)?.DataPointers;
			if (dataPointers != null)
			{
				foreach (var dataFieldStr in dataPointers)
				{
					string pointerStr = dataFieldStr;
					// some blueprints dont resolve properly as datapointers - must start / or  #
					if (!((dataFieldStr.StartsWith('/')) | (dataFieldStr.StartsWith('#'))))
						pointerStr = '/' + dataFieldStr;
					var dataField = JsonPointer.Parse(pointerStr);
					var _field = dataField.ToString().Trim('/', '#'); // if it does have the '/'|'#'
					var _value = dataField.Evaluate(data.RootElement);
					if (_value != null)
						dataList.Add(_field, _value);
				}
			}
			return JsonSerializer.SerializeToUtf8Bytes(dataList);
		}

		/// <summary>
		/// Builds a SortedList of TrackingData Elements
		/// </summary>
		/// <param name="disclosures">Definition of the Data</param>
		/// <param name="jsonData">The Data Elements</param>
		/// <returns></returns>
		public SortedList<string, string> ResolveTrackingData(IEnumerable<Disclosure> disclosures, JsonDocument jsonData)
		{
			var dataKvp = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData.RootElement.GetRawText());
			var dataList = new SortedList<string, string>();
			foreach (var disclosure in disclosures.ToList().Where(d => d.ParticipantAddress == DisclosureConstants.TrackingData))
			{
				foreach (var fieldIdStr in disclosure.DataPointers)
				{
					string pointerStr = fieldIdStr;
					// some blueprints dont resolve properly as datapointers - must start / or  #
					if (!((fieldIdStr.StartsWith('/')) | (fieldIdStr.StartsWith('#'))))
						pointerStr = '/' + fieldIdStr;
					var fieldId = JsonPointer.Parse(pointerStr);
					var field = fieldId.Evaluate(jsonData.RootElement);
					dataList.Add(fieldIdStr, field.ToString()!);
				}
			}
			return dataList;
		}
	}
}
