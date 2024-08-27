using Siccar.EndToEndTests.Action;
using Siccar.EndToEndTests.Blueprint;
using Siccar.EndToEndTests.Common;
using Siccar.EndToEndTests.Register;
using Siccar.EndToEndTests.Wallet;
using WalletService.SQLRepository;

namespace Siccar.EndToEndTests
{
    internal class SiccarServices
    {
        public static readonly MySqlRepository MySqlRepository = new(CreateWalletContext());
        public static readonly MongoDbRepository MongoDbRepository = new();
        public static WalletOperations Wallets => new();
        public static BlueprintOperations Blueprints => new();
        public static RegisterOperations Registers => new();
        public static ActionOperations Actions => new();

        public static async Task Clear()
        {
            var tasks = new List<Task>
            {
                MySqlRepository.Clear(),
                MongoDbRepository.Clear()
            };
            await Task.WhenAll(tasks);
        }

        private static WalletContext CreateWalletContext()
            => new WalletContextFactory().CreateDbContext(Array.Empty<string>());
    }
}
