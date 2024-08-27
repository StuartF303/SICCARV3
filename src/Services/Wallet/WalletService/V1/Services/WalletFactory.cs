using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Cryptography;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Siccar.Platform.Cryptography.KeyManager;
#nullable enable

namespace WalletService.V1.Services
{
	public class WalletFactory : IWalletFactory
	{
		private readonly IRegisterServiceClient _registerServiceClient;
		private readonly KeyManager manager = new();

		public WalletFactory(IRegisterServiceClient registerServiceClient) { _registerServiceClient = registerServiceClient; }

		public async Task<(Wallet wallet, string? mnemonic)> BuildWallet(string walletName, string mnemonic, string registerId, string tenant, string subject, bool isCreate)
		{
			(Status status, KeyRing? keyset) set;
			if (mnemonic != null && mnemonic != string.Empty)
			{
				set = manager.RecoverMasterKeyRing(mnemonic);
				if (set.status == Status.KM_BAD_MNEMONIC)
					throw new HttpStatusException(HttpStatusCode.BadRequest, "Mnemonic in request is not valid");
			}
			else
				set = manager.CreateMasterKeyRing(WalletNetworks.ED25519);

			var wa = new WalletAddress() { WalletId = set.keyset?.Wallet(), Address = set.keyset?.Wallet(), DerivationPath = "m/" };
			var dele = new WalletAccess() { WalletId = set.keyset?.Wallet(), AccessType = AccessTypes.owner, Subject = subject, Tenant = tenant, Reason = "Wallet Creation" };

			List<WalletTransaction> txns;
			txns = isCreate ? new List<WalletTransaction>() : await GetWalletTransactions(registerId, set.keyset!.Wallet());
			var wallet = new Wallet
			{
				PrivateKey = set.keyset?.WIFKey(),
				Owner = subject,
				Tenant = tenant,
				Address = set.keyset?.Wallet(),
				Name = walletName,
				Delegates = new List<WalletAccess> { dele },
				Addresses = new List<WalletAddress> { wa },
				Transactions = txns
			};
			return (wallet, mnemonic: set.keyset?.Mnemonic());
		}

		private async Task<List<WalletTransaction>> GetWalletTransactions(string registerId, string publicKey)
		{
			List<WalletTransaction> walletTransactions = new();
			var receivedTransactions = await _registerServiceClient.GetAllTransactionsByRecipientAddress(registerId, publicKey);
			var sentTransactions = await _registerServiceClient.GetAllTransactionsBySenderAddress(registerId, publicKey);
			var spentTransactions = new List<string>();

			foreach (TransactionModel sentTx in sentTransactions)
			{
				walletTransactions.Add(new WalletTransaction
				{
					TransactionId = sentTx.Id,
					Sender = sentTx.SenderWallet,
					MetaData = sentTx.MetaData,
					PreviousId = sentTx.PrevTxId,
					ReceivedAddress = publicKey,
					Timestamp = sentTx.TimeStamp,
					isConfirmed = true,
					isSpent = true,
					WalletId = publicKey
				});

				for (int i = 0; i < receivedTransactions.Count; i++)
				{
					if (receivedTransactions[i].TxId == sentTx.PrevTxId)
					{
						spentTransactions.Add(receivedTransactions[i].TxId);
					}
				}
			}
			foreach (TransactionModel recievedTx in receivedTransactions)
			{
				WalletTransaction? tx = walletTransactions.Find(transaction => transaction.TransactionId == recievedTx.Id);
				if (tx is null)
				{
					walletTransactions.Add(new WalletTransaction
					{
						TransactionId = recievedTx.Id,
						Sender = recievedTx.SenderWallet,
						MetaData = recievedTx.MetaData,
						PreviousId = recievedTx.PrevTxId,
						ReceivedAddress = publicKey,
						Timestamp = recievedTx.TimeStamp,
						isConfirmed = true,
						isSpent = spentTransactions.Contains(recievedTx.Id),
						WalletId = publicKey
					});
				}
				else
					tx.isSpent = spentTransactions.Contains(recievedTx.Id);
			}
			return walletTransactions;
		}
	}
}
