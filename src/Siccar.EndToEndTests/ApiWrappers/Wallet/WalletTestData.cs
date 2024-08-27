using Siccar.EndToEndTests.Wallet.Models;

namespace Siccar.EndToEndTests.Wallet
{
    public class WalletTestData
    {
        public const string DefaultWalletName = "Test Wallet";
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
    }
}
