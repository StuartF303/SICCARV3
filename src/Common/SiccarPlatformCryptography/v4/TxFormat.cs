// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

// Transaction V4 Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV4
{
	[Serializable]
	public class TxFormat : ITxFormat
	{
		public enum Operation { SIGNING, HASHING }

/**
 * Private Transaction fields
 **/
		private readonly PayloadManager Manager;
		private readonly UInt32 Version = WalletUtils.UInt32Exchange(4 | (UInt32)TransportIdentifier.Transaction);
		private byte[]? TxHash;
		private byte[] PreviousTxHash = new byte[32];
		private WalletUtils.CryptoKey? SenderWallet;
		private WalletUtils.CryptoKey[]? RecipientsWallets;
		private UInt64 TimeStamp = 0;
		private byte[]? MetaData;
		private byte[]? Signature;
		private readonly CryptoModule CryptoModule;

/*******************************************************************************\
* TxFormat()																	*
* Private constructor for the Transaction class so as to be only callable from	*
* the TransactionBuilder. Cascading constructors that can take an optional		*
* CryptoModule or create a default one if not supplied.							*
\*******************************************************************************/
		private TxFormat() : this(new CryptoModule()) {}
		private TxFormat(CryptoModule module)
		{
			ConstructorInfo? ci = typeof(PayloadManager).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, 
				[typeof(CryptoModule)], null);
			Manager = (PayloadManager)ci!.Invoke([module]);
			CryptoModule = module;
		}

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
			if (SenderWallet != null && SenderWallet.Value.Key != null && SenderWallet.Value.Key.Length == SenderWallet.Value.Network.GetPubKeySizeAttribute())
			{
				string? pk = WalletUtils.PubKeyToWallet(SenderWallet.Value.Key, (byte)SenderWallet!.Value.Network);
				if (pk != null)
					return (Status.STATUS_OK, pk);
			}
			return (Status.TX_NOT_SIGNED, string.Empty);
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
			List<string> wallets = [];
			if (RecipientsWallets != null)
			{
				for (int i = 0; i < RecipientsWallets.Length; i++)
					wallets.Add(WalletUtils.PubKeyToWallet(RecipientsWallets[i].Key, (byte)RecipientsWallets[i].Network)!);
			}
			return (Status.STATUS_OK, wallets.ToArray());
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
			return TimeStamp != 0 ? (Status.STATUS_OK, DateTimeOffset.FromUnixTimeSeconds((long)TimeStamp).UtcDateTime.ToString("O")) :
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
				try { return (Status.STATUS_OK, JsonSerializer.Serialize(System.Text.Encoding.ASCII.GetString(MetaData))); }
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
			return (TxHash != null && TxHash.Length == TxDefs.SHA256HashSize) ? (Status.STATUS_OK, WalletUtils.ByteArrayToHexString(TxHash)) :
				(Status.TX_NOT_SIGNED, null);
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
			return (Signature != null && Signature.Length > 0) ? (Status.STATUS_OK, WalletUtils.ByteArrayToHexString(Signature)) :
				(Status.TX_NOT_SIGNED, null);
		}

