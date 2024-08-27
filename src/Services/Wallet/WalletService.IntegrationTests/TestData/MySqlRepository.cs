using Microsoft.EntityFrameworkCore;
using WalletService.SQLRepository;

namespace WalletService.IntegrationTests.TestData
{
    public class MySqlRepository
    {
        private readonly WalletContext _context;

        public MySqlRepository(WalletContext context)
        {
            _context = context;
        }

        public async Task Clear()
        {
            _context.Wallets?.RemoveRange(_context.Wallets);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCount(string walletName)
        {
            return await _context.Wallets?.CountAsync(w => w.Name == walletName)!;
        }
    }
}
