#nullable enable

using System.Text.Json.Serialization;

namespace Siccar.Platform
{
    public sealed class CreateWalletResponse : Wallet
    {
        [JsonConstructor]
        public CreateWalletResponse()
        {
            
        }

        public CreateWalletResponse(Wallet? wallet, string? mnemonic)
        {
            Name = wallet?.Name;
            Address = wallet?.Address;
            Transactions = wallet?.Transactions;
            Owner = wallet?.Owner;
            Addresses = wallet?.Addresses;
            Delegates = wallet?.Delegates ?? new List<WalletAccess>();
            Mnemonic = mnemonic;
            PrivateKey = wallet?.PrivateKey;
            Tenant = wallet?.Tenant;
            Mnemonic = mnemonic;
        }

        public string? Mnemonic { get; init; }
    }
}
