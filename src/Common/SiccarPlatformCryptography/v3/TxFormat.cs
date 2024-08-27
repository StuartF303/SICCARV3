/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

// Transaction V3 Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV3
{
	[Serializable]
	public class TxFormat : ITxFormat
	{
		public enum Operation { SIGNING, HASHING }

/**
 * Private Transaction fields
 **/
		private readonly PayloadManager Manager = new();
		private readonly UInt32 Version = WalletUtils.UInt32Exchange(3 | (UInt32)TransportIdentifier.Transaction);
		private byte[]? TxHash;
		private byte[] PreviousTxHash = new byte[32];
		private byte[]? SenderWallet;
		private string[]? RecipientsWallets;
		private UInt64 TimeStamp = 0;
		private byte[]? MetaData;
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
		public UInt32 GetTxVersion() { return WalletUtils.UInt32Exchange(Version) & ~(UInt32)TransportIdentifier.Transaction; }

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
		public (Status result, string[] recipients) GetTxRecipients()
		{
			return (Status.STATUS_OK, (RecipientsWallets != null && RecipientsWallets.Length > 0) ? (string[])RecipientsWallets.Clone() : []);
		}

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
* Returns the previous transaction hash as a hex string. If no previous			*
* transaction hash has been set, then a null hash of all zeros will be			*
* returned.																		*
\*******************************************************************************/
		public (Status result, string? hash) GetPrevTxHash() { return (Status.STATUS_OK, WalletUtils.ByteArrayToHexString(PreviousTxHash)); }

/*******************************************************************************\
* GetTxMetaData()																*
* Use this call to return as a string any metadata associated with the			*
* transaction. This call is guaranteed to succeed, but may return null for a	*
* transaction which contains no metadata.										*
\*******************************************************************************/
		public (Status result, string? metadata) GetTxMetaData()
		{
			if (MetaData != null)
			{
				try { return (Status.STATUS_OK, Encoding.ASCII.GetString(MetaData)); }
				catch {}
			}
			return (Status.TX_BAD_METADATA, null);
		}

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
			if (to_wallets == null)
				return (Status.TX_INVALID_WALLET, null);
			else if (to_wallets.Length < 1)
			{
				if (RecipientsWallets != null && RecipientsWallets.Length > 0)
				{
					ResetSignedFields();
					RecipientsWallets = null;
				}
				return (Status.STATUS_OK, Array.Empty<bool>());
			}
			HashSet<string> wallets = [];
			bool[] indexes = new bool[to_wallets.Length];
			for (int i = 0; i < to_wallets.Length; i++)
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(to_wallets[i]);
				indexes[i] = pk != null && pk.Value.PubKey.Length == WalletNetworks.ED25519.GetPubKeySizeAttribute() && wallets.Add(to_wallets[i]);
			}
			if (wallets.Count > 0)
			{
				if (RecipientsWallets == null || RecipientsWallets.Length < 1 || !wallets.SetEquals(new HashSet<string>(RecipientsWallets)))
				{
					ResetSignedFields();
					RecipientsWallets = [.. wallets];
				}
			}
			return (Status.STATUS_OK, indexes);
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
		public Status SetTxMetaData(string? meta = null)
		{
			try
			{
				if (meta != null && meta.Length > 0 && JsonDocument.Parse(meta) != null)
				{
					byte[] m = Encoding.ASCII.GetBytes(meta);
					if (!new ReadOnlySpan<byte>(m).SequenceEqual(MetaData))
					{
						MetaData = m;
						ResetSignedFields();
					}
					return Status.STATUS_OK;
				}
			}
			catch (Exception) {}
			return Status.TX_BAD_METADATA;
		}

/*******************************************************************************\
* SetPrevTxHash()																*
* The Previous Transaction Hash may be set using this call. It may also be used	*
* to reset the Previous Transaction Hash to a NullHash by suppling a null/no	*
* argument. Any hash value provided to this method must be of valid size and	*
* format or a result of	INVALID_PREV_TX_HASH will be returned. If this call is	*
* successful, then the transaction must be signed/re-signed to be valid.		*
\*******************************************************************************/
		public Status SetPrevTxHash(string? hash = null)
		{
			if (hash == null || hash.Length == 0)
				hash = TxDefs.NullTxId;
			byte[]? h = WalletUtils.HexStringToByteArray(hash);
			if (h == null || h.Length != TxDefs.SHA256HashSize)
				return Status.TX_INVALID_PREVTX_HASH;
			if (PreviousTxHash == null || PreviousTxHash.Length < TxDefs.SHA256HashSize || !new ReadOnlySpan<byte>(PreviousTxHash).SequenceEqual(h))
			{
				ResetSignedFields();
				PreviousTxHash = h;
			}
			return Status.STATUS_OK;
		}

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
					if ((WalletNetworks)Network != WalletNetworks.ED25519)
						return Status.TX_SIGNING_FAILURE;
					PrivateKey = pkey.Value.PrivateKey;
				}
			}
			TimeStamp = (UInt64)DateTimeOffset.Now.ToUnixTimeSeconds();
			(Status result, byte[]? data) raw_tx = SerializeTxData(Operation.SIGNING);
			if (raw_tx.result != Status.STATUS_OK || raw_tx.data == null)
			{
				ResetSignedFields();
				return raw_tx.result;
			}
			(Status status, Signature) = CryptoModule.Sign(WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(raw_tx.data))!, Network, PrivateKey);
			if (status == Status.STATUS_OK && Signature != null && Signature.Length > 0)
			{
				SenderWallet = CryptoModule.CalculatePublicKey(Network, PrivateKey).pubkey;
				if (Enum.IsDefined(typeof(WalletNetworks), Network))
				{
					raw_tx = SerializeTxData(Operation.HASHING);
					if (raw_tx.result != Status.STATUS_OK)
					{
						ResetSignedFields();
						return raw_tx.result;
					}
					TxHash = WalletUtils.ComputeSHA256Hash(raw_tx.data);
					TxModified(true);
					return Status.STATUS_OK;
				}
			}
			ResetSignedFields();
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
			(Status result, byte[]? data) = SerializeTxData(Operation.SIGNING);
			if (result != Status.STATUS_OK || data == null || data.Length < 1)
				return result;
			if (CryptoModule.Verify(Signature, WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(data))!, (byte)WalletNetworks.ED25519, SenderWallet) != Status.STATUS_OK)
				return Status.TX_BAD_SIGNATURE;
			return Manager.VerifyAllPayloadsData().result;
		}

