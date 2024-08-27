using Microsoft.AspNetCore.DataProtection;
using Siccar.Platform;
using WalletService.Core.Interfaces;
#nullable enable

namespace WalletService.Core.Repositories
{
	public class WalletProtector : IWalletProtector
	{
		private readonly IDataProtectionProvider _protectorProvider;
		private readonly IDataProtector? _protectorInstance;
		private readonly string _purposeString = "WalletService.SQLRepository.dataprotector";

		public WalletProtector(IDataProtectionProvider dataProtector)
		{
			_protectorProvider = dataProtector;
			_protectorInstance = _protectorProvider.CreateProtector(_purposeString);
		}

		public string MasterEncryptionKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public string? ProtectWallet(string walletToProtect)
		{
			var protectedWallet = _protectorInstance?.Protect(walletToProtect);
			return protectedWallet;
		}

		public Wallet ProtectWallet(Wallet unprotectedWallet)
		{
			
			return new Wallet()
			{
				Address = unprotectedWallet.Address,
				PrivateKey = _protectorInstance?.Protect(unprotectedWallet.PrivateKey!),
				Addresses = unprotectedWallet.Addresses,
				Name = unprotectedWallet.Name,
				Owner = unprotectedWallet.Owner,
				Tenant = unprotectedWallet.Tenant,
				Transactions = unprotectedWallet.Transactions,
				Delegates = unprotectedWallet.Delegates
			};
		}

		public string? UnprotectWallet(string walletToUnprotect)
		{
			var walletString = _protectorInstance?.Unprotect(walletToUnprotect);
			return walletString;
		}

		public Wallet UnProtectWallet(Wallet protectedWallet)
		{
			return new Wallet()
			{
				Address = protectedWallet.Address,
				PrivateKey = _protectorInstance?.Unprotect(protectedWallet.PrivateKey!),
				Addresses = protectedWallet.Addresses,
				Name = protectedWallet.Name,
				Owner = protectedWallet.Owner,
				Tenant = protectedWallet.Tenant,
				Transactions = protectedWallet.Transactions,
				Delegates = protectedWallet.Delegates
			};
		}

		public WalletAddress ProtectWalletAddress(WalletAddress unprotectedAddress)
		{
			throw new NotImplementedException();
		}

		public WalletAddress UnProtectWalletAddress(WalletAddress protectedAddress)
		{
			throw new NotImplementedException();
		}
	}
}
