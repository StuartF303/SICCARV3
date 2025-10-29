// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Siccar.Platform;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public interface IRegisterServiceClient : ISiccarServiceClient
	{
		public Task<TransactionModel> GetTransactionById( string registerId, string txId);
		public Task<List<TransactionModel>> GetAllTransactions(string registerId, string query);
		public Task<ODataRaw<List<TransactionModel>>> GetAllTransactionsOData(string registerId, string query);
		public Task<List<TransactionModel>> GetBlueprintTransactions(string registerId);
		public Task<List<TransactionModel>> GetTransactionsByInstanceId(string registerId, string instanceId);
		public Task<List<TransactionModel>> GetAllTransactionsByBlueprintId(string registerId, string blueprintTxId);
		public Task<string> PostNewTransaction(string register, TransactionModel data);
		public Task<string> PostNewDocket(Docket data);
		public Task<Docket> GetDocketByHeight(string register, UInt64 height);
		public Task<List<Register>> GetRegisters();
		public Task<Register> GetRegister(string registerId);
		public Task<Register> CreateRegister(Register data);
		public Task<bool> DeleteRegister(string registerId);
		public Task<List<TransactionModel>> GetAllTransactionsBySenderAddress(string registerId, string senderAddress);
		public Task<List<TransactionModel>> GetAllTransactionsByRecipientAddress(string registerId, string recipientAddress);
		public Task<List<TransactionModel>?> GetParticipantTransactions(string registerId);
		public Task SubscribeRegister(string registerId);
		public Task UnSubscribeRegister(string registerId);
		public Task StartEvents();
		public Task<TransactionModel?> GetPublishedBlueprintTransaction(string registerId, string blueprintId);
		public delegate Task TransactionEventHandler(TransactionConfirmed transactionMessage);
		public event TransactionEventHandler OnTransactionValidationCompleted;
	}
}