/*******************************************************************************\
* GetTxTransport()																*
* This method outputs the transaction in a form that can be transported between	*
* services. The transaction is required to be built and signed before using		*
* this method. This should be the preferred way of encoding a transaction for	*
* transport as it is several orders of magnitude better performing than using	*
* a JSON format.																*
\*******************************************************************************/
		public (Status result, Transaction? transport) GetTxTransport()
		{
			TxModified();
			(Status status, byte[]? data) = SerializeTxData(Operation.HASHING);
			return (status != Status.STATUS_OK) ? (status, null) : new(Status.STATUS_OK, new Transaction()
			{
				TxId = WalletUtils.ByteArrayToHexString(TxHash),
				RegisterId = GetTxRegisterId(),
				Data = data
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
				try
				{
					if (reader.ReadUInt32() == Version)
					{
						PreviousTxHash = WalletUtils.ReadVLArray(reader);
						UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
						if (count > 0)
						{
							WalletUtils.ReadVLSize(reader);
							RecipientsWallets = new string[count];
							for (int i = 0; i < count; i++)
								RecipientsWallets[i] = WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)!;
						}
						TimeStamp = reader.ReadUInt64();
						MetaData = WalletUtils.ReadVLArray(reader);
						if (tx.TxId != null && tx.TxId.Length > 0)
						{
							WalletUtils.ReadVLSize(reader);
							Signature = WalletUtils.ReadVLArray(reader);
							SenderWallet = WalletUtils.ReadVLArray(reader);
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
									ResetSignedFields();
									if (result == Status.TX_NOT_SIGNED)
										result = Manager.VerifyAllPayloadsData().result;
								}
								else if (tx.TxId != null && tx.TxId.Length > 0)
									TxHash = WalletUtils.HexStringToByteArray(tx.TxId);
							}
							return result;
						}
					}
				}
				catch (Exception) {}
			}
			return Status.TX_BAD_FORMAT;
		}