/*******************************************************************************\
* Get() Method for the PayloadManager that allows the user to work directly		*
* with each individual payload or collection of payloads.						*
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
		public (Status result, bool[]? recipients) SetTxRecipients(string[]? wallets = null)
		{
			if (wallets == null)
				return (Status.TX_INVALID_WALLET, null);
			else if (wallets.Length < 1)
			{
				if (RecipientsWallets != null && RecipientsWallets.Length > 0)
				{
					ResetSignedFields();
					RecipientsWallets = null;
				}
				return (Status.STATUS_OK, []);
			}
			(bool[] Valid, WalletUtils.CryptoKey[]? Wallets) = WalletUtils.WalletsCheck(wallets);
			if (Wallets != null && Wallets.Length > 0)
			{
				ResetSignedFields();
				RecipientsWallets = Wallets;
			}
			return (Status.STATUS_OK, Valid);
		}

/*******************************************************************************\
* SetTxMetaData()																*
* This method used to set the metadata for a transaction which is supplied as	*
* a string. If a transaction already contains metadata, it will be replaced by	*
* this call. MetaData is optional and if not specified, then its key/value pair *
* will not appear in any subsequent JSON output. MetaData is parsed for			*
* correctness and will be rejected if not valid JSON. Using of this method will	*
* require a transaction to be re-signed.										*
\*******************************************************************************/
		public Status SetTxMetaData(string? meta = null)
		{
			try
			{
				if (meta != null && meta.Length > 0 && JsonDocument.Parse(meta) != null)
				{
					if (TxSparse())
						return Status.TX_PAYLOAD_IS_SPARSE;
					byte[] m = Encoding.ASCII.GetBytes(meta);
					if (!new ReadOnlySpan<byte>(m).SequenceEqual(MetaData))
					{
						MetaData = m;
						ResetSignedFields();
					}
					return Status.STATUS_OK;
				}
			}
			catch (Exception) { }
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
			if (hash == null || hash.Length < 1)
				hash = TxDefs.NullTxId;
			byte[]? h = WalletUtils.HexStringToByteArray(hash);
			if (h == null || h.Length != TxDefs.SHA256HashSize)
				return Status.TX_INVALID_PREVTX_HASH;
			if (TxSparse())
				return Status.TX_PAYLOAD_IS_SPARSE;
			ResetSignedFields();
			PreviousTxHash = h;
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
			if (TxSparse())
				return Status.TX_PAYLOAD_IS_SPARSE;
			if (Signature != null && Signature.Length > 1 && !TxModified())
				return Status.STATUS_OK;
			(WalletNetworks Network, byte[] PrivateKey) = (default, []);
			if (CryptoModule.AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
			{
				var pkey = WalletUtils.WIFToPrivKey(privkey);
				if (pkey == null || pkey.Value.PrivateKey.Length < 1 || !Enum.IsDefined(typeof(WalletNetworks), pkey.Value.Network))
					return Status.TX_INVALID_KEY;
				Network = (WalletNetworks)pkey.Value.Network;
				PrivateKey = pkey.Value.PrivateKey;
			}
			TimeStamp = (UInt64)DateTimeOffset.Now.ToUnixTimeSeconds();
			Status result;
			(result, byte[]? data) = SerializeTxData(Operation.SIGNING);
			if (result == Status.STATUS_OK && data != null && data.Length > 0)
			{
				(result, Signature) = CryptoModule.Sign(WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(data))!, (byte)Network, PrivateKey);
				if (result == Status.STATUS_OK && Signature != null && Signature.Length > 0)
				{
					(result, byte[]? pkey) = CryptoModule.CalculatePublicKey((byte)Network, PrivateKey);
					if (result == Status.STATUS_OK && pkey != null && pkey.Length == Network.GetPubKeySizeAttribute())
					{
						SenderWallet = new WalletUtils.CryptoKey(Network, pkey);
						(result, data) = SerializeTxData(Operation.HASHING);
						if (result == Status.STATUS_OK && data != null && data.Length > 0)
						{
							TxHash = WalletUtils.ComputeSHA256Hash(data);
							TxModified(true);
							return Status.STATUS_OK;
						}
					}
				}
			}
			ResetSignedFields();
			return result;
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
			if (Signature == null || Signature.Length < 1 || SenderWallet == null || SenderWallet.Value.Key == null ||
				SenderWallet.Value.Key.Length != SenderWallet.Value.Network.GetPubKeySizeAttribute() || TxModified())
				return Status.TX_NOT_SIGNED;
			(Status result, byte[]? data) = SerializeTxData(Operation.SIGNING);
			if (result != Status.STATUS_OK || data == null || data.Length < 1)
				return result;
			if (CryptoModule.Verify(Signature, WalletUtils.ComputeSHA256Hash(WalletUtils.ComputeSHA256Hash(data))!,
				(byte)SenderWallet.Value.Network, SenderWallet.Value.Key) != Status.STATUS_OK)
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
			if (TxSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
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
* This is an internal method that builds a ITxFormat transaction from a			*
* supplied Transaction class. It is not publically accessible and only called	*
* from the TransactionBuilder. It should not be called directly. The			*
* CryptoModule provided to the TransactionBuilder, or the default will be used	*
* used to perform signature verification and payload integrity checks after the	*
* build	process completes. All size fields are verified during the build, along	*
* with other security fields such as the TimeStamp.								*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked method")]
		private Status Build(Transaction tx, bool sparse)
		{
			static (Status, UInt32, WalletUtils.CryptoKey?) ReadWallet(BinaryReader reader)
			{
				(UInt32 Size, UInt32 Count) = ((UInt32, UInt32))WalletUtils.ReadVLCount(reader);
				byte network = reader.ReadByte();
				if (!Enum.IsDefined(typeof(WalletNetworks), network))
					return (Status.TX_BAD_FORMAT, 0, null);
				WalletUtils.CryptoKey set = new((WalletNetworks)network, WalletUtils.ReadVLArray(reader));
				return set.Key == null || set.Key.Length != set.Network.GetPubKeySizeAttribute() ? (Status.TX_BAD_FORMAT, 0, null) : (Status.STATUS_OK, Size + Count, set);
			}
			static (Status, WalletUtils.CryptoKey[]?) ReadRecipients(BinaryReader reader)
			{
				UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
				if (count < 1)
					return (Status.STATUS_OK, null);
				WalletUtils.CryptoKey[] wallets = new WalletUtils.CryptoKey[count];
				UInt32 blksz = (UInt32)WalletUtils.ReadVLSize(reader);
				for (int i = 0; i < count; i++)
				{
					var (status, Count, Address) = ReadWallet(reader);
					if (status != Status.STATUS_OK || Address == null)
						return (Status.TX_BAD_FORMAT, null);
					blksz -= Count;
					wallets[i] = new WalletUtils.CryptoKey(Address.Value.Network, Address.Value.Key);
				}
				return (blksz > 0) ? (Status.TX_BAD_FORMAT, null) : (Status.STATUS_OK, wallets);
			}
			static (Status, byte[]?, byte[]?, WalletUtils.CryptoKey?) ReadSignature(string? TxId, ulong Ts, BinaryReader reader)
			{
				if (string.IsNullOrEmpty(TxId))
					return Ts > 0 ? (Status.TX_BAD_FORMAT, null, null, null) : (Status.STATUS_OK, null, null, null);
				byte[]? Hash = WalletUtils.HexStringToByteArray(TxId);
				if (Hash?.Length != TxDefs.TxHashSize || Ts == 0)
					return (Status.TX_BAD_FORMAT, null, null, null);
				UInt32 blksz = (UInt32)WalletUtils.ReadVLSize(reader);
				(byte[] Data, UInt32 Count) = WalletUtils.ReadVLArrayCount(reader);
				var (status, Size, Address) = ReadWallet(reader);
				return status != Status.STATUS_OK ? (status, null, null, null) : ((blksz - Count - Size - Data.Length != 0) ? (Status.TX_BAD_SIGNATURE, null, null, null) : (Status.STATUS_OK, Data, Hash, Address));
			}
			Status status = Status.TX_BAD_FORMAT;
			if (tx != null && tx.Data != null && tx.Data.Length >= TxDefs.MinTxSize)
			{
				try
				{
					BinaryReader reader = new(new MemoryStream(tx.Data));
					if (reader.ReadUInt32() == Version)
					{
						PreviousTxHash = WalletUtils.ReadVLArray(WalletUtils.SkipVLSize(reader));
						(status, WalletUtils.CryptoKey[]? Wallets) = ReadRecipients(reader);
						if (status == Status.STATUS_OK)
						{
							if (Wallets != null && Wallets.Length > 0)
								RecipientsWallets = Wallets;
							TimeStamp = reader.ReadUInt64();
							MetaData = WalletUtils.ReadVLArray(reader);
							(status, byte[]? SigData, byte[]? Hash, WalletUtils.CryptoKey? Address) = ReadSignature(tx.TxId, TimeStamp, reader);
							if (status == Status.STATUS_OK)
							{
								if (Address != null && SigData != null && Hash != null)
								{
									Signature = SigData;
									SenderWallet = Address;
									TxHash = Hash;
								}
								MethodInfo? method = Manager.GetType().GetMethod("BuildPayloads", BindingFlags.Instance | BindingFlags.NonPublic);
								if (method == null)
									status = Status.TX_BAD_FORMAT;
								else
								{
									status = (Status)method.Invoke(Manager, [reader, sparse])!;
									if (status == Status.STATUS_OK)
									{
										TxModified(true);
										status = VerifyTx();
										if (status == Status.STATUS_OK || status == Status.TX_NOT_SIGNED)
											return Status.STATUS_OK;
									}
								}
							}
						}
					}
				}
				catch (Exception) { status = Status.TX_BAD_FORMAT; }
			}
			if (status != Status.STATUS_OK)
				ResetSignedFields();
			return status;
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
			static byte[] AddRecipientsBlock(WalletUtils.CryptoKey[]? wallets)
			{
				List<byte> hdr = [];
				if (wallets == null || wallets.Length < 1)
					hdr.Add(0);
				else
				{
					hdr.AddRange(WalletUtils.VLEncode(wallets.Length));
					List<byte> rtotal = [];
					for (int i = 0; i < wallets.Length; i++)
					{
						List<byte> recip = [(byte)wallets[i].Network, .. WalletUtils.VLEncode(wallets[i].Key!.Length), .. wallets[i].Key!];
						rtotal.AddRange(WalletUtils.VLEncode(recip.Count));
						rtotal.AddRange(recip);
					}
					hdr.AddRange(WalletUtils.VLEncode(rtotal.Count));
					hdr.AddRange(rtotal);
				}
				return [.. hdr];
			}
			static byte[] AddSignatureBlock(byte[] sig, WalletUtils.CryptoKey sender)
			{
				List<byte> sigdata = [.. WalletUtils.VLEncode(sig.Length), .. sig];
				List<byte> senddata = [(byte)sender.Network, .. WalletUtils.VLEncode(sender.Key!.Length), .. sender.Key];
				sigdata.AddRange(WalletUtils.VLEncode(senddata.Count));
				sigdata.AddRange(senddata);
				sigdata.InsertRange(0, WalletUtils.VLEncode(sigdata.Count));
				return [.. sigdata];
			}
			static byte[] AddMetaDataBlock(byte[]? meta)
			{
				List<byte> metadata = [];
				if (meta == null || meta.Length < 1)
					metadata.Add(0);
				else
				{
					metadata.AddRange(WalletUtils.VLEncode(meta.Length));
					metadata.AddRange(meta);
				}
				return [.. metadata];
			}

			if (Manager.GetPayloadsCount() < 1)
				return (Status.TX_NO_PAYLOAD, null);
			List<byte> data = [.. BitConverter.GetBytes(Version)];
			if (PreviousTxHash != null && PreviousTxHash.Length == TxDefs.TxHashSize)
			{
				List<byte> append =
				[
					.. WalletUtils.VLEncode(PreviousTxHash.Length),
					.. PreviousTxHash,
					.. AddRecipientsBlock(RecipientsWallets),
					.. BitConverter.GetBytes(TimeStamp),
					.. AddMetaDataBlock(MetaData),
				];
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
				if (oper == Operation.HASHING && Signature != null && Signature.Length > 0 && SenderWallet != null && SenderWallet.Value.Key?.Length > 0)
					data.AddRange(AddSignatureBlock(Signature, new WalletUtils.CryptoKey(SenderWallet.Value.Network, SenderWallet.Value.Key)));
				data.AddRange(WalletUtils.VLEncode(Manager.GetPayloadsCount()));
				MethodInfo? method = Manager.GetType().GetMethod("SerializePayloads", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					(Status result, List<byte>? data)? info = ((Status result, List<byte>? data)?)method.Invoke(Manager, [oper]);
					if (info != null && info.Value.data != null)
					{
						if (info.Value.result != Status.STATUS_OK)
							return (info.Value.result, null);
						data.AddRange(info.Value.data);
						return (info.Value.result, data.ToArray());
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
* TxSparse()																	*
* This internal method is used to determine if the transaction has been loaded	*
* as sparse. Operations that modify the transaction are prohibited in this		*
* mode, as is exporting and signing.											*
\*******************************************************************************/
		private bool TxSparse()
		{
			(Status status, IPayloadInfo? info) = Manager.GetPayloadInfo(1);
			return status == Status.STATUS_OK && info != null && info.GetPayloadSparse();
		}

/*******************************************************************************\
* ResetSignedFields()															*
* This method is used internally to set the state of a transaction to unsigned.	*
* It clears all of the fields that are required to be set when a transaction is *
* signed and should not be called outside of this class.						*
\*******************************************************************************/
		private void ResetSignedFields() { TxHash = Signature = null; SenderWallet = null; TimeStamp = 0; }

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
				catch (Exception) { }
			}
			return id;
		}
	}
}
