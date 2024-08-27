using Siccar.Platform;

namespace WalletService.Core.Interfaces
{
    public interface IWalletRepository
    {
        /// <summary>
        /// Gets all wallets
        /// </summary>
        public Task<IEnumerable<Wallet>> GetAll();

        /// <summary>
        /// Gets wallets by Owner
        /// </summary>
        public Task<IEnumerable<Wallet>> GetAll(string userSubject);

        /// <summary>
        /// Gets wallets by Tenant Id
        /// </summary>
        public Task<IEnumerable<Wallet>> GetAllInTenant(string tenantId);

        /// <summary>
        /// Get Wallet by Id (Master Wallet Address)
        /// </summary>
        public Task<Wallet> GetWallet(string pubKey, string userSubject);

        /// <summary>
        /// Saves a New wallet
        /// </summary>
        public Task<bool> SaveWallet(Wallet wallet, string userSubject);

        /// <summary>
        /// Update properties of the wallet
        /// </summary>
        public Task<Wallet> UpdateWallet(string address, string userSubject, UpdateWalletRequest wallet);
        
        /// <summary>
        /// Removes a given wallet from storage
        /// </summary>
        public Task DeleteWallet(string pubKey, string userSub);

        /// <summary>
        /// Updates details on a Wallet Transaction
        /// </summary>
        public Task<int> WalletUpdateTransactions(string pubKey, string transactionId, string prevTxId);

        /// <summary>
        /// Adds a wallet transaction
        /// </summary>
        public Task AddWalletTransaction(string pubKey, WalletTransaction TransactionId);

        /// <summary>
        /// Adds multiple delgates to a wallet.
        /// </summary>
        public Task AddDelegates(string pubKey, IEnumerable<WalletAccess> accesses, string userSub);

        /// <summary>
        /// Updates a wallet delegate
        /// </summary>
        public Task UpdateDelegate(string pubKey, WalletAccess access, string userSub);

        /// <summary>
        /// Removes a wallet delegate
        /// </summary>
        public Task RemoveDelegate(string address, string subject, string userSub);
    }
}
