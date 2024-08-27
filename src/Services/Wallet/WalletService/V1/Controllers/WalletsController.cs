using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Platform.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siccar.Platform;
using WalletService.Exceptions;
using System.Net;
using Siccar.Common;
using Siccar.Common.Adaptors;
using WalletService.Core;
using Siccar.Common.ServiceClients;
using WalletService.Core.Interfaces;
using WalletService.V1.Services;
using System.Diagnostics;
using Asp.Versioning;
#nullable enable

namespace WalletService.V1.Controllers
{
	/// <summary>
	/// Wallet Controller for managing keys and wallets
	/// </summary>
	/// <remarks>
	/// Controller for managing keys and wallets
	/// </remarks>
	/// <param name="client">The Dapr Local Client</param>
	/// <param name="logger">Our system Logger</param>
	/// <param name="walletRepository">The Wallet Repository</param>
	/// <param name="walletFactory">The factory for wallet functions</param>
	/// <param name="registerServiceClient">The client that communicates with the register</param>
	[Authorize]
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api")] 
	[Produces("application/json")]
	public class WalletsController(IDaprClientAdaptor client, ILogger<WalletsController> logger, IWalletRepository walletRepository,
		IWalletFactory walletFactory, IRegisterServiceClient registerServiceClient) : ControllerBase //ODataController
	{
		private readonly IDaprClientAdaptor _daprClient = client;
		private readonly IWalletRepository _walletRepository = walletRepository;
		private readonly IRegisterServiceClient _registerServiceClient = registerServiceClient;
		private readonly IWalletFactory _walletFactory = walletFactory;
		private readonly ILogger<WalletsController> _logger = logger;

		/// <summary>
		/// Gets all wallets that the authenticated user owns.
		/// </summary>
		/// <returns>The the retrieved wallets</returns>
		[HttpGet("Wallets")]
		//[EnableQuery]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Wallet>>> Get([FromQuery] bool allInTenant)
		{
			if (!allInTenant)
			{
				if (!User.IsInRole(Constants.WalletUserRole))
					throw new HttpStatusException(HttpStatusCode.Forbidden, $"User is not authorised.");

				var wallets = await _walletRepository.GetAll(GetUserSub());
					return Ok(wallets);
			}
			else
			{
				var userHasRequiredRoll = User.IsInRole(Constants.InstallationAdminRole) || User.IsInRole(Constants.InstallationReaderRole) ||
					User.IsInRole(Constants.TenantAdminRole);
				if (!userHasRequiredRoll)
					throw new HttpStatusException(HttpStatusCode.Forbidden, $"User is not authorised to view all wallets in the tenant.");

				var wallets = await _walletRepository.GetAllInTenant(GetUserTenant());
				return Ok(wallets);
			}
		}

		/// <summary>
		/// Gets a wallet from storage.
		/// </summary>
		/// <param name="address">The public key of the desired wallet to retrieve from storage</param>
		/// <returns>The the retrieved wallet object</returns>
		//[EnableQuery]
		[HttpGet("Wallets/{address}", Name = "GetWalletsByAddress", Order = 2)]
		//[HttpGet("Wallets({address})", Name = "GetWalletsByAddressOData", Order = 3)]  // doesnt appear to be odata'ing
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<Wallet>> GetWallet([FromRoute] string address)
		{
			var wallet = await _walletRepository.GetWallet(address, GetUserSub());
			return Ok(wallet);
		}

		/// <summary>
		/// Creates a wallet and generates a master keyset
		/// </summary>
		/// <param name="createWalletRequest">Initial information needed to create a wallet.</param>
		/// <param name="registerId">Id of the register for which to recreate the wallet.</param>
		/// <returns>The created wallet object</returns>
		[HttpPost("Wallets/{registerId}")]
		//[EnableQuery]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<CreateWalletResponse>> RebuildWallet([FromBody] CreateWalletRequest createWalletRequest, [FromRoute] string registerId)
		{
			_logger.LogInformation("Rebuilding Wallet: {req}", createWalletRequest);
			if (createWalletRequest.Mnemonic is null || createWalletRequest.Mnemonic == string.Empty)
				throw new HttpStatusException(HttpStatusCode.BadRequest, "Request must contain a mnemonic to rebuild the wallet.");

			var (wallet, mnemonic) = await _walletFactory.BuildWallet(createWalletRequest.Name, createWalletRequest.Mnemonic, registerId, GetUserTenant(), GetUserSub(), false);
			await _walletRepository.SaveWallet(wallet, wallet.Owner);
			await _daprClient.PublishEventAsync(WalletConstants.PubSub, Topics.WalletAddressCreationTopicName, wallet.Addresses);
			var response = CreateWalletResponse(wallet, mnemonic);
			return Accepted(response);
		}

		/// <summary>
		/// Creates a wallet and generates a master keyset
		/// </summary>
		/// <param name="createWalletRequest">Initial information needed to create a wallet.</param>
		/// <returns>The created wallet object</returns>
		[HttpPost("Wallets")]
		//[EnableQuery]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<CreateWalletResponse>> CreateWallet([FromBody] CreateWalletRequest createWalletRequest)
		{
			_logger.LogInformation("Creating Wallet: {req}", createWalletRequest);
			var (wallet, mnemonic) = await _walletFactory.BuildWallet(createWalletRequest.Name, createWalletRequest.Mnemonic, null, GetUserTenant(), GetUserSub(), true);
			await _walletRepository.SaveWallet(wallet, wallet.Owner);
			var newAddress = new WalletAddress { WalletId = wallet.Address, Address = wallet.Address };
			await _daprClient.PublishEventAsync(WalletConstants.PubSub, Topics.WalletAddressCreationTopicName, newAddress);
			var response = CreateWalletResponse(wallet, mnemonic);
			return Accepted(response);
		}

		/// <summary>
		/// Updates a wallet
		/// </summary>
		/// <param name="updateWallet">Data needed to update a wallet.</param>
		/// <param name="address">Address of the wallet to update.</param>
		/// <returns>The updated wallet object</returns>
		[HttpPatch("Wallets/{address}")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<Wallet>> UpdateWallet([FromRoute] string address, [FromBody] UpdateWalletRequest updateWallet)
		{
			_logger.LogInformation("Updating Wallet: {path}", updateWallet);
			var wallet = await _walletRepository.UpdateWallet(address, GetUserSub(), updateWallet);
			return Accepted(wallet);
		}

		/// <summary>
		/// Deletes a wallet
		/// </summary>
		/// <param name="address">Address of the wallet to update.</param>
		/// <returns>Status result</returns>
		[HttpDelete("Wallets/{address}")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<Wallet>> Delete([FromRoute] string address)
		{
			_logger.LogInformation("Deleting Wallet: {address}", address);
			await _walletRepository.DeleteWallet(address, GetUserSub());
			return Ok();
		}

		/// <summary>
		/// Returns the list of transactions for the supplied wallet.
		/// </summary>
		/// <param name="address">The public key of the desired wallet to retrieve from storage</param>
		/// <returns>List of received transactions</returns>
		[HttpGet("Wallets/{address}/transactions")]
		//[EnableQuery]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<IEnumerable<WalletTransaction>>> GetTransactions([FromRoute] string address)
		{
			var wallet = await _walletRepository.GetWallet(address, GetUserSub());
			return Ok(wallet.Transactions);
		}

		/// <summary>
		/// Returns the decrypted payloads inteded for the supplied wallet.
		/// </summary>
		/// <param name="address">The public key of the desired wallet to retrieve from storage</param>
		/// <param name="id">The id of the transaction whose payloads will be decrypted</param>
		/// <returns>List of received transactions</returns>
		[HttpGet("Wallets/{address}/transactions/{id}")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<List<string>>> GetTransactionById([FromRoute] string address, [FromRoute] string id)
		{
			return Ok(await _registerServiceClient.GetTransactionById(GetRegisterAvailableToUser(), id));
		}

		/// <summary>
		/// Signs a transaction.
		/// </summary>
		/// <param name="address">The wallet address of the wallet which will create the transaction</param>
		/// <param name="transaction">The raw unsigned transaction</param>
		/// <returns>The the retrieved wallet object</returns>
		[HttpPost("Wallets/{address}/transactions")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<TransactionModel>> SignTransaction([FromRoute] string address, [FromBody] Transaction transaction)
		{
			var wallet = await _walletRepository.GetWallet(address, GetUserSub()) ?? throw new HttpStatusException(HttpStatusCode.NotFound, $"Wallet with address: {address}, could not be found.");
			(_, ITxFormat? tx) = TransactionBuilder.Build(transaction);
			tx!.SignTx(wallet.PrivateKey);
			TransactionModel? registerTransaction = TransactionFormatter.ToModel(tx.GetTxTransport().transport);

			List<string> recipients = new(tx.GetTxRecipients().recipients);
			bool[] spent = new bool[recipients.Count + 1];
			if (!recipients.Contains(wallet.Address!))
			{
				recipients.Add(wallet.Address!);
				spent[^1] = true;
			}
			for (int i = 0; i < recipients.Count; i++)
			{
				WalletTransaction wt = BuildWalletTransaction(recipients[i], registerTransaction, registerTransaction!.MetaData, spent[i]);
				await _walletRepository.AddWalletTransaction(recipients[i], wt);
			}
			await _daprClient.PublishEventAsync(WalletConstants.PubSub, Topics.TransactionPendingTopicName, registerTransaction);
			return registerTransaction!;
		}

		private static WalletTransaction BuildWalletTransaction(string walletAddress, TransactionModel? tx, TransactionMetaData? transactionMetaData, bool isSpent = false)
		{
			WalletTransaction walletTransaction = new()
			{
				WalletId = walletAddress,
				MetaData = transactionMetaData,
				TransactionId = tx?.Id,
				PreviousId = tx?.PrevTxId,
				Sender = tx?.SenderWallet,
				ReceivedAddress = walletAddress,
				isSpent = isSpent
			};
			return walletTransaction;
		}

		/// <summary>
		/// Decrypts payloads for the user/client in context.
		/// </summary>
		/// <param name="address">The address of the wallet which will decrypt the payloads. Calling client must have authoriztion to use it.</param>
		/// <param name="transaction">The transaction whose payloads will be decrypted for the user.</param>
		/// <returns>The the retrieved wallet object</returns>
		[HttpPost("Wallets/{address}/transactions/decrypt")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<byte[][]>> DecryptTransactionPayloads([FromRoute] string address, [FromBody] TransactionModel transaction)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			var wallet = await _walletRepository.GetWallet(address, GetUserSub());
			_logger.LogDebug("Get wallet {addr} took {ms} ms", address, stopwatch.ElapsedMilliseconds);
			stopwatch.Restart();

			if (wallet is null)
				throw new HttpStatusException(HttpStatusCode.NotFound, $"Wallet with address: {address}, could not be found.");

			var cryptoTx = TransactionBuilder.Build(TransactionFormatter.ToTransaction(transaction));
			_logger.LogDebug("Get wallet {addr} took {ms} ms", address, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();

			return cryptoTx.transaction!.GetTxPayloadManager().GetAccessiblePayloadsData(wallet.PrivateKey).data!;
		}

		/// <summary>
		/// Decrypts payloads that haven't been encrypted for any wallet (Public payload data)
		/// </summary>
		/// <param name="transaction">The raw encrypted transaction</param>
		/// <returns>The decrypted transactions</returns>
		[HttpPost("Wallets/transactions/decrypt")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public ActionResult<byte[][]> GetAccessiblePayloads([FromBody] TransactionModel transaction)
		{
			var cryptoTx = TransactionBuilder.Build(TransactionFormatter.ToTransaction(transaction));
			return cryptoTx.transaction!.GetTxPayloadManager().GetAccessiblePayloadsData().data!;
		}

		/// <summary>
		/// Validates a transaction.
		/// </summary>
		/// <param name="address">The address of the wallet which will decrypt the payloads. Calling client must have authoriztion to use it.</param>
		/// <param name="transaction">The transaction whose payloads will be decrypted for the user.</param>
		/// <returns>The the retrieved wallet object</returns>
		[HttpPost("Wallets/{address}/transactions/validate")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public ActionResult<bool> IsValidTransaction([FromRoute] string address, [FromBody] TransactionModel transaction)
		{
			var cryptoTx = TransactionBuilder.Build(TransactionFormatter.ToTransaction(transaction));
			if (cryptoTx.result != Status.STATUS_OK)
				return false;
			var result = cryptoTx.transaction!.VerifyTx();
			return result == Status.STATUS_OK;
		}

		/// <summary>
		/// Get all the delegates that have access to a wallet
		/// </summary>
		/// <param name="address">The address of the wallet.</param>
		/// <returns>A list of wallet delegates.</returns>
		[HttpGet("Wallets/{address}/delegates")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<List<WalletAccess>>> GetWalletDelegates([FromRoute] string address)
		{
			var wallet = await _walletRepository.GetWallet(address, GetUserSub());
			return Ok(wallet.Delegates);
		}

		/// <summary>
		/// Adds the delegate to a wallet
		/// </summary>
		/// <param name="address">The address of the wallet.</param>
		/// <param name="walletAccesses">The Delegation descriptions.</param>
		/// <returns>Result or Exception.</returns>
		[HttpPost("Wallets/{address}/delegates")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<List<WalletAccess>>> AddWalletDelegates([FromRoute] string address, [FromBody] List<WalletAccess> walletAccesses)
		{
			var userTenant = GetUserTenant();
			foreach (var walletAccess in walletAccesses)
			{
				if (address != walletAccess.WalletId)
					return BadRequest("The wallet address does not match the walletId");
				walletAccess.Tenant = userTenant;
			}
			await _walletRepository.AddDelegates(address, walletAccesses, GetUserSub());
			return Accepted();
		}

		/// <summary>
		/// Remove the delegate "subject" from a wallet
		/// </summary>
		/// <param name="address">The address of the wallet.</param>
		/// <param name="subject">The delegate to remove.</param>
		/// <returns>A list of wallet delegates.</returns>
		[HttpDelete("Wallets/{address}/{subject}/delegates")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<List<WalletAccess>>> RemoveWalletDelegate([FromRoute] string address, [FromRoute] string subject)
		{
			await _walletRepository.RemoveDelegate(address, subject, GetUserSub());
			return Accepted();
		}

		/// <summary>
		/// Updates the delegate in a wallet
		/// </summary>
		/// <param name="address">The address of the wallet.</param>
		/// <param name="walletAccess">The Delegation description.</param>
		/// <returns>Result or Exception.</returns>
		[HttpPatch("Wallets/{address}/delegates")]
		[Authorize(Roles = Constants.WalletUserRole)]
		public async Task<ActionResult<List<WalletAccess>>> UpdateWalletDelegate([FromRoute] string address, [FromBody] WalletAccess walletAccess)
		{
			walletAccess.Tenant = GetUserTenant();
			await _walletRepository.UpdateDelegate(address, walletAccess, GetUserSub());
			return Accepted();
		}
		private string GetUserSub() { return Request.HttpContext.User.FindFirst(claim => claim.Type.Equals("sub", System.StringComparison.CurrentCultureIgnoreCase))!.Value; }
		private string GetUserTenant() { return Request.HttpContext.User.FindFirst(claim => claim.Type.Equals("tenant", System.StringComparison.CurrentCultureIgnoreCase))!.Value; }
		private string GetRegisterAvailableToUser()
		{
			var Claims = Request.HttpContext.User.FindFirst(claim => claim.Type.Equals("registers", System.StringComparison.CurrentCultureIgnoreCase));
			return Claims == null ?
				throw new HttpStatusException(HttpStatusCode.NotFound, "User has no registers in their claims") : Claims.Value;
		}

		private static CreateWalletResponse CreateWalletResponse(Wallet wallet, string mnemonic)
		{
			var response = new CreateWalletResponse(wallet, mnemonic);
			return response;
		}
	}
}
