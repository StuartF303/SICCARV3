using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletService.Core;
using Siccar.Common.Adaptors;
using WalletService.Core.Interfaces;
using Siccar.Platform;
using System.Net;
#nullable enable

namespace WalletService.SQLRepository
{
	public class WalletRepository : IWalletRepository
	{
		private readonly WalletContext _walletContext;
		private readonly ILogger<WalletRepository> _logger;
		private readonly IWalletProtector _walletProtector;
		private readonly IDaprClientAdaptor? _client;

		/// <summary>
		/// Constructs the wallet repository setting the encryption keywo
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="WalletProtector"></param>
		/// <param name="dbContext"></param>
		public WalletRepository(ILogger<WalletRepository> logger, WalletContext dbContext, IWalletProtector protector)
		{
			_walletContext = dbContext;
			_walletProtector = protector;
			_logger = logger;
		}

		/// <summary>
		/// Constructs the wallet repository setting the encryption purpose from dapr secrets
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="WalletProtector"></param>
		/// <param name="dbContext"></param>
		public WalletRepository(ILogger<WalletRepository> logger, IWalletProtector WalletProtector, WalletContext dbContext, IDaprClientAdaptor daprClientAdaptor)
		{
			_walletContext = dbContext;
			_walletProtector = WalletProtector;
			_logger = logger;
			_client = daprClientAdaptor;
		}

		/// <summary>
		/// Deletes a Wallet from the Store
		/// </summary>
		/// <param name="pubKey">Wallet Address to Delete</param>
		/// <returns></returns>
		public async Task DeleteWallet(string pubKey, string userSub)
		{
			var wallet = (_walletContext?.Wallets?
				.Include(w => w.Delegates)
				.FirstOrDefault(w => w.Address == pubKey)) ?? throw new WalletException(HttpStatusCode.NotFound, "Not found.");
			var accessType = CheckUserIsAuthorized(userSub, wallet);

			if (accessType == AccessTypes.owner || accessType == AccessTypes.delegaterw)
			{
				_walletContext?.Remove(wallet);
				await _walletContext!.SaveChangesAsync();
				return;
			}
			throw new WalletException(HttpStatusCode.Forbidden, "User does not have required access level.");
		}

		/// <summary>
		/// Get ALL Wallets - Intensive and should be rarely used.
		/// THIS METHOD DOES NOT DECRYPT THE PRIVATE KEYS
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Wallet>> GetAll()
		{
			return await _walletContext.Wallets!.ToListAsync(); //AsEnumerable<Wallet>();
		}

		/// <summary>
		/// Get all Wallets for the given tenant id.
		/// THIS METHOD DOES NOT DECRYPT THE PRIVATE KEYS
		/// </summary>
		/// <param name="tenantId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Wallet>> GetAllInTenant(string tenantId)
		{
			var wallets = _walletContext.Wallets!
				.Include(w => w.Delegates)
				.Where(w => w.Tenant == tenantId);

			return await wallets.ToListAsync(); //.AsEnumerable<Wallet>();
		}

		/// <summary>
		/// Get all Wallets for a Given Owner
		/// THIS METHOD DOES NOT DECRYPT THE PRIVATE KEYS
		/// </summary>
		/// <param name="userSub"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Wallet>> GetAll(string userSub)
		{
			var wallets = _walletContext.Wallets!
				.Include(w => w.Delegates)
				.Where(w => w.Owner == userSub || w.Delegates.Any(deleg => deleg.Subject == userSub));
			// was corrected to return all records, not last one
			return await wallets.ToListAsync(); //.AsEnumerable<Wallet>();
		}

		/// <summary>
		/// Get a particular Wallet by Address
		/// IF AUTHORISED THIS METHOD _DOES_ DECRYPT THE PRIVATE KEYS
		/// </summary>
		/// <param name="pubKey"></param>
		/// <returns></returns>
		public async Task<Wallet> GetWallet(string pubKey, string userSub)
		{
			var encWallet = _walletContext.Wallets!
				.Include(w => w.Delegates)
				.Include(w => w.Transactions)!
				.ThenInclude(w => w.MetaData)
				.Include(w => w.Addresses)
				.FirstOrDefault(w => w.Address == pubKey) ?? throw new WalletException(HttpStatusCode.NotFound, "Not found.");

			// are you allowed
			CheckUserIsAuthorized(userSub, encWallet);
			return await Task.Run(() => _walletProtector.UnProtectWallet(encWallet));
		}

