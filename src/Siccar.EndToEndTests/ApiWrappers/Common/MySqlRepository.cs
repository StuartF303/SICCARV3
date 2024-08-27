using WalletService.SQLRepository;

namespace Siccar.EndToEndTests.Common
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
            _context.Addresses?.RemoveRange(_context.Addresses);
            _context.Delegates?.RemoveRange(_context.Delegates);
            _context.Wallets?.RemoveRange(_context.Wallets);
            _context.TransactionMetaData?.RemoveRange(_context.TransactionMetaData);
            _context.Transactions?.RemoveRange(_context.Transactions);
            await _context.SaveChangesAsync();
        }
    }
}
