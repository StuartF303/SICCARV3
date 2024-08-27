using Siccar.Application;
using Siccar.Platform;
using System.Threading.Tasks;
#nullable enable

namespace ActionService.V1.Services
{
	public interface ITransactionRequestBuilder
	{
		Task<Transaction> BuildTransactionRequest(Blueprint blueprint, ActionSubmission actionSubmission);
		Task<Transaction> BuildTransactionRequestFromPreviousTransaction(Blueprint blueprint, ActionSubmission actionSubmission, TransactionModel previousActionTx);
	}
}