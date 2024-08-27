// MemoryPool Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using Siccar.Platform;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Registers.ValidatorCore
{
	public interface IMemPool
	{
		public int RegisterCount();
		public void AddTxToPool(TransactionModel transaction);
		public List<TransactionModel> GetPool(string RegisterId = "");
	}
}
