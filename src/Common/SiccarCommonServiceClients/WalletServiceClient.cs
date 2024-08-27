using System.Net;
using System.Text.Json;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public class WalletServiceClient : IWalletServiceClient
	{
		private readonly SiccarBaseClient _baseClient;
		private readonly string _walletEndpoint = "Wallets";
		private readonly string _pendingTxEndpoint = "PendingTransactions";

		public WalletServiceClient(SiccarBaseClient baseClient) { _baseClient = baseClient; }

		public static async Task<IEnumerable<WalletAddress>> GetWalletAddressesAsync() { return await Task.Run(() => new List<WalletAddress>()); }

		public async Task<CreateWalletResponse> CreateWallet(string Name, string Description = "", string Mnemonic = "")
		{
			var URI = $"{_walletEndpoint}";
			string j1 = $"\"name\":\"{Name}\"";
			if (!string.IsNullOrWhiteSpace(Description))
				j1 += $",\"description\":\"{Description}\"";
			if (!string.IsNullOrWhiteSpace(Mnemonic))
				j1 += $",\"mnemonic\":\"{Mnemonic}\"";

			string jstring = "{" + j1 + "}";
			JsonDocument walletJson = JsonDocument.Parse(jstring);

			var response = await _baseClient.PostJsonAsync(URI, walletJson);

			if (response != null)
			{
				var res = JsonSerializer.Deserialize<CreateWalletResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				if (res != null)
					return res;
			}
			return new CreateWalletResponse(null, null);
		}

		public async Task<Wallet?> GetWallet(string walletAddress)
		{
			var URI = $"{_walletEndpoint}";
			try
			{
				var response = await _baseClient.GetJsonAsync($"{URI}/{walletAddress}");
				var result = response?.Deserialize<Wallet>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				return result;
			}
			catch (HttpStatusException e) when (e.Status == HttpStatusCode.NotFound) {}
			return null;
		}

		public async Task<List<Wallet>?> ListWallets(bool allInTenant = false)
		{
			var URI = $"{_walletEndpoint}?allInTenant={allInTenant}";
			try
			{
				var response = await _baseClient.GetJsonAsync($"{URI}");
				return response.Deserialize<List<Wallet>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
			}
			catch (HttpStatusException e) when (e.Status == HttpStatusCode.NotFound) {}
			return null;
		}

		/* For use in the future */
		public async Task<TransactionModel> SignAndSendTransaction(Transaction? request, string walletAddress)
		{
			var URI = $"{_walletEndpoint}/{walletAddress}/transactions";
			var response = await _baseClient.PostJsonAsync(URI, JsonSerializer.Serialize(request, _baseClient.serializerOptions));
			return JsonSerializer.Deserialize<TransactionModel>(response, _baseClient.serializerOptions)!;
		}

		public async Task<byte[][]> DecryptTransaction(TransactionModel? transaction, string walletAddress)
		{
			var URI = $"{_walletEndpoint}/{walletAddress}/transactions/decrypt";
			var response = await _baseClient.PostJsonAsync(URI, JsonSerializer.Serialize(transaction, _baseClient.serializerOptions));
			return (!string.IsNullOrWhiteSpace(response)) ?
				JsonSerializer.Deserialize<byte[][]>(response, _baseClient.serializerOptions)! : Array.Empty<byte[]>();
		}

		public async Task<byte[][]> GetAccessiblePayloads(TransactionModel? transaction)
		{
			var uri = $"{_walletEndpoint}/transactions/decrypt";
			var response = await _baseClient.PostJsonAsync(uri, JsonSerializer.Serialize(transaction, _baseClient.serializerOptions));
			return !string.IsNullOrWhiteSpace(response) ? JsonSerializer.Deserialize<byte[][]>(response, _baseClient.serializerOptions)! : Array.Empty<byte[]>();
		}

		public async Task<byte[][]> DecryptTransaction(TransactionModel? transaction, string walletAddress, string? accessToken = null)
		{
			var URI = $"{_walletEndpoint}/{walletAddress}/transactions/decrypt";
			var payload = JsonSerializer.Serialize(transaction, _baseClient.serializerOptions);
			var response = await _baseClient.PostJsonAsync(URI, payload);
			return JsonSerializer.Deserialize<byte[][]>(response, _baseClient.serializerOptions)!;
		}

		public async Task<List<PendingTransaction>> GetWalletTransactions(string walletAddress)
		{
			var URI = $"{_pendingTxEndpoint}/{walletAddress}";
			var response = await _baseClient.GetJsonAsync(URI);
			if (response != null)
				return JsonSerializer.Deserialize<List<PendingTransaction>>(response, _baseClient.serializerOptions)!;
			throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, $"Request to WalletService was not successful.");
		}

		public async Task<List<WalletTransaction>> GetAllTransactions(string walletAddress)
		{
			var uri = $"{_walletEndpoint}/{walletAddress}/transactions";
			var response = await _baseClient.GetJsonAsync(uri);
			if (response != null)
				return JsonSerializer.Deserialize<List<WalletTransaction>>(response, _baseClient.serializerOptions)!;
			throw new HttpStatusException(HttpStatusCode.NotFound, "Request to WalletService was not successful.");
		}

		public async Task AddDelegate(string walletAddress, WalletAccess @delegate) { await AddDelegates(walletAddress, new List<WalletAccess> { @delegate }); }

		public async Task AddDelegates(string walletAddress, IEnumerable<WalletAccess> @delegate)
		{
			var uri = $"{_walletEndpoint}/{walletAddress}/delegates";
			await _baseClient.PostJsonAsync(uri, JsonSerializer.Serialize(@delegate, _baseClient.serializerOptions));
		}

		public async Task DeleteDelegate(string walletAddress, string subject)
		{
			var uri = $"{_walletEndpoint}/{walletAddress}/{subject}/delegates";
			await _baseClient.Delete(uri);
		}

		public async Task UpdateDelegate(string walletAddress, WalletAccess @delegate)
		{
			var uri = $"{_walletEndpoint}/{walletAddress}/delegates";
			await _baseClient.PatchJsonAsync(uri, JsonSerializer.Serialize(@delegate, _baseClient.serializerOptions));
		}

		public Task<HttpResponseMessage> DeleteWallet(string walletAddress)
		{
			var URI = $"{_walletEndpoint}/{walletAddress}";
			return _baseClient.Delete(URI);
		}
	}
}
