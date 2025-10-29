// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

// Transaction V1 Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV1
{
	[Serializable]
	public class TxFormat : ITxFormat
	{
		public enum Operation { SIGNING, HASHING }

/**
 * Private Transaction fields
 **/
		private readonly PayloadManager Manager = new();
		private readonly UInt32 Version = 1;
		private byte[]? TxHash;
		private byte[]? SenderWallet;
		private ulong TimeStamp = 0;
		private byte[]? Signature;
		private readonly CryptoModule CryptoModule = new();

/*******************************************************************************\
* TxFormat()																	*
* Private constructor for the Transaction class so as to be only callable from	*
* the TransactionBuilder. No custom CryptoModule is supported by this class.	*
\*******************************************************************************/
		private TxFormat() {}

/**
 * Get() methods for the various fields of the transaction.
 **/

/*******************************************************************************\
* GetTxVersion()																*
* Returns the immutable version of the transaction. This fields is guaranteed	*
* to always be valid even if the transaction is unsigned. Use this field to		*
* determine the capability of the transaction version.							*
\*******************************************************************************/
		public UInt32 GetTxVersion() { return Version; }

/*******************************************************************************\
* GetTxSender()																	*
* Use this method to obtain the sender/signer of a transaction. The method		*
* returns a tuple containing a result of the call. If a status other than		*
* STATUS_OK is returned, then the wallet address will be null. This method is	*
* only valid after a transaction has been signed. Calling this method on an		*
* unsigned transaction will yield an error status and a sender wallet address	*
* of null.																		*
\*******************************************************************************/
		public (Status result, string? sender) GetTxSender()
		{
			if (SenderWallet != null && SenderWallet.Length > 0 && !TxModified())
			{
				string? pk = WalletUtils.PubKeyToWallet(SenderWallet, (byte)WalletNetworks.ED25519);
				if (pk != null)
					return (Status.STATUS_OK, pk);
			}
			return (Status.TX_NOT_SIGNED, null);
		}

/*******************************************************************************\
* GetTxRecipients()																*
* Returns a string array of recipient wallets for the transaction. If the		*
* transaction is a broadcast and has no recipients, then this method will		*
* return an empty array. It is assured that all wallet addresses returned by	*
* this call are valid.															*
\*******************************************************************************/
		public (Status result, string[] recipients) GetTxRecipients() { return TxRecipients(); }

/*******************************************************************************\
* GetTxTimeStamp()																*
* This method will return the timestamp for the transaction as a UTC string.	*
* The timestamp is only set during signing, so this method will return the		*
* default of 01 Jan 1970 and an Status of TX_NOT_SIGNED for any unsigned		*
* transaction. The timestamp is defaulted by any call which modifies			*
* transaction content and therefore is only valid after a call to SignTx()		*
* where the returned Status is STATUS_OK.										*
\*******************************************************************************/
		public (Status result, string? timestamp) GetTxTimeStamp()
		{
			return TimeStamp != 0 && !TxModified() ? (Status.STATUS_OK, DateTimeOffset.FromUnixTimeSeconds((long)TimeStamp).UtcDateTime.ToString("O")) :
				(Status.TX_NOT_SIGNED, null);
		}

/*******************************************************************************\
* GetPrevTxHash()																*
* Returns the previous transaction hash as a hex string. This is not supported	*
* functionality on this transaction version.									*
\*******************************************************************************/
		public (Status result, string? hash) GetPrevTxHash() { return (Status.TX_NOT_SUPPORTED, null); }

/*******************************************************************************\
* GetTxMetaData()																*
* Use this call to return as a string any metadata associated with the			*
* transaction. This is not supported functionality on this transaction			*
* version.																		*
\*******************************************************************************/
		public (Status result, string? metadata) GetTxMetaData() { return (Status.TX_NOT_SUPPORTED, null); }

/*******************************************************************************\
* GetTxHash()																	*
* This method will return the hash for a transaction as a hex string. It is		*
* only valid after a call to SignTX(). Any transaction that has not been		*
* signed, or has been modified subsequently to signing will return a status of	*
* TX_NOT_SIGNED and a null hash. The hash is guaranteed to be of a correct		*
* format and size if the result of the call is STATUS_OK.						*
\*******************************************************************************/
		public (Status result, string? hash) GetTxHash()
		{
			return TxHash != null && TxHash.Length == TxDefs.SHA256HashSize && !TxModified() ?
				(Status.STATUS_OK, WalletUtils.ByteArrayToHexString(TxHash)) : (Status.TX_NOT_SIGNED, null);
		}

/*******************************************************************************\
* GetTxSignature()																*
* Returns the signature for a transaction as a hex string. This call is only	*
* valid after a transaction has been signed. Any unsigned transaction, or		*
* transaction that has been modified following signing, will return a			*
* TX_NOT_SIGNED and a null signature.											*
\*******************************************************************************/
		public (Status result, string? signature) GetTxSignature()
		{
			return Signature != null && Signature.Length > 0 && !TxModified() ?
				(Status.STATUS_OK, WalletUtils.ByteArrayToHexString(Signature)) : (Status.TX_NOT_SIGNED, null);
		}

/*******************************************************************************\
* GetTxPayloadManager()															*
* Method to provide access to the PayloadManager that allows working with		*
* individual or collection of payloads.											*
\*******************************************************************************/
		public IPayloadManager GetTxPayloadManager() { return Manager; }

/**
 * Set() methods for the various fields of the transaction.
 **/

/*******************************************************************************\
* SetTxRecipients()																*
* Use this method for setting the recipients for a transaction by provding an	*
* array of wallet addresses. Recipients are not required for a transaction.		*
* Returns an array of indexes of each wallet address in the input that was		*
* added to the transaction, or null if no changes were made. Subsequent calls	*
* to this function replace existing recipients of a transaction. Note, the		*
* transaction must signed/re-signed if the returned index array is non null.	*
\*******************************************************************************/
		public (Status result, bool[]? recipients) SetTxRecipients(string[]? to_wallets = null)
		{
			if (to_wallets == null || to_wallets.Length < 1)
				return (Status.TX_INVALID_WALLET, null);
			if (Manager.GetPayloadsCount() > 0)
				return (Status.TX_NOT_SUPPORTED, null);
			HashSet<string> dup = [];
			bool[] index = new bool[to_wallets.Length];
			for (int i = 0; i < to_wallets.Length; i++)
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(to_wallets[i]);
				index[i] = pk != null && (pk.Value.PubKey.Length == WalletNetworks.ED25519.GetPubKeySizeAttribute()) && dup.Add(to_wallets[i]);
			}
			if (dup.Count > 0)
			{
				(_, string[] receivers) = GetTxRecipients();
				if (receivers.Length == dup.Count && new HashSet<string>(receivers).SetEquals(dup))
					index = new bool[to_wallets.Length];
				else
				{
					TxHash = Signature = null; TimeStamp = 0;
					MethodInfo? method = Manager.GetType().GetMethod("SetPayloadWallets", BindingFlags.Instance | BindingFlags.NonPublic);
					if (method == null)
						return (Status.TX_INVALID_WALLET, null);
					string[] valid = new string[dup.Count];
					dup.CopyTo(valid);
					method.Invoke(Manager, [valid]);
				}
			}
			return (Status.STATUS_OK, index);
		}

/*******************************************************************************\
* SetTxMetaData()																*
* This method used to set the metadata for a transaction which is supplied as	*
* a string. If a transaction already contains metadata, it will be replaced by	*
* this call. MetaData is optional and if not specified, then its key/value pair *
* will not appear in any subsequent JSON output. MetaData is parsed for			*
* correctness and will be rejected if not valid JSON. Using of this method will	*
* require a transaction to be signed/re-signed.									*
\*******************************************************************************/
		public Status SetTxMetaData(string? meta) { return Status.TX_NOT_SUPPORTED; }

/*******************************************************************************\
* SetPrevTxHash()																*
* The Previous Transaction Hash may be set using this call. It may also be used	*
* to reset the Previous Transaction Hash to a NullHash by suppling a null/no	*
* argument. Any hash value provided to this method must be of valid size and	*
* format or a result of	INVALID_PREV_TX_HASH will be returned. If this call is	*
* successful, then the transaction must be signed/re-signed to be valid.		*
\*******************************************************************************/
		public Status SetPrevTxHash(string? hash = null) { return Status.TX_NOT_SUPPORTED; }

/**
 * General Purpose Transaction Functions
 **/

/*******************************************************************************\
* SignTx()																		*
* This should be the last mathod invoked upon a transaction. It allows the		*
* Transaction Hash, TimeStamp and Signature to be available for retrieval,		*
* along with the Transaction sender. A appropriately initialized SigningModule	*
* is required for this call. 													*
\*******************************************************************************/
		public Status SignTx(string? privkey)
		{
			if (GetTxPayloadManager().GetPayloadsCount() < 1)
				return Status.TX_NO_PAYLOAD;
			if (Signature != null && Signature.Length > 0 && !TxModified())
				return Status.STATUS_OK;
			(byte Network, byte[] PrivateKey) = (0, []);
			if (CryptoModule.AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
			{
				var pkey = WalletUtils.WIFToPrivKey(privkey);
				if (pkey != null && pkey.Value.PrivateKey.Length > 0)
				{
					Network = pkey.Value.Network;
					PrivateKey = pkey.Value.PrivateKey;
				}
			}
			TimeStamp = (UInt64)DateTimeOffset.Now.ToUnixTimeSeconds();
			(Status status, Signature) = CryptoModule.Sign(WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(SerializeTxData(Operation.SIGNING)))!, Network, PrivateKey);
			if (status == Status.STATUS_OK && Signature != null && Signature.Length > 0)
			{
				SenderWallet = CryptoModule.CalculatePublicKey(Network, PrivateKey).pubkey;
				TxHash = WalletUtils.ComputeSHA256Hash(SerializeTxData(Operation.HASHING));
				TxModified(true);
				return Status.STATUS_OK;
			}
			TimeStamp = 0;
			return Status.TX_SIGNING_FAILURE;
		}

/*******************************************************************************\
* VerifyTx()																	*
* Using this method the integrity of the transaction can be confirmed. It		*
* checks if the signature for a transaction is valid and also verifies that any	*
* payload data has not been modified. An appropriately initialised				*
* SigningModule is required for this call.										*
\*******************************************************************************/
		public Status VerifyTx()
		{
			if (Signature == null || Signature.Length < 1 || SenderWallet == null || SenderWallet.Length != WalletNetworks.ED25519.GetPubKeySizeAttribute() || TxModified())
				return Status.TX_NOT_SIGNED;
			byte[]? data = SerializeTxData(Operation.SIGNING);
			if (CryptoModule.Verify(Signature, WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(data))!, (byte)WalletNetworks.ED25519, SenderWallet) != Status.STATUS_OK)
				return Status.TX_BAD_SIGNATURE;
			return Manager.VerifyAllPayloadsData().result;
		}

