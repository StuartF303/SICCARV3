// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

// Transaction Builder Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Reflection;
#nullable enable

namespace Siccar.Platform.Cryptography
{
/************************************\
*   Available Transaction Versions	 *
\************************************/
	public enum TransactionVersion : UInt32
	{
		TX_VERSION_1 = 1,
		TX_VERSION_2 = 2,
		TX_VERSION_3 = 3,
		TX_VERSION_4 = 4,
		TX_VERSION_LATEST = TX_VERSION_4
	}

	public class TransactionBuilder
	{

/*******************************************************************************\
* Build()																		*
* Static method using for creating an empty transaction object. The transaction *
* is supplied as an argument. A version outside of the range of versions will	*
* yield the latest supported transaction version. The enum value				*
* TX_VERSION_LATEST can be used to always get the latest available transaction	*
* version.																		*
\*******************************************************************************/
		public static ITxFormat Build(TransactionVersion version, CryptoModule? module = null)
		{
			static T Construct<T>(CryptoModule? m = null)
			{
				return (T)typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
					m != null ? [typeof(CryptoModule)] : [], null)!.Invoke(m != null ? new object[] { m! } : null);
			}
			return (UInt32)version switch
			{
				1 => Construct<TransactionV1.TxFormat>(),
				2 => Construct<TransactionV2.TxFormat>(),
				3 => Construct<TransactionV3.TxFormat>(),
				4 => Construct<TransactionV4.TxFormat>(module),
				_ => Construct<TransactionV4.TxFormat>(module)
			};
		}

/*******************************************************************************\
* Build()																		*
* Static method for creating a transaction object from a JSON string. If the	*
* JSON is invalid or does not contain all of the required keys, an error status	*
* will be returned. The status will indicate whether a key/value pair, or the	*
* transaction format that is invalid. If the version is outside of the			*
* supported version range, then an error will be returned and the transaction	*
* will be null. MetaData is an optional key/value pair. Duplicate or invalid	*
* wallets found within the JSON will return a status of	TX_BAD_JSON_PROPERTY.	*
\*******************************************************************************/
		public static (Status result, ITxFormat? transaction) Build(Transaction? tx, bool sparse = false, CryptoModule? module = null)
		{
			var txvi = TransactionUtils.GetVersion(tx);
			if (txvi != null && txvi.Value.Ident == TransportIdentifier.Transaction)
			{
				try
				{
					ITxFormat t = Build(txvi.Value.Version, module);
					MethodInfo? method = t.GetType().GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic);
					if (method != null)
					{
						Status result = (Status)method.Invoke(t, [tx!, sparse])!;
						return result != Status.STATUS_OK ? (result, null) : (result, t);
					}
				}
				catch (Exception) {}
			}
			return (Status.TX_BAD_FORMAT, null);
		}
	}
}