		/// <summary>
		/// Saves a Wallet Record, ensuring ecryption of sensitive fields
		/// </summary>
		/// <param name="wallet"></param>
		/// <param name="userSub"></param>
		/// <returns></returns>
		/// <exception cref="WalletException"></exception>
		public async Task<bool> SaveWallet(Wallet wallet, string userSub)
		{
			// we are going to do some simple updates at this point
			var orginalWallet = _walletContext?.Wallets?.FirstOrDefault(w => w.Address == wallet.Address);

			if (orginalWallet != null)
				throw new WalletException(HttpStatusCode.BadRequest, $"Wallet with key: {orginalWallet.Address} already exists.");

			var savedWallet = _walletProtector.ProtectWallet(wallet);
			_walletContext?.Wallets?.Add(savedWallet);
			await _walletContext!.SaveChangesAsync();
			return true;
		}

		/// <summary>
		/// Stores a Wallet Address Record
		/// </summary>
		/// <param name="walletAddress"></param>
		/// <returns></returns>
		public async Task<string> WalletAddAddress(WalletAddress walletAddress, string userSub)
		{
			//TODO: Needs tested but is not yet implemented
			var envWallAddr = new WalletAddress()
			{
				Address = walletAddress.Address,
				DerivationPath = _walletProtector.ProtectWallet(walletAddress.DerivationPath),
				WalletId = walletAddress.WalletId
			};

			var keyWallet = (_walletContext?.Wallets?.FirstOrDefault(w => w.Address == walletAddress.Address)) ?? throw new WalletException(HttpStatusCode.NotFound, "Users wallet not found.");
			var accessType = CheckUserIsAuthorized(userSub, keyWallet);

			if (accessType == AccessTypes.owner || accessType == AccessTypes.delegaterw)
			{
				keyWallet.Addresses?.Append<WalletAddress>(envWallAddr);
				await _walletContext!.Wallets!.AddAsync(keyWallet);
				return envWallAddr.Address;
			}
			throw new WalletException(HttpStatusCode.Forbidden, "User does not have required access level.");
		}

		public Task<Wallet> UpdateWallet(string address, string userSub, UpdateWalletRequest wallet)
		{
			throw new NotImplementedException();
		}
		public async Task<int> WalletUpdateTransactions(string pubKey, string transactionId, string previousTxId)
		{
			int updates = 0;
			var expectedId = $"{transactionId}:{pubKey}";

			// firstly does wallet/address exist - we 
			var usrWallet = _walletContext.Wallets!.FirstOrDefault(w => w.Address == pubKey) ?? throw new WalletException(HttpStatusCode.NotFound, "No Wallet Found.");
			var walletTransactions = from trans in _walletContext.Transactions where trans.Id == expectedId select trans;
			if (walletTransactions.Any())
			{
				if (walletTransactions?.Count() > 1)
				{
					_logger.LogWarning("Multiple Records exist for TxID: {txid}", transactionId);
					throw new WalletException(HttpStatusCode.Conflict, $"Wallet Transaction: {expectedId} already found.");
				}
				var walletTransaction = walletTransactions!.First();
				// Update the existing transaction
				walletTransaction.isConfirmed = true;
				var previousTransaction = _walletContext.Transactions!.FirstOrDefault(t => t.TransactionId == previousTxId);
				// lookup previous transaction and change to spent. Only happens when sender is also the recipient
				if (previousTransaction != null)
					previousTransaction.isSpent = true;

				try
				{
					updates = await _walletContext!.SaveChangesAsync();
					_logger.LogInformation("Update: {pkey} TxID: {txid} PrevTxId : {ptxid}", pubKey, transactionId, previousTxId);
					
				}
				catch (DbUpdateConcurrencyException er)
				{
					_logger.LogWarning("FAILED Update: {wid} TxID: {txid} PrevTxId : {ptxid} Error : {msg}",
						walletTransaction.Id, transactionId, previousTxId, er.Message);
					//DbUpdateConcurrencyException: The database operation was expected to affect 1 row(s), but actually affected 0 row(s);
				}
				catch(Exception ex)
				{
					_logger.LogError("FAILED Update: {wid} TxID: {txid} PrevTxId : {ptxid} Error : {msg}",
						walletTransaction.Id, transactionId, previousTxId, ex.Message);

					throw new WalletException(HttpStatusCode.BadRequest, $"Error: {ex.Message}.");
				}
			}
			else
			{
				_logger.LogWarning($"FAILED No Transaction To Update");
				throw new WalletException(HttpStatusCode.NotFound, $"Wallet Transaction: {expectedId} not found.");
			}
			return updates;
		}

