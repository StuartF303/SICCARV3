using Microsoft.AspNetCore.Mvc;
using Siccar.Platform;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WalletService.V1.Services
{
	public interface IWalletFactory
	{
		public Task<(Wallet wallet, string mnemonic)> BuildWallet(string walletName, string mnemonic, string regsiterId, string tenant, string subject, bool isCreate);
	}
}
