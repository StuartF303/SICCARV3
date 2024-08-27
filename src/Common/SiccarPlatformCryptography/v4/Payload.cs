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

// Transaction V4 Payload Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV4
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Mostly reflection invoked constructors and methods")]
	[Serializable]
	public class Payload : IPayload
	{

/*******************************************************************************\
* PayloadBox()																	*
* This is an internal payload class which stores a wallet address as a raw		*
* public key, its associated network and encrypted payload symmetric key. This	*
* class is instantiated for each wallet public key that can access the payload.	*
\*******************************************************************************/
		private class PayloadBox(WalletUtils.CryptoKey wallet, byte[] box)
		{
			private readonly WalletUtils.CryptoKey Wallet = wallet;
			private readonly byte[] CryptoBox = box;

			public byte[] GetCryptoBox() { return CryptoBox; }
			public WalletNetworks GetNetwork() { return Wallet.Network; }
			public byte[] GetWallet() { return Wallet.Key!; }
			public byte[] GetSigningData()
			{
				List<byte> data =
				[
					(byte)Wallet.Network,
					.. WalletUtils.VLEncode(Wallet.Key!.Length),
					.. Wallet.Key,
					.. WalletUtils.VLEncode(CryptoBox.Length),
					.. CryptoBox,
				];
				return [.. data];
			}
		}

		private readonly List<PayloadBox> payload_boxes = [];
		private byte[]? payload_data;
		private byte[]? payload_iv;
		private byte[]? payload_hash;
		private UInt64 payload_size;
		private bool payload_sparse = false;
		private IPayloadOptions? payload_options;
		private PayloadFlags payload_flags = 0;
		private readonly CryptoModule module;

/*******************************************************************************\
* Payload()																		*
* Private constructor for a payload that requires a CryptoModule so as to		*
* support any cryptographic operations provided by the payload.					*
\*******************************************************************************/

		private Payload(CryptoModule cm) { module = cm; }

/*******************************************************************************\
* Build()																		*
* This method is used for building a payload from a data array, wallets and		*
* payload options. Data is assessed for compressability before being encrypted	*
* with the specified algorithm in the options. The flags are set appropriately	*
* and the hash and size for the data calculated. The symmetric key for the		*
* encrypted data is secured using the CryptoModule provided at payload			*
* construction time. If null is used as the wallets parameter, the data will	*
* not be encrypted and this will be considered a cleartext payload. An empty	*
* wallet array will encrypt the data whilst providing no wallet access. If no	*
* payload options are provided to this method, then a version specific default	*
* set will be applied.															*
\*******************************************************************************/
		private Status Build(byte[]? data, WalletUtils.CryptoKey[]? wallets, IPayloadOptions? options)
		{
			payload_options = options ?? new PayloadOptions();
			(payload_data, bool IsCompressed) = WalletUtils.Compress(data, payload_options.CType);
			if (payload_data == null || payload_data.Length < 1)
				return Status.TX_CRYPTO_FAILURE;
			if (IsCompressed)
				payload_flags |= PayloadFlags.PAYLOAD_COMPRESSION;
			if (payload_options.EType != EncryptionType.None && wallets != null)
			{
				(payload_data, byte[]? Key, payload_iv) = WalletUtils.Encrypt(payload_data, payload_options.EType);
				if (payload_data == null || Key == null || Key.Length != payload_options.EType.GetSymKeySizeAttribute()
					|| payload_iv == null || payload_iv.Length != payload_options.EType.GetIVSizeAttribute())
					return Status.TX_CRYPTO_FAILURE;
				if (wallets.Length > 0)
				{
					for (int i = 0; i < wallets.Length; i++)
					{
						(Status status, byte[]? ct) = module.Encrypt(Key, (byte)wallets[i].Network, wallets[i].Key!);
						if (status != Status.STATUS_OK || ct == null)
							return Status.TX_CRYPTO_FAILURE;
						payload_boxes.Add(new PayloadBox(wallets[i], ct));
					}
					payload_flags |= PayloadFlags.PAYLOAD_ENCRYPTION;
				}
			}
			if (payload_options.PType)
				payload_flags |= PayloadFlags.PAYLOAD_PROTECTED;
			payload_hash = WalletUtils.HashData(payload_data, payload_options.HType);
			payload_size = (UInt64)payload_data.Length;
			return Status.STATUS_OK;
		}

/*******************************************************************************\
* Build()																		*
* This method constructs all of the internal fields of the payload using a		*
* positioned BinaryReader to load each field from the binary data. The reader	*
* must be positioned at the start of the payload block within the transaction.	*
* All of the read field lengths are validated for correctness, including those	*
* types as defined by the payload options. User flags are also initialized, as	*
* well as payload data if the transaction is not declared as SPARSE. The size	*
* field of payload data will be valid even if the transaction is delcared as	*
* SPARSE. Both signed and unsigned transactions may be loaded using this		*
* function call.																*
\*******************************************************************************/
		private Status Build(BinaryReader reader, bool sparse)
		{
			static (IPayloadOptions? Options, PayloadFlags Flags) ReadFlagOps(BinaryReader reader)
			{
				UInt32 opt = reader.ReadUInt32();
				IPayloadOptions po = new PayloadOptions() { UType = (UInt16)opt };
				opt >>= 16;
				if (!Enum.IsDefined(typeof(PayloadTypeField), (UInt16)opt))
					return (null, 0);
				po.TType = (PayloadTypeField)opt;
				opt = reader.ReadUInt32();
				PayloadFlags pf = (PayloadFlags)(opt >> 16);
				UInt16 vt = (UInt16)(opt & 31);
				if (!Enum.IsDefined(typeof(CompressionType), vt))
					return (null, 0);
				po.CType = (CompressionType)vt;
				vt = (UInt16)((opt >> 5) & 31);
				if (!Enum.IsDefined(typeof(EncryptionType), vt))
					return (null, 0);
				po.EType = (EncryptionType)vt;
				vt = (UInt16)((opt >> 10) & 31);
				if (!Enum.IsDefined(typeof(HashType), vt))
					return (null, 0);
				po.HType = (HashType)vt;
				return (po, pf);
			}
			static (UInt32 Count, PayloadBox? Box) ReadWalletBox(BinaryReader reader)
			{
				(UInt32 Size, UInt32 Count) = ((UInt32, UInt32))WalletUtils.ReadVLCount(reader);
				byte network = reader.ReadByte();
				byte[] addr = WalletUtils.ReadVLArray(reader);
				return (Enum.IsDefined(typeof(WalletNetworks), network) && addr.Length == ((WalletNetworks)network).GetPubKeySizeAttribute()) ?
					(Size + Count, new PayloadBox(new WalletUtils.CryptoKey((WalletNetworks)network, addr), WalletUtils.ReadVLArray(reader))) : (0, null);
			}
			payload_sparse = sparse;
			try
			{
				UInt32 tblksz = (UInt32)WalletUtils.ReadVLSize(reader);
				(payload_options, payload_flags) = ReadFlagOps(reader);
				tblksz -= sizeof(PayloadTypeField) + (sizeof(UInt16) << 1) + sizeof(PayloadFlags);
				if (payload_options != null)
				{
					(UInt32 Size, UInt32 Count) = ((UInt32, UInt32))WalletUtils.ReadVLCount(reader);
					tblksz -= Count;
					(UInt32 Size, UInt32 Count) rblksz = (0, 0);
					if (Size > 0)
					{
						rblksz = ((UInt32, UInt32))WalletUtils.ReadVLCount(reader);
						tblksz -= rblksz.Count + rblksz.Size;
						for (uint i = 0; i < rblksz.Size; i++)
						{
							(UInt32 boxsz, PayloadBox? box) = ReadWalletBox(reader);
							rblksz.Size -= boxsz;
							if (box != null)
								payload_boxes.Add(box);
						}
					}
					(payload_hash, Count) = WalletUtils.ReadVLArrayCount(reader);
					if (payload_options.HType.GetHashSizeAttribute() == payload_hash.Length)
					{
						tblksz -= Count + (UInt32)payload_hash.Length;
						(payload_size, Count) = ((UInt64, UInt32))WalletUtils.ReadVLCount(reader);
						tblksz -= Count;
						if ((payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
							payload_iv = reader.ReadBytes(payload_options.EType.GetIVSizeAttribute());
						if (!payload_sparse)
							payload_data = reader.ReadBytes((int)payload_size);
						else
							reader.BaseStream.Position += (long)payload_size;
						if (((rblksz.Size | tblksz) == 0) && (payload_sparse || VerifyHash()))
							return Status.STATUS_OK;
					}
				}
			}
			catch (Exception) {}
			return Status.TX_CORRUPT_PAYLOAD;
		}

/*******************************************************************************\
* GetData()																		*
* This method returns the payload data to the caller. If the data is encrypted	*
* then a private key must be supplied. Both the private key	and the derived		*
* public key are checked for correctness by this method. The public key must	*
* match an entry in the payload cryptoboxes in order to decrypt the payload		*
* contents. If the payload data is compressed, then this method will decompress	*
* the data before returning it to the caller. Unencrypted payloads will also be	*
* uncompressed and have a return status of TX_NOT_ENCRYPTED.					*
\*******************************************************************************/
		private (Status status, byte[]? data) GetData(string? privkey = null)
		{
			if (DataEncrypted())
			{
				if (privkey != null && privkey.Length > 1 && payload_boxes.Count > 0)
				{
					(Status status, WalletUtils.KeySet? set) = WIFToKeySet(module, privkey);
					if (status != Status.STATUS_OK || set == null)
						return (status, null);
					Int32 index = HasWallet(set.Value.PublicKey, payload_boxes);
					if (index >= 0)
					{
						(status, byte[]? data) = DecryptSymKey(payload_boxes[index].GetCryptoBox(), set.Value.PrivateKey, module);
						if (status != Status.STATUS_OK || data == null || data.Length != payload_options?.EType.GetSymKeySizeAttribute())
							return (status, null);
						(status, data) = DecryptData(payload_data, data, payload_iv, payload_options);
						if (status != Status.STATUS_OK)
							return (status, null);
						if ((payload_flags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION)
							data = WalletUtils.Decompress(data);
						return data != null && data.Length > 0 ? (status, data) : (Status.TX_CRYPTO_FAILURE, null);
					}
				}
				return (Status.TX_ACCESS_DENIED, null);
			}
			if ((payload_flags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION)
			{
				byte[]? data = WalletUtils.Decompress(payload_data);
				return data != null && data.Length > 0 ? (Status.TX_NOT_ENCRYPTED, data) : (Status.TX_CRYPTO_FAILURE, null);
			}
			return (Status.TX_NOT_ENCRYPTED, payload_data);
		}

/*******************************************************************************\
* AddWallets()																	*
* Method used to add a single or multiple wallets to a payload. The	wallets		*
* passed to this method are not validated for correctness. They should be both	*
* correctly formatted, de-duplicated and provided as an array of WalletSets.	*
* The wallets argument must not be null. but may be empty. If the payload data	*
* is unencrypted, then it will be encrypted by this function and appropriate	*
* cryptoboxes created. The flags, size, hash and IV will be	recomputed if the	*
* payload data becomes encrypted by this method. The algorithm used to secure	*
* the payload can be supplied as an optional argument, or the default will be	*
* used. Specifying an EncryptionType of none for an	unencrypted payload will	*
* result in the default encryption algorithm being used. Cryptoboxes will be	*
* created for only those supplied wallets that do not already have a cryptobox	*
* in the payload. Upon completion, an array of boolean values are returned		*
* indicating which supplied	wallets were added to the payload.					*
\*******************************************************************************/
		private (Status status, bool[] added) AddWallets(string? privkey, WalletUtils.CryptoKey[] wallets, EncryptionType EType)
		{
			bool[] added = new bool[wallets.Length];
			if ((payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
			{
				(Status status, WalletUtils.KeySet? set) = WIFToKeySet(module, privkey);
				if (status != Status.STATUS_OK || set == null)
					return (status, added);
				Int32 index = HasWallet(set.Value.PublicKey, payload_boxes);
				if (index < 0)
					return (Status.TX_ACCESS_DENIED, added);
				(status, byte[]? data) = DecryptSymKey(payload_boxes[index].GetCryptoBox(), set.Value.PrivateKey, module);
				if (status != Status.STATUS_OK || data == null || data.Length < 1)
					return (status, added);
				for (int i = 0; i < wallets.Length; i++)
				{
					if (HasWallet(wallets[i], payload_boxes) < 0)
					{
						(status, byte[]? ct) = module.Encrypt(data, (byte)wallets[i].Network, wallets[i].Key!);
						if (status == Status.STATUS_OK && ct != null && ct.Length > 0)
						{
							payload_boxes.Add(new PayloadBox(wallets[i], ct));
							added[i] = true;
						}
					}
				}
			}
			else
			{
				payload_options!.EType = (EType == EncryptionType.None) ? EncryptionType.XCHACHA20_POLY1305 : EType;
				(payload_data, byte[]? Key, payload_iv) = WalletUtils.Encrypt(payload_data, payload_options!.EType);
				if (payload_data == null || Key == null || Key.Length != payload_options.EType.GetSymKeySizeAttribute()
					|| payload_iv == null || payload_iv.Length != payload_options.EType.GetIVSizeAttribute())
					return (Status.TX_CRYPTO_FAILURE, added);
				for (int i = 0; i < wallets.Length; i++)
				{
					(Status result, byte[]? ct) = module.Encrypt(Key, (byte)wallets[i].Network, wallets[i].Key!);
					if (result == Status.STATUS_OK && ct != null && ct.Length > 0)
					{
						payload_boxes.Add(new PayloadBox(wallets[i], ct));
						added[i] = true;
					}
				}
				payload_hash = WalletUtils.HashData(payload_data, payload_options!.HType);
				payload_size = (UInt64)payload_data.Length;
				payload_flags |= PayloadFlags.PAYLOAD_ENCRYPTION;
			}
			return (Status.STATUS_OK, added);
		}

/*******************************************************************************\
* RemoveWallets()																*
* Method used to remove a single or multiple wallets from a payload. The		*
* wallets passed to this method are not validated in any way. They should be	*
* both correctly formatted, de-duplicated and provided as an array of			*
* WalletSets. The wallets argument must not be null, but may be empty. This		*
* method always succeeds and returns an array of boolean values with each value	*
* identifying the wallet that was removed from the payload.						*
\*******************************************************************************/
		private (Status status, bool[]) RemoveWallets(WalletUtils.CryptoKey[] wallets)
		{
			bool[] removed = new bool[wallets.Length];
			if (payload_boxes.Count > 0)
			{
				for (int i = 0; i < wallets.Length; i++)
				{
					int index = HasWallet(wallets[i], payload_boxes);
					if (index >= 0)
					{
						payload_boxes.RemoveAt(index);
						removed[i] = true;
					}
				}
			}
			return (Status.STATUS_OK, removed);
		}

/*******************************************************************************\
* ReleaseData()																	*
* Method used to remove the encryption and compression from a payload. By		*
* default compression is removed when releasing a payload, but this behaviour	*
* may be disabled by passing an appropriate argument. Both the private and		*
* public keys are evaulated for correctness when using this method. The public	*
* key of the specified private one, must match one already in the payload for	*
* this function to succeed. If a matching public key entry cannot be found,		*
* then this method will return TX_ACCESS_DENIED. Following successful release	*
* of the payload, the hash, size and flags for the payload will be recomputed.	*
\*******************************************************************************/
		private (Status status, bool released) ReleaseData(string privkey, bool uncompress)
		{
			if ((payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) != PayloadFlags.PAYLOAD_ENCRYPTION)
				return (Status.TX_NOT_ENCRYPTED, false);
			(Status status, WalletUtils.KeySet? set) = WIFToKeySet(module, privkey);
			if (status != Status.STATUS_OK || set == null)
				return (status, false);
			Int32 index = HasWallet(set.Value.PublicKey, payload_boxes);
			if (index < 0)
				return (Status.TX_ACCESS_DENIED, false);
			(status, byte[]? data) = DecryptSymKey(payload_boxes[index].GetCryptoBox(), set.Value.PrivateKey, module);
			if (status != Status.STATUS_OK || data == null || data.Length < 1)
				return (status, false);
			(status, data) = DecryptData(payload_data, data, payload_iv, payload_options!);
			if (status != Status.STATUS_OK)
				return (Status.TX_CRYPTO_FAILURE, false);
			if (uncompress)
			{
				data = WalletUtils.Decompress(data);
				if (data == null || data.Length < 1)
					return (Status.TX_CRYPTO_FAILURE, false);
				payload_flags &= ~PayloadFlags.PAYLOAD_COMPRESSION;
			}
			payload_data = data;
			payload_hash = WalletUtils.HashData(payload_data, payload_options!.HType);
			payload_size = (UInt64)payload_data!.Length;
			payload_flags &= ~PayloadFlags.PAYLOAD_ENCRYPTION;
			payload_boxes.Clear();
			payload_iv = null;
			return (Status.STATUS_OK, true);
		}

/*******************************************************************************\
* ReplaceData()																	*
* This method is used to replace the data of a payload whilst maintaining the	*
* wallet access to the data. If the replacing data is identified as				*
* compressible, it will be compressed as defined by the payload compression		*
* options. The payload flags for compression maybe added or removed depending	*
* depending upon this determination. The data will also be encrypted as defined	*
* by the payload encryption	type options. Both the hash for the data and		*
* payload size will be recalculated upon successful replacement. All payload	*
* cryptoboxes for wallet access will be replaced by this method.				*
\*******************************************************************************/
		private (Status status, bool replaced) ReplaceData(byte[] data)
		{
			if ((payload_flags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION)
			{
				(payload_data, bool IsCompressed) = WalletUtils.Compress(data, payload_options!.CType);
				if (payload_data == null || payload_data.Length < 1)
					return (Status.TX_CRYPTO_FAILURE, false);
				if (!IsCompressed)
					payload_flags &= PayloadFlags.PAYLOAD_COMPRESSION;
			}
			if ((payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
			{
				(payload_data, byte[]? Key, payload_iv) = WalletUtils.Encrypt(payload_data, payload_options!.EType);
				if (payload_data == null || Key == null || Key.Length != payload_options.EType.GetSymKeySizeAttribute()
					|| payload_iv == null || payload_iv.Length != payload_options.EType.GetIVSizeAttribute())
					return (Status.TX_CRYPTO_FAILURE, false);
				for (int i = 0; i < payload_boxes.Count; i++)
				{
					(Status status, byte[]? ct) = module.Encrypt(Key, (byte)payload_boxes[i].GetNetwork(), payload_boxes[i].GetWallet());
					if (status != Status.STATUS_OK || ct == null || ct.Length < 1)
						return (status, false);
					payload_boxes[i] = new PayloadBox(new WalletUtils.CryptoKey(payload_boxes[i].GetNetwork(), payload_boxes[i].GetWallet()), ct);
				}
			}
			payload_hash = WalletUtils.HashData(payload_data, payload_options!.HType);
			payload_size = (UInt64)payload_data!.Length;
			return (Status.STATUS_OK, true);
		}

/*******************************************************************************\
* SigningData()																	*
* This method is used to serialize all of the internal structures of the		*
* payload into an array that can be hashed for signing. The signing data		*
* includes all payload fields, flags and options including the payload IV, hash	*
* and size. Variable length field sizes are also included in this signing data.	*
* Payload data is not included in the serialized signing data for the payload,	*
* but the payload IV is prepended to the payload data field.					*
\*******************************************************************************/
		private List<byte> SigningData()
		{
			List<byte> data = [.. BitConverter.GetBytes((((UInt32)payload_options!.TType) << 16) | payload_options!.UType)];
			UInt32 fo = (((UInt32)payload_flags) << 16) | ((((UInt32)payload_options.HType) & 31) << 10) |
				((((UInt32)payload_options.EType) & 31) << 5) | (((UInt32)payload_options.CType) & 31);
			data.AddRange(BitConverter.GetBytes(fo));
			data.AddRange(WalletUtils.VLEncode(payload_boxes.Count));
			if (payload_boxes.Count > 0)
			{
				List<byte> boxes = [];
				for (int i = 0; i < payload_boxes.Count; i++)
				{
					byte[] box = payload_boxes[i].GetSigningData();
					boxes.AddRange(WalletUtils.VLEncode(box.Length));
					boxes.AddRange(box);
				}
				data.AddRange(WalletUtils.VLEncode(boxes.Count));
				data.AddRange(boxes);
			}
			data.AddRange(WalletUtils.VLEncode(payload_hash!.Length));
			data.AddRange(payload_hash);
			data.AddRange(WalletUtils.VLEncode(payload_size));
			data.InsertRange(0, WalletUtils.VLEncode(data.Count));
			if ((payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
			{
				data.AddRange(payload_iv != null && payload_iv.Length == payload_options.EType.GetIVSizeAttribute() ?
					payload_iv : new byte[payload_options.EType.GetIVSizeAttribute()]);
			}
			return data;
		}

/*******************************************************************************\
* HashingData()																	*
* This method is used to serialize all of the internal structures of the		*
* payload, including the payload data. It is functionally identical to the		*
* SigningData() method except for the inclusion of this data.					*
\*******************************************************************************/
		private List<byte> HashingData()
		{
			List<byte> data = SigningData();
			data.AddRange(payload_data!);
			return data;
		}

/*******************************************************************************\
* WIFToKeySet()																	*
* This method takes a WIF encoded private key and produces a raw private and	*
* public key. The keys are verified for integrity and checked that the network	*
* of the key is supported by the platform. The CryptoModule is used to perform	*
* the private to public key arithmetic. The network for the keys is also made	*
* available to the caller.														*
\*******************************************************************************/
		private static (Status status, WalletUtils.KeySet? set) WIFToKeySet(CryptoModule module, string? privkey)
		{
			(byte Network, byte[] PrivateKey)? kd = WalletUtils.WIFToPrivKey(privkey);
			if (kd == null || kd.Value.PrivateKey == null || kd.Value.PrivateKey.Length < 1)
				return (Status.TX_INVALID_KEY, null);
			if (!Enum.IsDefined(typeof(WalletNetworks), kd.Value.Network))
				return (Status.TX_INVALID_KEY, null);
			(Status status, byte[]? pubkey) = module.CalculatePublicKey(kd.Value.Network, kd.Value.PrivateKey);
			if (status != Status.STATUS_OK || pubkey == null || pubkey.Length < 1)
				return (status, null);
			return (Status.STATUS_OK, new WalletUtils.KeySet()
			{
				PrivateKey = new WalletUtils.CryptoKey((WalletNetworks)kd.Value.Network, kd.Value.PrivateKey),
				PublicKey = new WalletUtils.CryptoKey((WalletNetworks)kd.Value.Network, pubkey)
			});
		}

/*******************************************************************************\
* HasWallet()																	*
* This internal method finds the specified CryptoKey within the payload and		*
* returns its index. If the CryptoKey cannot be found within the payload then a	*
* value	of -1 is returned. The CryptoKey should be correctly formatted and must	*
* not be null.																	*
\*******************************************************************************/
		private static Int32 HasWallet(WalletUtils.CryptoKey key, List<PayloadBox> boxes)
		{
			for (int i = 0; i < boxes.Count; i++)
			{
				if (new ReadOnlySpan<byte>(key.Key).SequenceEqual(boxes[i].GetWallet()) && key.Network == boxes[i].GetNetwork())
					return i;
			}
			return -1;
		}

/*******************************************************************************\
* DecryptSymKey()																*
* This internal method is used to decrypt the payload data using the supplied	*
* symmetric key. The data will be decrypted using the algorithm specified by	*
* payload options. If decyption fails, then a crypto failure error will be		*
* returned.																		*
\*******************************************************************************/
		private static (Status status, byte[]? data) DecryptSymKey(byte[] data, WalletUtils.CryptoKey key, CryptoModule module)
		{
			try { return (module.AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL) ?
					module.Decrypt(data, (byte)key.Network, key.Key!) : module.Decrypt(data, 0, null!); }
			catch (Exception) {}
			return (Status.TX_BAD_CRYPTOBOX, null);
		}

/*******************************************************************************\
* DecryptData()																	*
* This internal method is used to decrypt the payload data using the supplied	*
* symmetric key. The data will be decrypted using the algorithm specified by	*
* payload options. If decyption fails, then a crypto failure error will be		*
* returned.																		*
\*******************************************************************************/
		private static (Status status, byte[]? data) DecryptData(byte[]? pdata, byte[] symkey, byte[]? iv, IPayloadOptions options)
		{
			byte[]? data = WalletUtils.Decrypt(pdata, symkey, iv, options.EType);
			return (data != null && data.Length > 0 ? Status.STATUS_OK : Status.TX_CRYPTO_FAILURE, data);
		}

/** Publically Accessible Functions **/
		public IPayloadInfo? GetInfo()
		{
			static string[] PayloadWallets(List<PayloadBox> boxes, PayloadFlags flags)
			{
				string[] wallets = [];
				if ((flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION && boxes.Count > 0)
				{
					wallets = new string[boxes.Count];
					for (int i = 0; i < boxes.Count; i++)
						wallets[i] = WalletUtils.PubKeyToWallet(boxes[i].GetWallet(), (byte)boxes[i].GetNetwork())!;
				}
				return wallets;
			}
			ConstructorInfo? ci = typeof(PayloadInfo).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(UInt64), typeof(byte[]), typeof(string[]), typeof(UInt32), typeof(UInt32), typeof(bool)], null);
			return (ci == null) ? null : ((PayloadInfo)ci.Invoke([ DataSize(), DataHash(), PayloadWallets(payload_boxes, payload_flags),
					(((UInt32)payload_flags) << 16) | ((((UInt32)payload_options!.HType) & 31) << 10) |
					((((UInt32)payload_options.EType) & 31) << 5) | (((UInt32)payload_options.CType) & 31),
					(((UInt32)payload_options!.TType) << 16) | payload_options!.UType, payload_sparse ]));
		}
		public byte[] DataHash() { return (payload_hash == null || payload_hash.Length < 1) ? [] : payload_hash; }
		public UInt64 DataSize() { return (UInt64)payload_size; }
		public bool VerifyHash()
		{
			return DataHash().Length > 0 && (DataSparse() ||
				((ReadOnlySpan<byte>)WalletUtils.HashData(payload_data, payload_options!.HType)).SequenceEqual(DataHash()));
		}
		public bool DataSparse() { return payload_sparse; }
		public bool DataProtected() { return (payload_flags & PayloadFlags.PAYLOAD_PROTECTED) == PayloadFlags.PAYLOAD_PROTECTED; }
		public bool DataCompressed() { return (payload_flags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION; }
		public bool DataEncrypted() { return (payload_flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION; }
	}
}
