using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
#nullable enable

namespace ActionService.V1.Services
{
	public interface IPayloadResolver
	{
		public Task<Dictionary<string, object>> GetAllPreviousPayloadsForWalletAsync(string walletAddress, List<TransactionModel> previousTransactions,
			IWalletServiceClient walletServiceClient);
		public ITxFormat AddParticipantPayloadsToTransaction(Blueprint blueprint, IEnumerable<Disclosure> disclosures, JsonDocument data, string senderAddress);
		public SortedList<string, string> ResolveTrackingData(IEnumerable<Disclosure> disclosures, JsonDocument data);
		public Task<ITxFormat> AddFilePayloadToTransaction(Blueprint blueprint, IEnumerable<Disclosure> disclosures, string fileName, string fileSchemaId);
	}
}
