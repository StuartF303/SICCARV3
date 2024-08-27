using System.Net.Http.Json;
using Siccar.EndToEndTests.Common;
using Siccar.EndToEndTests.Wallet.Models;

namespace Siccar.EndToEndTests.Wallet
{
    public class WalletOperations
    {
        private readonly string _walletUri = "api/wallets/{0}";

        public async Task<HttpResponseMessage> Create(CreateWallet createWallet)
        {
            var httpClient = await HttpClientFactory.Create();
            return await httpClient.PostAsJsonAsync(string.Format(_walletUri, ""), createWallet);
        }
    }
}