/*******************************************************************************\
* GetTxTransport()																*
* This method outputs the transaction in a form that can be transported between	*
* services. If the transaction is unsigned, then this function will return a	*
* raw transaction for later signing. Raw transactions are indicated by not		*
* having their TxId field set. A transaction must have at least one payload for	*
* this function call to succeed.												*
\*******************************************************************************/
		public (Status result, Transaction? transport) GetTxTransport()
		{
			TxModified();
			return (GetTxPayloadManager().GetPayloadsCount() < 1) ? (Status.TX_NO_PAYLOAD, null) : new(Status.STATUS_OK, new Transaction()
			{
				TxId = WalletUtils.ByteArrayToHexString(TxHash),
				RegisterId = null,
				Data = SerializeTxData(Operation.HASHING)
			});
		}

/*******************************************************************************\
* ToJSON()																		*
* Use this method to turn the transaction into a JSON object. This method is	*
* now obsolete and returns TX_NOT_SUPPORTED. JSON functionality is now provided	*
* by the TransactionFormatter instead.											*
\*******************************************************************************/
		[Obsolete("Use the TransactionFormater instead")]
		public (Status result, string? json) ToJSON() { return (Status.TX_NOT_SUPPORTED, null); }

/*******************************************************************************\
* Build()																		*
* This is an internal method that creates the built transaction classes from a	*
* supplied Transaction class. It is not publically accessible and is only		*
* called from the TransactionBuilder. It should not be called directly. The		*
* SigningModule argument is optional and if	not provided then no signature		*
* verification or payload integrity checks are performed on the built			*
* Transaction.																	*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked method")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0060: Version specific unused parameter")]
		private Status Build(Transaction tx, bool sparse)
		{
			if (tx != null && tx.Data != null)
			{
				BinaryReader reader = new(new MemoryStream(tx.Data));
				if (reader.ReadUInt32() == Version)
				{
					try
					{
						UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
						List<string> addr = [];
						for (int i = 0; i < count; i++)
						{
							string? wallet = WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519);
							if (wallet != null)
								addr.Add(wallet);
						}
						if (addr.Count > 0)
						{
							if (SetTxRecipients([.. addr]).result != Status.STATUS_OK || count != addr.Count)
								return Status.TX_INVALID_WALLET;
						}
						TimeStamp = reader.ReadUInt64();
						if (tx.TxId != null && tx.TxId.Length > 0)
						{
							Signature = reader.ReadBytes((int)WalletUtils.ReadVLSize(reader));
							SenderWallet = reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute());
						}
						MethodInfo? method = Manager.GetType().GetMethod("BuildPayloads", BindingFlags.Instance | BindingFlags.NonPublic);
						if (method != null)
						{
							Status result = (Status)method.Invoke(Manager, [reader])!;
							if (result == Status.STATUS_OK)
							{
								TxModified(true);
								result = VerifyTx();
								if (result != Status.STATUS_OK)
								{
									TxHash = SenderWallet = Signature = null; TimeStamp = 0;
									if (result == Status.TX_NOT_SIGNED)
										result = Manager.VerifyAllPayloadsData().result;
								}
								else if (tx.TxId != null && tx.TxId.Length > 0)
									TxHash = WalletUtils.HexStringToByteArray(tx.TxId);
							}
							return result;
						}
					}
					catch (Exception) {}
				}
			}
			return Status.TX_BAD_FORMAT;
		}

/*******************************************************************************\
* SerializeTxData()																*
* An internal method used to serialize the internal structures of the			*
* transaction into a sequence of bytes that can be hashed. This function should	*
* never be called directly. Supports serializing data for Signing and complete	*
* object hash creation.	No Status return is provided by this function.			*
\*******************************************************************************/
		private byte[] SerializeTxData(Operation oper)
		{
			List<byte> data = [.. BitConverter.GetBytes(Version)];
			(_, string[]? rw) = TxRecipients();
			if (rw == null || rw.Length < 1)
				data.Add(0);
			else
			{
				data.AddRange(WalletUtils.VLEncode(rw.Length));
				for (int i = 0; i < rw.Length; i++)
				{
					(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(rw[i]);
					if (pk != null)
						data.AddRange(pk.Value.PubKey);
				}
			}
			data.AddRange(BitConverter.GetBytes(TimeStamp));
			if (oper == Operation.HASHING && Signature != null && Signature.Length > 0)
			{
				data.AddRange(WalletUtils.VLEncode(Signature.Length));
				data.AddRange(Signature);
				data.AddRange(SenderWallet!);
			}
			data.AddRange(WalletUtils.VLEncode(Manager.GetPayloadsCount()));
			MethodInfo? method = Manager.GetType().GetMethod("SerializePayloads", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				(Status status, List<byte>? bytes) = ((Status status, List<byte>? bytes))method.Invoke(Manager, [oper])!;
				if (status == Status.STATUS_OK && bytes != null && bytes.Count > 0)
					data.AddRange(bytes);
			}
			return [.. data];
		}

/*******************************************************************************\
* TxRecipients()																*
* This internal method is used for obtaining the transaction recipients list	*
* from inside the PayloadManager. This allows the transaction to maintain a		*
* single recipient list. It can, but probably should not be called directly.	*
\*******************************************************************************/
		private (Status result, string[] recipients) TxRecipients()
		{
			MethodInfo? method = Manager.GetType().GetMethod("GetPayloadWallets", BindingFlags.Instance | BindingFlags.NonPublic);
			string[] r = [];
			if (method != null)
				r = (string[]?)method.Invoke(Manager, null) ?? [];
			return (Status.STATUS_OK, r);
		}

/*******************************************************************************\
* TxModified()																	*
* Use this internal method to read and set the internal modified state of the	*
* transaction. This method is called when signing the transaction to set the	*
* internal state of the transaction to unmodified.								*
\*******************************************************************************/
		private bool TxModified(bool state = false)
		{
			MethodInfo? method = Manager.GetType().GetMethod("TxModified", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				if (!(bool?)method.Invoke(Manager, [state])! ?? false)
					return false;
			}
			TxHash = null;
			SenderWallet = null;
			Signature = null;
			TimeStamp = 0;
			return true;
		}
	}
}
