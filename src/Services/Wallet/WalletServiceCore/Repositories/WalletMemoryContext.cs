using WalletService.Core.Interfaces;
using Siccar.Platform;

namespace WalletServiceCore.Repositories
{
    public class WalletMemoryContext : IWalletRepository
    {
        public WalletMemoryContext()
        {

        }

        public Task AddDelegates(string pubKey, IEnumerable<WalletAccess> accesses, string userSub)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDelegate(string pubKey, WalletAccess access, string userSub)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDelegate(string pubKey, string subject, string userSub)
        {
            throw new NotImplementedException();
        }

        public Task AddWalletTransaction(string pubKey, WalletTransaction TransactionId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteWallet(string pubKey, string userSub)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Wallet>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Wallet>> GetAll(string userSubject)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Wallet>> GetAllInTenant(string userSubject)
        {
            throw new NotImplementedException();
        }

        public Task<Wallet> GetWallet(string pubKey, string userSubject)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WalletTransaction>> GetWalletTransactions(string address, string userSubject)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveWallet(Wallet wallet, string userSubject)
        {
            throw new NotImplementedException();
        }

        public Task<Wallet> UpdateWallet(string address, string userSubject, UpdateWalletRequest wallet)
        {
            throw new NotImplementedException();
        }

        public Task<int> WalletUpdateTransactions(string pubKey, string TransactionId, string prevTxId)
        {
            throw new NotImplementedException();
        }
    }
}
