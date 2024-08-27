using Siccar.Platform;
#nullable enable

namespace WalletService.Core.Interfaces
{
	public interface IWalletProtector
	{
		public string MasterEncryptionKey { get; set; } // used for testing - cannot be got when using HSM

		public string? ProtectWallet(string walletToProtect);

		public string? UnprotectWallet(string walletToUnprotect);

		public Wallet ProtectWallet(Wallet unprotectedWallet);

		public Wallet UnProtectWallet(Wallet protectedWallet);

		public WalletAddress ProtectWalletAddress(WalletAddress unprotectedAddress);

		public WalletAddress UnProtectWalletAddress(WalletAddress protectedAddress);
	}
}
