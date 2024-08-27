using System.Collections.Generic;

namespace Siccar.Platform
{
	public class TransactionConfirmed
	{
		/// <summary>
		/// The transaction Id to be added to the wallet
		/// </summary>
		public string TransactionId { get; set; }

		/// <summary>
		/// The the previous transactionId to be removed from the senders wallet
		/// </summary>
		public string PreviousTransactionId { get; set; }

		/// <summary>
		/// The address of the sender
		/// </summary>
		public string Sender { get; set; }

		/// <summary>
		/// The address of the wallet to add the transaction
		/// </summary>
		public List<string> ToWallets { get; set; }

		/// <summary>
		/// The metadata of the transaction
		/// </summary>
		public TransactionMetaData MetaData {get;set;}
	}
}