		public async Task AddWalletTransaction(string pubKey, WalletTransaction transactionUpdate)
		{
			var expectedId = $"{transactionUpdate.TransactionId}:{pubKey}";
			var walletTransaction = _walletContext?.Transactions?.FirstOrDefault(t => t.Id == expectedId);
			if (walletTransaction == null)
			{
				_walletContext?.Transactions?.Add(transactionUpdate);
				await _walletContext!.SaveChangesAsync();
			}
		}

		public async Task AddDelegates(string pubKey, IEnumerable<WalletAccess> accesses, string userSub)
		{
			var wallet = (_walletContext?.Wallets?.Include(w => w.Delegates).FirstOrDefault(w => w.Address == pubKey)) ?? throw new WalletException(HttpStatusCode.NotFound, $"Wallet address with address: {pubKey} not found");
			CheckUserHaveWriteAccess(userSub, wallet);

			foreach (var access in accesses)
			{
				if (wallet.Delegates.Any(d => d.Subject == access.Subject))
					throw new WalletException(HttpStatusCode.BadRequest, $"A delegate already exists for user subject: {access.Subject}");
			}

			foreach (var access in accesses)
			{
				wallet.Delegates.Add(access);
			}

			await _walletContext!.SaveChangesAsync();
		}

		public async Task RemoveDelegate(string pubKey, string subject, string userSub)
		{
			var wallet = (_walletContext?.Wallets?.Include(w => w.Delegates).FirstOrDefault(w => w.Address == pubKey)) ?? throw new WalletException(HttpStatusCode.NotFound, $"Wallet address with address: {pubKey} not found");
			CheckUserHaveWriteAccess(userSub, wallet);

			var dlgs = wallet.Delegates.FirstOrDefault(d => d.Subject == subject) ?? throw new WalletException(HttpStatusCode.BadRequest, $"A delegate doesn't exists for user subject: {subject}");
			_walletContext?.Delegates?.Remove(dlgs);
			await _walletContext!.SaveChangesAsync();
		}

		public async Task UpdateDelegate(string pubKey, WalletAccess access, string userSub)
		{
			var wallet = (_walletContext?.Wallets?
				.Where(w => w.Address == pubKey)
				.Include(w => w.Delegates)
				.SingleOrDefault()) ?? throw new WalletException(HttpStatusCode.NotFound, $"Wallet address with address: {pubKey} not found");
			var accessType = CheckUserHaveWriteAccess(userSub, wallet);
			var dlgs = wallet.Delegates.FirstOrDefault(d => d.Subject == access.Subject) ?? throw new WalletException(HttpStatusCode.BadRequest, $"A delegate doesn't exists for user subject: {access.Subject}");
			var upd = (_walletContext?.Delegates?.Where(d => d.Id == dlgs.Id).SingleOrDefault()) ?? throw new WalletException(HttpStatusCode.NotFound, $"Wallet Delegate (fatal): {access.Subject} not found in the database");

			// upd.Tenant = access.Tenant;
			// upd.AssignedTime = access.AssignedTime;
			upd.Reason = access.Reason;
			upd.AccessType = access.AccessType;
			await _walletContext!.SaveChangesAsync();
		}

		private static AccessTypes CheckUserIsAuthorized(string userSub, Wallet encWallet)
		{
			if (encWallet.Owner == userSub)
				return AccessTypes.owner;
			var walletDelegate = encWallet.Delegates.FirstOrDefault(wa => wa.Subject == userSub);

			if (walletDelegate != null)
				return walletDelegate.AccessType;
			throw new WalletException(HttpStatusCode.Forbidden, "Unauthorized Wallet access.");
		}

		private static AccessTypes CheckUserHaveWriteAccess(string userSub, Wallet encWallet)
		{
			var _rc = CheckUserIsAuthorized(userSub, encWallet);
			if ((_rc != AccessTypes.owner) && (_rc != AccessTypes.delegaterw))
				throw new WalletException(HttpStatusCode.Forbidden, $"User {userSub} should have write access to modify Wallet Delegates.");
			return _rc;
		}
	}
}