/*******************************************************************************\
* SerializeTxData()																*
* This method is used to serialize the transaction for hash computation. It has *
* two modes of operation; serializing the transaction for signature generation	*
* and serializing for transaction hash generation. The transaction must contain	*
* at least one payload in order for this call to succeed. If no payload exists	*
* or serialization fails, an appropriate error will be returned and the			*
* serialization data will be null.												*
\*******************************************************************************/
		private (Status result, byte[]? txdata) SerializeTxData(Operation oper)
		{
			if (Manager.GetPayloadsCount() < 1)
				return (Status.TX_NO_PAYLOAD, null);
			List<byte> data = [.. BitConverter.GetBytes(Version)];
			if (PreviousTxHash != null && PreviousTxHash.Length == TxDefs.SHA256HashSize)
			{
				List<byte> append = [];
				data.AddRange(WalletUtils.VLEncode(PreviousTxHash.Length));
				data.AddRange(PreviousTxHash);
				if (RecipientsWallets == null || RecipientsWallets.Length < 1)
					data.Add(0);
				else
				{
					data.AddRange(WalletUtils.VLEncode(RecipientsWallets.Length));
					for (int i = 0; i < RecipientsWallets.Length; i++)
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(RecipientsWallets[i]);
						if (pk != null)
						{
							append.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
							append.AddRange(pk.Value.PubKey);
						}
					}
					data.AddRange(WalletUtils.VLEncode(append.Count));
					data.AddRange(append);
				}
				data.AddRange(BitConverter.GetBytes(TimeStamp));
				if (MetaData == null || MetaData.Length < 1)
					data.Add(0);
				else
				{
					data.AddRange(WalletUtils.VLEncode(MetaData.Length));
					data.AddRange(MetaData);
				}
				if (oper == Operation.HASHING && Signature != null && Signature.Length > 0 && SenderWallet != null && SenderWallet.Length > 0)
				{
					append.Clear();
					append.AddRange(WalletUtils.VLEncode(Signature.Length));
					append.AddRange(Signature);
					append.AddRange(WalletUtils.VLEncode(SenderWallet.Length));
					append.AddRange(SenderWallet);
					data.AddRange(WalletUtils.VLEncode(append.Count));
					data.AddRange(append);
				}
				data.AddRange(WalletUtils.VLEncode(Manager.GetPayloadsCount()));
				MethodInfo? method = Manager.GetType().GetMethod("SerializePayloads", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					(Status result, List<byte>? data)? info = ((Status result, List<byte>? data)?)method.Invoke(Manager, [oper]);
					if (info != null)
					{
						if (info.Value.result != Status.STATUS_OK)
							return (info.Value.result, null);
						data.AddRange(info.Value.data!);
						return (Status.STATUS_OK, data.ToArray());
					}
				}
			}
			return (Status.TX_SERIALIZE_FAILURE, null);
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
			ResetSignedFields();
			return true;
		}

/*******************************************************************************\
* ResetSignedFields()															*
* This method is used internally to set the state of a transaction to unsigned.	*
* It clears all of the fields that are required to be set when a transaction is *
* signed and should not be called outside of this class.						*
\*******************************************************************************/
		private void ResetSignedFields() { TxHash = SenderWallet = Signature = null; TimeStamp = 0; }

/*******************************************************************************\
* GetTxRegisterId()																*
* An internal method that gets the RegisterId from any MetaData set for the		*
* transaction. This method will only return the RegisterId if the transaction	*
* is signed and the MetaData is valid.											*
\*******************************************************************************/
		private string? GetTxRegisterId()
		{
			string? id = null;
			if (MetaData != null)
			{
				try
				{
					if (JsonDocument.Parse(MetaData).RootElement.TryGetProperty("RegisterId", out JsonElement element))
						id = element.GetString();
				}
				catch (Exception) {}
			}
			return id;
		}
	}
}
