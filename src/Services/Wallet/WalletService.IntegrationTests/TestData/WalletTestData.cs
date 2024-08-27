using WalletService.IntegrationTests.Models;
using WalletService.SQLRepository;

namespace WalletService.IntegrationTests.TestData
{
    public class WalletTestData
    {
        public const string DefaultWalletName = "Test Wallet";
        public static MySqlRepository? MySqlRepository;
        public const string DefaultWalletMnemonic =
            "maid limit write faculty night beauty wash mushroom grace fashion immune swallow property similar sort payment crew notable tobacco disagree rate blind alter kit";
        
        public static CreateWallet NewDefault()
        {
            return new CreateWallet
            {
                Name = DefaultWalletName,
                Mnemonic = DefaultWalletMnemonic
            };
        }

        public static async Task Clear()
        {
            MySqlRepository ??= new MySqlRepository(CreateContext());
            await MySqlRepository.Clear();
        }

        public static async Task<CreateWalletResponse> CreateNewDefault(WalletOperations walletOperations)
        {
            var response = await walletOperations.Create(NewDefault());
            return (await response.Content.ReadFromJsonAsync<CreateWalletResponse>())!;
        }

        private static WalletContext CreateContext()
            => new WalletContextFactory().CreateDbContext(Array.Empty<string>());
    }
}
