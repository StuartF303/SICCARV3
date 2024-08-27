using Siccar.Platform;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public interface IWalletServiceClient : ISiccarServiceClient
	{
		Task<List<Wallet>?> ListWallets(bool allInTenant = false);
		Task<Wallet?> GetWallet(string walletAddress);
		public Task<CreateWalletResponse> CreateWallet(string Name, string Description = "", string Mnemonic = "");
		public Task<TransactionModel> SignAndSendTransaction(Transaction? request, string walletAddress);
		public Task<byte[][]> DecryptTransaction(TransactionModel transaction, string walletAddress);
		public Task<byte[][]> DecryptTransaction(TransactionModel transaction, string walletAddress, string? accessToken = null);
		public Task<byte[][]> GetAccessiblePayloads(TransactionModel? transaction);
		public Task<List<PendingTransaction>> GetWalletTransactions(string walletAddress);
		public Task<List<WalletTransaction>> GetAllTransactions(string walletAddress);
		public Task AddDelegate(string walletAddress, WalletAccess @delegate);
		public Task AddDelegates(string walletAddress, IEnumerable<WalletAccess> delegates);
		public Task DeleteDelegate(string walletAddress, string subject);
		public Task UpdateDelegate(string walletAddress, WalletAccess @delegate);
		/// <summary>
		/// Deletes a wallet witha given address, if appropriate access.
		/// </summary>
		/// <param name="walletAddress"></param>
		/// <returns></returns>
		public Task<HttpResponseMessage> DeleteWallet(string walletAddress);
	}
}
