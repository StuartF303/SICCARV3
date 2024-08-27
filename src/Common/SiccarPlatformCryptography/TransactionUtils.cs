// Wallet Utilities Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public class TransactionUtils
	{
		public static (TransportIdentifier Ident, TransactionVersion Version)? GetVersion(Transaction? tx)
		{
			if (tx != null && tx.Data != null)
			{
				try
				{
					UInt32 ver = BitConverter.ToUInt32(tx.Data, 0);
					TransportIdentifier id = TransportIdentifier.Transaction;
					if (ver != 1 && ver != 2)
					{
						const UInt32 mask = 0xf0000000;
						ver = WalletUtils.UInt32Exchange(ver);
						id = (TransportIdentifier)(ver & mask);
						ver &= ~mask;
					}
					return (id, (TransactionVersion)ver);
				}
				catch (Exception) {}
			}
			return null;
		}
	}
}