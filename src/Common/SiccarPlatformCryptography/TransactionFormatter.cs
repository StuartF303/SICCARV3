// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

// Transaction Formatter Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public class TransactionFormatter
	{

/*******************************************************************************\
* ToJSON()																		*
* Method to convert a Transaction object into JSON. If the transaction is not	*
* valid then an empty JSON object will be returned. This method may be used on	*
* signed or unsigned transactions.												*
\*******************************************************************************/
		public static string ToJSON(Transaction? transaction)
		{
			if (transaction != null && transaction.Data != null)
			{
				BinaryReader reader = new(new MemoryStream(transaction.Data));
				UInt32 ver = BitConverter.ToUInt32(transaction.Data, 0);
				if (ver == 1)
					return FormatV1JSON(reader, transaction.TxId);
				else if (ver == 2)
					return FormatV2JSON(reader, transaction.TxId);
				else if ((WalletUtils.UInt32Exchange(ver) & ~(UInt32)TransportIdentifier.Transaction) == 3)
					return FormatV3JSON(reader, transaction.TxId);
				else if ((WalletUtils.UInt32Exchange(ver) & ~(UInt32)TransportIdentifier.Transaction) == 4)
					return FormatV4JSON(reader, transaction.TxId);
			}
			return new string("{}");
		}

/*******************************************************************************\
* ToModel()																		*
* Use this method to convert a Transaction object into a TransactionModel. This	*
* method should be used sparingly as the TransactionModel class needs to be		*
* phased out in favour of a Transaction object.									*
\*******************************************************************************/
		public static TransactionModel? ToModel(Transaction? transaction)
		{
			if (transaction != null && transaction.Data != null)
			{
				try
				{
					BinaryReader reader = new(new MemoryStream(transaction.Data));
					UInt32 ver = BitConverter.ToUInt32(transaction.Data, 0);
					if (ver == 1)
						return FormatV1Model(reader, transaction.TxId);
					else if (ver == 2)
						return FormatV2Model(reader, transaction.TxId);
					else if ((WalletUtils.UInt32Exchange(ver) & ~(UInt32)TransportIdentifier.Transaction) == 3)
						return FormatV3Model(reader, transaction.TxId);
					else if ((WalletUtils.UInt32Exchange(ver) & ~(UInt32)TransportIdentifier.Transaction) == 4)
						return FormatV4Model(reader, transaction.TxId);
				}
				catch (Exception) {}
			}
			return null;
		}

/*******************************************************************************\
* ToTransaction()																*
* This method converts a TransactionModel into a Transaction object for			*
* transaction control. It can be used on both a signed and unsigned				*
* transactions. Care must be taken when using this function as deserialization	*
* of MetaData, even if empty, leaves artifacts in the transaction. This method	*
* should be used sparingly as the TransactionModel class needs to be phased out	*
* in favour of a Transaction object.											*
\*******************************************************************************/
		public static Transaction? ToTransaction(TransactionModel? model)
		{
			if (model != null)
			{
				UInt32 ver = model.Version;
				if (ver == 1)
					return FormatV1TxModel(model);
				else if (ver == 2)
					return FormatV2TxModel(model);
				else if (ver == 3)
					return FormatV3TxModel(model);
				else if (ver == 4)
					return FormatV4TxModel(model);
			}
			return null;
		}

/*******************************************************************************\
* ToTransaction()																*
* This method converts a Transaction JSON string into a Transaction object		*
* for transaction control. Usable for both signed and unsigned transactions.	*
\*******************************************************************************/
		public static Transaction? ToTransaction(string? json)
		{
			if (json != null)
			{
				try
				{
					JsonDocument document = JsonDocument.Parse(json);
					UInt32 ver = document.RootElement.GetProperty(JSONKeys.Version).GetUInt32();
					if (ver == 1)
						return FormatV1TxJSON(document);
					else if (ver == 2)
						return FormatV2TxJSON(document);
					else if (ver == 3)
						return FormatV3TxJSON(document);
					else if (ver == 4)
						return FormatV4TxJSON(document);
				}
				catch (Exception) {}
			}
			return null;
		}

/*******************************************************************************\
* ToJSONLayout()																*
* Use this method to convert a Transaction into a JSON representation of the	*
* binary transaction object. This function can be used for both signed and		*
* unsigned transactions.														*
\*******************************************************************************/
		public static string ToJSONLayout(Transaction? transaction)
		{
			StringBuilder builder = new("{");
			return (transaction == null || transaction.Data == null || transaction.Data.Length < 1) ? builder.Append('}').ToString() :
				builder.Append(JSONKeys.QTxId).Append(transaction.TxId != null && transaction.TxId.Length > 0 ? ItemValueWrap(transaction.TxId) : "null,")
				.Append(JSONKeys.QRegister).Append(transaction.RegisterId != null && transaction.RegisterId.Length > 0 ? ItemValueWrap(transaction.RegisterId) : "null,")
				.Append(JSONKeys.QData).Append('\"').Append(Convert.ToBase64String(transaction.Data)).Append("\"}").ToString();
		}

/** Transaction to Model methods **/
		private static TransactionModel? FormatV1Model(BinaryReader reader, string? id)
		{
			TransactionModel tx = new() { Version = reader.ReadUInt32() };
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
			{
				List<string> recipients = [];
				for (uint i = 0; i < count; i++)
					recipients.Add(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!);
				tx.RecipientsWallets = recipients;
			}
			UInt64 ts = reader.ReadUInt64();
			if (id != null)
			{
				tx.TxId = id;
				tx.TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime;
				tx.Signature = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))!;
				tx.SenderWallet = WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!;
			}
			tx.PayloadCount = (UInt64)WalletUtils.ReadVLSize(reader);
			if (tx.PayloadCount > 0)
			{
				tx.Payloads = new PayloadModel[tx.PayloadCount];
				for (uint i = 0; i < tx.PayloadCount; i++)
				{
					tx.Payloads[i] = new PayloadModel();
					count = (UInt32)WalletUtils.ReadVLSize(reader);
					UInt16 flags = 0;
					if (count > 0)
					{
						tx.Payloads[i].Challenges = new Challenge[count];
						List<string> recipients = [];
						for (uint j = 0; j < count; j++)
						{
							recipients.Add(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!);
							Challenge c = new() { size = (UInt64)WalletUtils.ReadVLSize(reader) };
							c.hex = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)c.size));
							tx.Payloads[i].Challenges[j] = c;
						}
						tx.Payloads[i].WalletAccess = [.. recipients];
						Challenge iv = new() { size = (UInt64)WalletUtils.ReadVLSize(reader) };
						iv.hex = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)iv.size));
						tx.Payloads[i].IV = iv;
						flags |= (UInt16)PayloadFlags.PAYLOAD_ENCRYPTION;
					}
					tx.Payloads[i].PayloadSize = (UInt64)WalletUtils.ReadVLSize(reader);
					tx.Payloads[i].Hash = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)TxDefs.SHA256HashSize));
					tx.Payloads[i].Data = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)tx.Payloads[i].PayloadSize));
					tx.Payloads[i].PayloadFlags = string.Format("0x{0:x4}", flags);
				}
				return tx;
			}
			return null;
		}
		private static TransactionModel? FormatV2Model(BinaryReader reader, string? id)
		{
			TransactionModel tx = new() { Version = reader.ReadUInt32() };
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
			{
				List<string> recipients = [];
				for (uint i = 0; i < count; i++)
					recipients.Add(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!);
				tx.RecipientsWallets = recipients;
			}
			UInt64 ts = reader.ReadUInt64();
			if (id != null)
			{
				tx.TxId = id;
				tx.TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime;
				tx.Signature = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))!;
				tx.SenderWallet = WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!;
			}
			tx.PayloadCount = (UInt64)WalletUtils.ReadVLSize(reader);
			if (tx.PayloadCount > 0)
			{
				tx.Payloads = new PayloadModel[tx.PayloadCount];
				for (uint i = 0; i < tx.PayloadCount; i++)
				{
					tx.Payloads[i] = new PayloadModel { PayloadFlags = string.Format("0x{0:x4}", reader.ReadUInt16()) };
					count = (UInt32)WalletUtils.ReadVLSize(reader);
					if (count > 0)
					{
						tx.Payloads[i].Challenges = new Challenge[count];
						List<string> recipients = [];
						for (uint j = 0; j < count; j++)
						{
							recipients.Add(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)!);
							Challenge c = new() { size = (UInt64)WalletUtils.ReadVLSize(reader) };
							c.hex = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)c.size));
							tx.Payloads[i].Challenges[j] = c;
						}
						tx.Payloads[i].WalletAccess = [.. recipients];
						Challenge iv = new() { size = (UInt64)WalletUtils.ReadVLSize(reader) };
						iv.hex = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)iv.size));
						tx.Payloads[i].IV = iv;
					}
					tx.Payloads[i].PayloadSize = (UInt64)WalletUtils.ReadVLSize(reader);
					tx.Payloads[i].Hash = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)TxDefs.SHA256HashSize));
					tx.Payloads[i].Data = WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)tx.Payloads[i].PayloadSize));
				}
				return tx;
			}
			return null;
		}
		private static TransactionModel? FormatV3Model(BinaryReader reader, string? id)
		{
			TransactionModel tx = new()
			{
				Version = WalletUtils.UInt32Exchange(reader.ReadUInt32()) & ~(UInt32)TransportIdentifier.Transaction,
				PrevTxId = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))!
			};
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
			{
				WalletUtils.ReadVLSize(reader);
				List<string> recipients = [];
				for (uint i = 0; i < count; i++)
					recipients.Add(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)!);
				tx.RecipientsWallets = [.. recipients];
			}
			UInt64 ts = reader.ReadUInt64();
			try
			{
				string? meta = Encoding.ASCII.GetString(WalletUtils.ReadVLArray(reader));
				if (meta != null && meta.Length > 0)
					tx.MetaData = JsonSerializer.Deserialize<TransactionMetaData>(meta);
			}
			catch { return null; }
			if (id != null)
			{
				WalletUtils.ReadVLSize(reader);
				tx.TxId = id;
				tx.TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime;
				tx.Signature = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))!;
				tx.SenderWallet = WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)!;
			}
			tx.PayloadCount = (UInt64)WalletUtils.ReadVLSize(reader);
			if (tx.PayloadCount < 1)
				return null;
			tx.Payloads = new PayloadModel[tx.PayloadCount];
			for (uint i = 0; i < tx.PayloadCount; i++)
			{
				WalletUtils.ReadVLSize(reader);
				tx.Payloads[i] = new PayloadModel { PayloadFlags = string.Format("0x{0:x4}", reader.ReadUInt16()) };
				count = (UInt32)WalletUtils.ReadVLSize(reader);
				if (count > 0)
				{
					WalletUtils.ReadVLSize(reader);
					tx.Payloads[i].Challenges = new Challenge[count];
					tx.Payloads[i].WalletAccess = new string[count];
					for (uint j = 0; j < count; j++)
					{
						WalletUtils.ReadVLSize(reader);
						tx.Payloads[i].WalletAccess[j] = WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519);
						byte[] ch = WalletUtils.ReadVLArray(reader);
						tx.Payloads[i].Challenges[j] = new Challenge { size = (ulong)ch.Length, hex = WalletUtils.ByteArrayToHexString(ch) };
					}
					byte[] iv = WalletUtils.ReadVLArray(reader);
					tx.Payloads[i].IV = new Challenge { size = (ulong)iv.Length, hex = WalletUtils.ByteArrayToHexString(iv) };
				}
				tx.Payloads[i].Hash = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader));
				tx.Payloads[i].PayloadSize = (UInt64)WalletUtils.ReadVLSize(reader);
				tx.Payloads[i].Data = Convert.ToBase64String(reader.ReadBytes((int)tx.Payloads[i].PayloadSize));
			}
			return tx;
		}
		private static TransactionModel? FormatV4Model(BinaryReader reader, string? id)
		{
			static string ReadWallet(BinaryReader reader)
			{
				byte network = WalletUtils.SkipVLSize(reader).ReadByte();
				return WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), network)!;
			}

			TransactionModel tx = new()
			{
				Version = WalletUtils.UInt32Exchange(reader.ReadUInt32()) & ~(UInt32)TransportIdentifier.Transaction,
				PrevTxId = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(WalletUtils.SkipVLSize(reader)))!
			};
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
			{
				WalletUtils.SkipVLSize(reader);
				List<string> recipients = [];
				for (uint i = 0; i < count; i++)
					recipients.Add(ReadWallet(reader));
				tx.RecipientsWallets = [.. recipients];
			}
			UInt64 ts = reader.ReadUInt64();
			try
			{
				string? meta = Encoding.ASCII.GetString(WalletUtils.ReadVLArray(reader));
				if (meta != null && meta.Length > 0)
					tx.MetaData = JsonSerializer.Deserialize<TransactionMetaData>(meta);
			}
			catch { return null; }
			if (id != null)
			{
				WalletUtils.ReadVLSize(reader);
				tx.TxId = id;
				tx.TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime;
				tx.Signature = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))!;
				tx.SenderWallet = ReadWallet(reader);
			}
			else if (ts > 0)
				return null;
			tx.PayloadCount = (UInt64)WalletUtils.ReadVLSize(reader);
			if (tx.PayloadCount < 1)
				return null;
			tx.Payloads = new PayloadModel[tx.PayloadCount];
			for (uint i = 0; i < tx.PayloadCount; i++)
			{
				tx.Payloads[i] = new PayloadModel { PayloadFlags = string.Format("0x{0:x16}", WalletUtils.SkipVLSize(reader).ReadUInt64()) };
				count = (UInt32)WalletUtils.ReadVLSize(reader);
				if (count > 0)
				{
					WalletUtils.ReadVLSize(reader);
					tx.Payloads[i].Challenges = new Challenge[count];
					tx.Payloads[i].WalletAccess = new string[count];
					for (uint j = 0; j < count; j++)
					{
						tx.Payloads[i].WalletAccess[j] = ReadWallet(reader);
						byte[] ch = WalletUtils.ReadVLArray(reader);
						tx.Payloads[i].Challenges[j] = new Challenge { size = (ulong)ch.Length, hex = WalletUtils.ByteArrayToHexString(ch) };
					}
				}
				tx.Payloads[i].Hash = WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader));
				tx.Payloads[i].PayloadSize = (UInt64)WalletUtils.ReadVLSize(reader);
				UInt32 flags = (UInt32)(Convert.ToUInt64(tx.Payloads[i].PayloadFlags, 16) >> 32);
				if (((PayloadFlags)(flags >> 16) & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
				{
					UInt16 type = (UInt16)((flags >> 5) & 31);
					if (!Enum.IsDefined((EncryptionType)type))
						return null;
					byte[] iv = reader.ReadBytes(((EncryptionType)type).GetIVSizeAttribute());
					tx.Payloads[i].IV = new Challenge { size = (ulong)iv.Length, hex = WalletUtils.ByteArrayToHexString(iv) };
				}
				tx.Payloads[i].Data = Convert.ToBase64String(reader.ReadBytes((int)tx.Payloads[i].PayloadSize));
			}
			return tx;
		}

/** Transaction to JSON methods **/
		private static string FormatV1JSON(BinaryReader reader, string? id)
		{
			StringBuilder builder = new();
			builder.Append('{').Append(JSONKeys.QTxId).Append(id != null ? ItemValueWrap(id) : "null,")
				.Append(JSONKeys.QVersion).Append(reader.ReadUInt32()).Append(',');
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			StringBuilder recipients = new();
			recipients.Append(JSONKeys.QRecipients).Append('[');
			if (count > 0)
			{
				for (uint i = 0; i < count; i++)
					recipients.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)));
				recipients.Remove(recipients.Length - 1, 1);
			}
			recipients.Append("],");
			UInt64 ts = reader.ReadUInt64();
			if (id != null)
			{
				string? sig = ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader)));
				builder.Append(JSONKeys.QSender)
					.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)))
					.Append(recipients).Append(JSONKeys.QTimeStamp).Append(ItemValueWrap(DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime.ToString("O")))
					.Append(JSONKeys.QSignature).Append(sig);
			}
			else
				builder.Append(recipients);
			UInt32 pcount = (UInt32)WalletUtils.ReadVLSize(reader);
			if (pcount > 0)
			{
				builder.Append(JSONKeys.QPayloadCount).Append(pcount).Append(',').Append(JSONKeys.QPayloads).Append('[');
				for (uint i = 0; i < pcount; i++)
				{
					builder.Append('{');
					recipients.Clear().Append(JSONKeys.QAccess).Append('[');
					StringBuilder challenges = new();
					StringBuilder iv = new();
					count = (UInt32)WalletUtils.ReadVLSize(reader);
					if (count > 0)
					{
						challenges.Append(JSONKeys.QChallenges).Append('[');
						for (uint j = 0; j < count; j++)
						{
							recipients.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)));
							UInt32 sz = (UInt32)WalletUtils.ReadVLSize(reader);
							challenges.Append('{').Append(JSONKeys.QSize).Append(sz).Append(',').Append(JSONKeys.QHex)
								.Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)sz)))).Append("},");
						}
						recipients.Remove(recipients.Length - 1, 1);
						challenges.Remove(challenges.Length - 1, 1).Append("],");
						UInt32 ivsz = (UInt32)WalletUtils.ReadVLSize(reader);
						iv.Append(JSONKeys.QIV).Append('{').Append(JSONKeys.QSize).Append(ivsz).Append(',').Append(JSONKeys.QHex)
							.Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)ivsz)))).Append("},");
					}
					recipients.Append("],");
					UInt64 psz = (UInt64)WalletUtils.ReadVLSize(reader);
					builder.Append(recipients).Append(challenges).Append(iv).Append(JSONKeys.QPayloadSize).Append(psz).Append(',')
						.Append(JSONKeys.QEncrypted).Append(count > 0 ? "true" : "false").Append(',').Append(JSONKeys.QHash)
						.Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)TxDefs.SHA256HashSize)))).Append(JSONKeys.QData)
						.Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)psz)))).Append("},");
				}
				builder.Remove(builder.Length - 1, 1).Append("]}");
				return builder.ToString();
			}
			return new string("{}");
		}
		private static string FormatV2JSON(BinaryReader reader, string? id)
		{
			StringBuilder builder = new();
			builder.Append('{').Append(JSONKeys.QTxId).Append(id != null ? ItemValueWrap(id) : "null,")
				.Append(JSONKeys.QVersion).Append(reader.ReadUInt32()).Append(',');
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			StringBuilder recipients = new();
			recipients.Append(JSONKeys.QRecipients).Append('[');
			if (count > 0)
			{
				for (uint i = 0; i < count; i++)
					recipients.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)));
				recipients.Remove(recipients.Length - 1, 1);
			}
			recipients.Append("],");
			UInt64 ts = reader.ReadUInt64();
			if (id != null)
			{
				string? sig = ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader)));
				builder.Append(JSONKeys.QSender)
					.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)))
					.Append(recipients).Append(JSONKeys.QTimeStamp).Append(ItemValueWrap(DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime.ToString("O")))
					.Append(JSONKeys.QSignature).Append(sig);
			}
			else
				builder.Append(recipients);
			UInt32 pcount = (UInt32)WalletUtils.ReadVLSize(reader);
			if (pcount > 0)
			{
				builder.Append(JSONKeys.QPayloadCount).Append(pcount).Append(',').Append(JSONKeys.QPayloads).Append('[');
				for (uint i = 0; i < pcount; i++)
				{
					UInt16 flags = reader.ReadUInt16();
					builder.Append('{');
					recipients.Clear().Append(JSONKeys.QAccess).Append('[');
					StringBuilder challenges = new();
					StringBuilder iv = new();
					count = (UInt32)WalletUtils.ReadVLSize(reader);
					if (count > 0)
					{
						challenges.Append(JSONKeys.QChallenges).Append('[');
						for (uint j = 0; j < count; j++)
						{
							recipients.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(reader.ReadBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()), (byte)WalletNetworks.ED25519)));
							UInt32 sz = (UInt32)WalletUtils.ReadVLSize(reader);
							challenges.Append('{').Append(JSONKeys.QSize).Append(sz).Append(',').Append(JSONKeys.QHex)
								.Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)sz)))).Append("},");
						}
						recipients.Remove(recipients.Length - 1, 1);
						challenges.Remove(challenges.Length - 1, 1).Append("],");
						UInt32 ivsz = (UInt32)WalletUtils.ReadVLSize(reader);
						iv.Append(JSONKeys.QIV).Append('{').Append(JSONKeys.QSize).Append(ivsz).Append(',').Append(JSONKeys.QHex)
							.Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)ivsz)))).Append("},");
					}
					recipients.Append("],");
					UInt64 psz = (UInt64)WalletUtils.ReadVLSize(reader);
					builder.Append(recipients).Append(challenges).Append(iv).Append(JSONKeys.QPayloadSize).Append(psz).Append(',').Append(JSONKeys.QEncrypted)
						.Append(((PayloadFlags)flags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION ? "true" : "false")
						.Append(',').Append(JSONKeys.QHash).Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)TxDefs.SHA256HashSize))))
						.Append(JSONKeys.QData).Append(ValueWrap(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)psz)))).Append("},");
				}
				builder.Remove(builder.Length - 1, 1).Append("]}");
				return builder.ToString();
			}
			return new string("{}");
		}
		private static string FormatV3JSON(BinaryReader reader, string? id)
		{
			StringBuilder builder = new();
			UInt32 ver = WalletUtils.UInt32Exchange(reader.ReadUInt32());
			builder.Append('{').Append(JSONKeys.QType).Append(ItemValueWrap(((TransportIdentifier)ver & TransportIdentifier.Transaction).ToString()))
				.Append(JSONKeys.QTxId).Append(id != null ? ItemValueWrap(id) : "null,")
				.Append(JSONKeys.QPrevId).Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))))
				.Append(JSONKeys.QVersion).Append(ver & 0x00ffffff).Append(',');
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			StringBuilder list = new();
			list.Append(JSONKeys.QRecipients).Append('[');
			if (count > 0)
			{
				WalletUtils.ReadVLSize(reader);
				for (int i = 0; i < count; i++)
					list.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)));
				list.Remove(list.Length - 1, 1);
			}
			list.Append("],");
			UInt64 ts = reader.ReadUInt64();
			string? meta = null, sig = null;
			count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
				meta = Encoding.ASCII.GetString(reader.ReadBytes((int)count));
			if (id != null && id.Length > 0)
			{
				WalletUtils.ReadVLSize(reader);
				sig = ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader)));
				builder.Append(JSONKeys.QSender)
					.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)));
			}
			builder.Append(list);
			if (ts > 0)
			{
				builder.Append(JSONKeys.QTimeStamp).Append(ItemValueWrap(DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime.ToString("O")))
					.Append(JSONKeys.QSignature).Append(sig);
			}
			if (meta != null)
				builder.Append(JSONKeys.QMetaData).Append(meta).Append(',');
			count = (UInt32)WalletUtils.ReadVLSize(reader);
			builder.Append(JSONKeys.QPayloadCount).Append(count).Append(',').Append(JSONKeys.QPayloads).Append('[');
			for (int i = 0; i < count; i++)
			{
				WalletUtils.ReadVLSize(reader);
				builder.Append('{').Append(JSONKeys.QFlags).Append(ItemValueWrap(string.Format("0x{0:x4}", reader.ReadUInt16())));
				UInt32 prc = (UInt32)WalletUtils.ReadVLSize(reader);
				list.Clear().Append(JSONKeys.QAccess).Append('[');
				StringBuilder challenges = new();
				if (prc > 0)
				{
					WalletUtils.ReadVLSize(reader);
					challenges.Append(JSONKeys.QChallenges).Append('[');
					for (int k = 0; k < prc; k++)
					{
						WalletUtils.ReadVLSize(reader);
						list.Append(ItemValueWrap(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), (byte)WalletNetworks.ED25519)));
						UInt32 csz = (UInt32)WalletUtils.ReadVLSize(reader);
						challenges.Append('{').Append(JSONKeys.QSize).Append(csz).Append(',').Append(JSONKeys.QHex).Append('\"')
							.Append(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)csz))).Append("\"},");
					}
					list.Remove(list.Length - 1, 1);
					challenges.Remove(challenges.Length - 1, 1).Append("],").Append(JSONKeys.QIV).Append('{').Append(JSONKeys.QSize);
					prc = (UInt32)WalletUtils.ReadVLSize(reader);
					challenges.Append(prc).Append(',').Append(JSONKeys.QHex).Append('\"').Append(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)prc))).Append("\"},");
				}
				builder.Append(list).Append("],");
				if (prc > 0)
					builder.Append(challenges);
				list.Clear().Append(JSONKeys.QHash).Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader))));
				prc = (UInt32)WalletUtils.ReadVLSize(reader);
				builder.Append(JSONKeys.QPayloadSize).Append(prc).Append(',').Append(list).Append(JSONKeys.QData).Append('\"')
					.Append(Convert.ToBase64String(reader.ReadBytes((int)prc))).Append("\"},");
			}
			return builder.Remove(builder.Length - 1, 1).Append("]}").ToString();
		}
		private static string FormatV4JSON(BinaryReader reader, string? id)
		{
			static string ReadWalletObject(BinaryReader reader)
			{
				StringBuilder sb = new("{");
				byte network = WalletUtils.SkipVLSize(reader).ReadByte();
				return sb.Append(JSONKeys.QWallet).Append(ItemValueWrap(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), network)))
					.Append(JSONKeys.QNetwork).Append(ValueWrap(string.Format("0x{0:x2}", network))).Append("},").ToString();
			}

			StringBuilder builder = new();
			UInt32 ver = WalletUtils.UInt32Exchange(reader.ReadUInt32());
			builder.Append('{').Append(JSONKeys.QType).Append(ItemValueWrap(((TransportIdentifier)ver & TransportIdentifier.Transaction).ToString()))
				.Append(JSONKeys.QTxId).Append(id != null ? ItemValueWrap(id) : "null,")
				.Append(JSONKeys.QPrevId).Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(WalletUtils.SkipVLSize(reader)))))
				.Append(JSONKeys.QVersion).Append(ver & 0x00ffffff).Append(',');
			UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
			StringBuilder list = new();
			list.Append(JSONKeys.QRecipients).Append('[');
			if (count > 0)
			{
				WalletUtils.SkipVLSize(reader);
				for (int i = 0; i < count; i++)
					list.Append(ReadWalletObject(reader));
				list.Remove(list.Length - 1, 1);
			}
			list.Append("],");
			UInt64 ts = reader.ReadUInt64();
			string? meta = null, sig = null;
			count = (UInt32)WalletUtils.ReadVLSize(reader);
			if (count > 0)
				meta = Encoding.ASCII.GetString(reader.ReadBytes((int)count));
			byte sn = 0;
			if (id != null && id.Length > 0)
			{
				sig = ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(WalletUtils.SkipVLSize(reader))));
				sn = WalletUtils.SkipVLSize(reader).ReadByte();
				builder.Append(JSONKeys.QSender).Append('{').Append(JSONKeys.QWallet).Append(ItemValueWrap(WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(reader), sn)))
					.Append(JSONKeys.QNetwork).Append(ValueWrap(string.Format("0x{0:x2}", sn))).Append("},").ToString();
			}
			builder.Append(list);
			if (ts > 0)
			{
				builder.Append(JSONKeys.QTimeStamp).Append(ItemValueWrap(DateTimeOffset.FromUnixTimeSeconds((long)ts).UtcDateTime.ToString("O")))
					.Append(JSONKeys.QSignature).Append('{').Append(JSONKeys.QHex).Append(sig).Append(JSONKeys.QType)
					.Append(ValueWrap(((WalletNetworks)sn).ToString())).Append("},");
			}
			if (meta != null)
				builder.Append(JSONKeys.QMetaData).Append(meta).Append(',');
			count = (UInt32)WalletUtils.ReadVLSize(reader);
			builder.Append(JSONKeys.QPayloadCount).Append(count).Append(',').Append(JSONKeys.QPayloads).Append('[');
			for (int i = 0; i < count; i++)
			{
				UInt32 tf = WalletUtils.SkipVLSize(reader).ReadUInt32();
				UInt32 po = reader.ReadUInt32();
				builder.Append('{').Append(JSONKeys.QTypeField).Append(ItemValueWrap(((PayloadTypeField)(tf >> 16)).ToString()))
					.Append(JSONKeys.QUserField).Append(ItemValueWrap(string.Format("0x{0:x4}", (UInt16)(tf & 65535)))).Append(JSONKeys.QFlags).Append('{');
				bool encrypted = (PayloadFlags)((po >> 16) & (UInt32)PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION;
				builder.Append(JSONKeys.QEncrypted)
					.Append(encrypted ? "true" : "false");
				builder.Append(',').Append(JSONKeys.QCompressed)
					.Append(((PayloadFlags)((po >> 16) & (UInt32)PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION) ? "true" : "false");
				builder.Append(',').Append(JSONKeys.QProtected)
					.Append(((PayloadFlags)((po >> 16) & (UInt32)PayloadFlags.PAYLOAD_PROTECTED) == PayloadFlags.PAYLOAD_PROTECTED) ? "true" : "false").Append("},");
				builder.Append(JSONKeys.QOptions).Append(ItemValueWrap(string.Format("0x{0:x8}", po)));
				UInt32 prc = (UInt32)WalletUtils.ReadVLSize(reader);
				builder.Append(JSONKeys.QAccess).Append('[');
				if (prc > 0)
				{
					WalletUtils.SkipVLSize(reader);
					for (int k = 0; k < prc; k++)
					{
						builder.Append(ReadWalletObject(reader));
						builder.Remove(builder.Length - 2, 1);
						UInt32 csz = (UInt32)WalletUtils.ReadVLSize(reader);
						builder.Append("\"Challenge\":").Append('{').Append(JSONKeys.QSize).Append(csz).Append(',').Append(JSONKeys.QHex).Append('\"')
							.Append(WalletUtils.ByteArrayToHexString(reader.ReadBytes((int)csz))).Append("\"}},");
					}
					builder.Remove(builder.Length - 1, 1);
				}
				list.Clear().Append(JSONKeys.QHash).Append('{').Append(JSONKeys.QHex)
					.Append(ItemValueWrap(WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(reader)))).Append(JSONKeys.QType)
					.Append(ValueWrap(((HashType)((po >> 10) & 31)).ToString())).Append("},");
				prc = (UInt32)WalletUtils.ReadVLSize(reader);
				builder.Append("],").Append(JSONKeys.QPayloadSize).Append(prc).Append(',').Append(list);
				if (encrypted)
				{
					EncryptionType type = (EncryptionType)((po >> 5) & 31);
					builder.Append(JSONKeys.QType).Append(ItemValueWrap(type.ToString()));
					prc += (UInt32)type.GetIVSizeAttribute();
				}
				builder.Append(JSONKeys.QData).Append('\"').Append(Convert.ToBase64String(reader.ReadBytes((int)prc))).Append("\"},");
			}
			return builder.Remove(builder.Length - 1, 1).Append("]}").ToString();
		}

/** TransactionModel to Transaction methods **/
		private static Transaction? FormatV1TxModel(TransactionModel model)
		{
			List<byte> data = [.. BitConverter.GetBytes(model.Version)];
			UInt32 count = (UInt32)(model.RecipientsWallets != null && model.RecipientsWallets.Any() ? model.RecipientsWallets.Count() : 0);
			data.AddRange(WalletUtils.VLEncode(count));
			for (int i = 0; i < count; i++)
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.RecipientsWallets?.ElementAt(i));
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			if (model.TimeStamp == DateTime.MinValue)
				data.AddRange(BitConverter.GetBytes((UInt64)0));
			else if (model.Signature != null && model.Signature.Length > 0 && model.SenderWallet != null && model.SenderWallet.Length > 0)
			{
				data.AddRange(BitConverter.GetBytes((UInt64)(new DateTimeOffset(model.TimeStamp)).ToUnixTimeSeconds()));
				byte[] sig = WalletUtils.HexStringToByteArray(model.Signature)!;
				data.AddRange(WalletUtils.VLEncode(sig.Length));
				data.AddRange(sig);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.SenderWallet);
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			data.AddRange(WalletUtils.VLEncode(model.PayloadCount));
			for (int i = 0; i < (int)model.PayloadCount; i++)
			{
				UInt32 j = (UInt32)(model.Payloads[i].WalletAccess != null && model.Payloads[i].WalletAccess.Length > 0 ? model.Payloads[i].WalletAccess.Length : 0);
				data.AddRange(WalletUtils.VLEncode(j));
				if (j > 0)
				{
					for (int k = 0; k < j; k++)
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.Payloads[i].WalletAccess[k]);
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						data.AddRange(pk.Value.PubKey);
						data.AddRange(WalletUtils.VLEncode(model.Payloads[i].Challenges[k].size));
						data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Challenges[k].hex)!);
					}
					data.AddRange(WalletUtils.VLEncode(model.Payloads[i].IV.size));
					data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].IV.hex)!);
				}
				data.AddRange(WalletUtils.VLEncode(model.Payloads[i].PayloadSize));
				data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Hash)!);
				data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Data)!);
			}
			string? txid = null;
			if (model.TxId != null && model.TxId.Length > 0)
				txid = model.TxId;
			return new Transaction() { TxId = txid, Data = [.. data] };
		}
		private static Transaction? FormatV2TxModel(TransactionModel model)
		{
			List<byte> data = [.. BitConverter.GetBytes(model.Version)];
			UInt32 count = (UInt32)(model.RecipientsWallets != null && model.RecipientsWallets.Any() ? model.RecipientsWallets.Count() : 0);
			data.AddRange(WalletUtils.VLEncode(count));
			for (int i = 0; i < count; i++)
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.RecipientsWallets?.ElementAt(i));
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			if (model.TimeStamp == DateTime.MinValue)
				data.AddRange(BitConverter.GetBytes((UInt64)0));
			else if (model.Signature != null && model.Signature.Length > 0 && model.SenderWallet != null && model.SenderWallet.Length > 0)
			{
				data.AddRange(BitConverter.GetBytes((UInt64)new DateTimeOffset(model.TimeStamp).ToUnixTimeSeconds()));
				byte[] sig = WalletUtils.HexStringToByteArray(model.Signature)!;
				data.AddRange(WalletUtils.VLEncode(sig.Length));
				data.AddRange(sig);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.SenderWallet);
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			data.AddRange(WalletUtils.VLEncode(model.PayloadCount));
			for (int i = 0; i < (int)model.PayloadCount; i++)
			{
				data.AddRange(BitConverter.GetBytes(Convert.ToUInt16(model.Payloads[i].PayloadFlags, 16)));
				UInt32 j = (UInt32)(model.Payloads[i].WalletAccess != null && model.Payloads[i].WalletAccess.Length > 0 ? model.Payloads[i].WalletAccess.Length : 0);
				data.AddRange(WalletUtils.VLEncode(j));
				if (j > 0)
				{
					for (int k = 0; k < j; k++)
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.Payloads[i].WalletAccess[k]);
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						data.AddRange(pk.Value.PubKey);
						data.AddRange(WalletUtils.VLEncode(model.Payloads[i].Challenges[k].size));
						data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Challenges[k].hex)!);
					}
					data.AddRange(WalletUtils.VLEncode(model.Payloads[i].IV.size));
					data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].IV.hex)!);
				}
				data.AddRange(WalletUtils.VLEncode(model.Payloads[i].PayloadSize));
				data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Hash)!);
				data.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Data)!);
			}
			string? txid = null;
			if (model.TxId != null && model.TxId.Length > 0)
				txid = model.TxId;
			return new Transaction() { TxId = txid, Data = [.. data] };
		}
		private static Transaction? FormatV3TxModel(TransactionModel model)
		{
			List<byte> data = [ .. BitConverter.GetBytes(WalletUtils.UInt32Exchange(model.Version | (UInt32)TransportIdentifier.Transaction)) ];
			byte[] hash = WalletUtils.HexStringToByteArray(model.PrevTxId)!;
			data.AddRange(WalletUtils.VLEncode(hash.Length));
			data.AddRange(hash);
			if (model.RecipientsWallets == null || !model.RecipientsWallets.Any())
				data.Add(0);
			else
			{
				data.AddRange(WalletUtils.VLEncode(model.RecipientsWallets.Count()));
				List<byte> append = [];
				for (int i = 0; i < model.RecipientsWallets?.Count(); i++)
				{
					(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.RecipientsWallets?.ElementAt(i));
					if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
						return null;
					append.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
					append.AddRange(pk.Value.PubKey);
				}
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
			}
			data.AddRange(BitConverter.GetBytes((UInt64)(model.TimeStamp == DateTime.MinValue ? 0 : new DateTimeOffset(model.TimeStamp).ToUnixTimeSeconds())));
			if (model.MetaData != null)
			{
				try
				{
					byte[] meta = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(model.MetaData));
					data.AddRange(WalletUtils.VLEncode(meta.Length));
					data.AddRange(meta);
				}
				catch (Exception) { return null; }
			}
			else
				data.Add(0);
			if (model.Signature != null && model.Signature.Length > 0 && model.SenderWallet != null && model.SenderWallet.Length > 0)
			{
				List<byte> append = [];
				byte[] sig = WalletUtils.HexStringToByteArray(model.Signature)!;
				append.AddRange(WalletUtils.VLEncode(sig.Length));
				append.AddRange(sig);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.SenderWallet);
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				append.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
				append.AddRange(pk.Value.PubKey);
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
			}
			data.AddRange(WalletUtils.VLEncode(model.PayloadCount));
			for (int i = 0; i < (int)model.PayloadCount; i++)
			{
				List<byte> append = [.. BitConverter.GetBytes(Convert.ToUInt16(model.Payloads[i].PayloadFlags, 16))];
				UInt32 j = (UInt32)(model.Payloads[i].WalletAccess != null && model.Payloads[i].WalletAccess.Length > 0 ? model.Payloads[i].WalletAccess.Length : 0);
				append.AddRange(WalletUtils.VLEncode(j));
				if (j > 0)
				{
					List<byte> prbs = [];
					for (int k = 0; k < j; k++)
					{
						List<byte> prb = [];
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.Payloads[i].WalletAccess[k]);
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						prb.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
						prb.AddRange(pk.Value.PubKey);
						prb.AddRange(WalletUtils.VLEncode(model.Payloads[i].Challenges[k].size));
						prb.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].Challenges[k].hex)!);
						prbs.AddRange(WalletUtils.VLEncode(prb.Count));
						prbs.AddRange(prb);
					}
					append.AddRange(WalletUtils.VLEncode(prbs.Count));
					append.AddRange(prbs);
					append.AddRange(WalletUtils.VLEncode(model.Payloads[i].IV.size));
					append.AddRange(WalletUtils.HexStringToByteArray(model.Payloads[i].IV.hex)!);
				}
				hash = WalletUtils.HexStringToByteArray(model.Payloads[i].Hash)!;
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
				append.AddRange(WalletUtils.VLEncode(model.Payloads[i].PayloadSize));
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
				data.AddRange(Convert.FromBase64String(model.Payloads[i].Data));
			}
			string? txid = null, reg = null;
			if (model.TxId != null && model.TxId.Length > 0)
				txid = model.TxId;
			if (model.MetaData != null && model.MetaData.RegisterId != null && model.MetaData.RegisterId.Length > 0)
				reg = model.MetaData.RegisterId;
			return new Transaction() { TxId = txid, RegisterId = reg, Data = [.. data] };
		}
		private static Transaction? FormatV4TxModel(TransactionModel model)
		{
			List<byte> data = [ .. BitConverter.GetBytes(WalletUtils.UInt32Exchange(model.Version | (UInt32)TransportIdentifier.Transaction)) ];
			List<byte> subdata = [];
			byte[] hash = WalletUtils.HexStringToByteArray(model.PrevTxId)!;
			subdata.AddRange(WalletUtils.VLEncode(hash.Length));
			subdata.AddRange(hash);
			if (model.RecipientsWallets == null || !model.RecipientsWallets.Any())
				subdata.Add(0);
			else
			{
				subdata.AddRange(WalletUtils.VLEncode(model.RecipientsWallets.Count()));
				List<byte> append = [];
				for (int i = 0; i < model.RecipientsWallets?.Count(); i++)
				{
					(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.RecipientsWallets?.ElementAt(i));
					if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
						return null;
					List<byte> entry = [pk.Value.Network, .. WalletUtils.VLEncode(pk.Value.PubKey.Length), .. pk.Value.PubKey];
					append.AddRange(WalletUtils.VLEncode(entry.Count));
					append.AddRange(entry);
				}
				subdata.AddRange(WalletUtils.VLEncode(append.Count));
				subdata.AddRange(append);
			}
			subdata.AddRange(BitConverter.GetBytes((UInt64)(model.TimeStamp == DateTime.MinValue ? 0 : new DateTimeOffset(model.TimeStamp).ToUnixTimeSeconds())));
			if (model.MetaData != null)
			{
				try
				{
					byte[] meta = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(model.MetaData));
					subdata.AddRange(WalletUtils.VLEncode(meta.Length));
					subdata.AddRange(meta);
				}
				catch (Exception) { return null; }
			}
			else
				subdata.Add(0);
			data.AddRange(WalletUtils.VLEncode(subdata.Count));
			data.AddRange(subdata);
			if (model.Signature != null && model.Signature.Length > 0 && model.SenderWallet != null && model.SenderWallet.Length > 0)
			{
				subdata.Clear();
				byte[] sig = WalletUtils.HexStringToByteArray(model.Signature)!;
				subdata.AddRange(WalletUtils.VLEncode(sig.Length));
				subdata.AddRange(sig);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.SenderWallet);
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				List<byte> append = [pk.Value.Network, .. WalletUtils.VLEncode(pk.Value.PubKey.Length), .. pk.Value.PubKey];
				subdata.AddRange(WalletUtils.VLEncode(append.Count));
				subdata.AddRange(append);
				data.AddRange(WalletUtils.VLEncode(subdata.Count));
				data.AddRange(subdata);
			}
			data.AddRange(WalletUtils.VLEncode(model.PayloadCount));
			for (int i = 0; i < (int)model.PayloadCount; i++)
			{
				List<byte> append = [];
				UInt64 fd = Convert.ToUInt64(model.Payloads[i].PayloadFlags, 16);
				append.AddRange(BitConverter.GetBytes(fd));
				UInt32 j = (UInt32)(model.Payloads[i].WalletAccess != null && model.Payloads[i].WalletAccess.Length > 0 ? model.Payloads[i].WalletAccess.Length : 0);
				append.AddRange(WalletUtils.VLEncode(j));
				if (j > 0)
				{
					List<byte> prbs = [];
					for (int k = 0; k < j; k++)
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(model.Payloads[i].WalletAccess[k]);
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						List<byte> prb =
						[
							pk.Value.Network,
							.. WalletUtils.VLEncode(pk.Value.PubKey.Length),
							.. pk.Value.PubKey,
							.. WalletUtils.VLEncode(model.Payloads[i].Challenges[k].size),
							.. WalletUtils.HexStringToByteArray(model.Payloads[i].Challenges[k].hex)!,
						];
						prbs.AddRange(WalletUtils.VLEncode(prb.Count));
						prbs.AddRange(prb);
					}
					append.AddRange(WalletUtils.VLEncode(prbs.Count));
					append.AddRange(prbs);
				}
				hash = WalletUtils.HexStringToByteArray(model.Payloads[i].Hash)!;
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
				append.AddRange(WalletUtils.VLEncode(model.Payloads[i].PayloadSize));
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
				if ((((PayloadFlags)(fd >> 48)) & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION)
				{
					EncryptionType t = (EncryptionType)((fd >> 37) & 31);
					data.AddRange(model.Payloads[i].IV != null && model.Payloads[i].IV.hex != null && model.Payloads[i].IV.size == (ulong)t.GetIVSizeAttribute() ?
						WalletUtils.HexStringToByteArray(model.Payloads[i].IV.hex)! : new byte[t.GetIVSizeAttribute()]);
				}
				data.AddRange(Convert.FromBase64String(model.Payloads[i].Data));
			}
			string? txid = null, reg = null;
			if (model.TxId != null && model.TxId.Length > 0)
				txid = model.TxId;
			if (model.MetaData != null && model.MetaData.RegisterId != null && model.MetaData.RegisterId.Length > 0)
				reg = model.MetaData.RegisterId;
			return new Transaction() { TxId = txid, RegisterId = reg, Data = [.. data] };
		}

/** JSON to Transaction methods **/
		private static Transaction? FormatV1TxJSON(JsonDocument document)
		{
			List<byte> data = [];
			JsonElement element = document.RootElement;
			data.AddRange(BitConverter.GetBytes(element.GetProperty(JSONKeys.Version).GetUInt32()));
			JsonElement.ArrayEnumerator rpcount = element.GetProperty(JSONKeys.Recipients).EnumerateArray();
			data.AddRange(WalletUtils.VLEncode(rpcount.Count()));
			while (rpcount.MoveNext())
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(rpcount.Current.GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			UInt64 ts = 0;
			if (element.TryGetProperty(JSONKeys.TimeStamp, out JsonElement t))
				ts = (UInt64)((DateTimeOffset)DateTime.Parse(t.GetString()!)).ToUnixTimeSeconds();
			data.AddRange(BitConverter.GetBytes(ts));
			if (element.TryGetProperty(JSONKeys.Signature, out JsonElement sig))
			{
				byte[] sigstr = WalletUtils.HexStringToByteArray(sig.GetString()) ?? [];
				data.AddRange(WalletUtils.VLEncode(sigstr.Length));
				data.AddRange(sigstr);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(element.GetProperty(JSONKeys.Sender).GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			data.AddRange(WalletUtils.VLEncode(element.GetProperty(JSONKeys.PayloadCount).GetUInt32()));
			rpcount = element.GetProperty(JSONKeys.Payloads).EnumerateArray();
			while (rpcount.MoveNext())
			{
				JsonElement.ArrayEnumerator wacount = rpcount.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				UInt32 count = (UInt32)wacount.Count();
				data.AddRange(WalletUtils.VLEncode(count));
				if (count > 0)
				{
					JsonElement.ArrayEnumerator chcount = rpcount.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					while (wacount.MoveNext() && chcount.MoveNext())
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(wacount.Current.GetString());
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						data.AddRange(pk.Value.PubKey);
						data.AddRange(WalletUtils.VLEncode(chcount.Current.GetProperty(JSONKeys.Size).GetUInt32()));
						data.AddRange(WalletUtils.HexStringToByteArray(chcount.Current.GetProperty(JSONKeys.Hex).GetString())!);
					}
					data.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32()));
					data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString())!);
				}
				data.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32()));
				data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Hash).GetString())!);
				data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Data).GetString())!);
			}
			string? txid = null;
			if (element.TryGetProperty(JSONKeys.TxId, out JsonElement id))
				txid = string.IsNullOrEmpty(id.GetString()) ? null : id.GetString();
			return new Transaction() { TxId = txid, Data = [.. data] };
		}
		private static Transaction? FormatV2TxJSON(JsonDocument document)
		{
			List<byte> data = [];
			JsonElement element = document.RootElement;
			data.AddRange(BitConverter.GetBytes(element.GetProperty(JSONKeys.Version).GetUInt32()));
			JsonElement.ArrayEnumerator rpcount = element.GetProperty(JSONKeys.Recipients).EnumerateArray();
			data.AddRange(WalletUtils.VLEncode(rpcount.Count()));
			while (rpcount.MoveNext())
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(rpcount.Current.GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			UInt64 ts = 0;
			if (element.TryGetProperty(JSONKeys.TimeStamp, out JsonElement t))
				ts = (UInt64)((DateTimeOffset)DateTime.Parse(t.GetString()!)).ToUnixTimeSeconds();
			data.AddRange(BitConverter.GetBytes(ts));
			if (element.TryGetProperty(JSONKeys.Signature, out JsonElement sig))
			{
				byte[] sigstr = WalletUtils.HexStringToByteArray(sig.GetString()) ?? [];
				data.AddRange(WalletUtils.VLEncode(sigstr.Length));
				data.AddRange(sigstr);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(element.GetProperty(JSONKeys.Sender).GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				data.AddRange(pk.Value.PubKey);
			}
			data.AddRange(WalletUtils.VLEncode(element.GetProperty(JSONKeys.PayloadCount).GetUInt32()));
			rpcount = element.GetProperty(JSONKeys.Payloads).EnumerateArray();
			while (rpcount.MoveNext())
			{
				data.AddRange(BitConverter.GetBytes((UInt16)(rpcount.Current.GetProperty(JSONKeys.Encrypted).GetBoolean() ? PayloadFlags.PAYLOAD_ENCRYPTION : 0)));
				JsonElement.ArrayEnumerator wacount = rpcount.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				UInt32 count = (UInt32)wacount.Count();
				data.AddRange(WalletUtils.VLEncode(count));
				if (count > 0)
				{
					JsonElement.ArrayEnumerator chcount = rpcount.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					while (wacount.MoveNext() && chcount.MoveNext())
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(wacount.Current.GetString());
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						data.AddRange(pk.Value.PubKey);
						data.AddRange(WalletUtils.VLEncode(chcount.Current.GetProperty(JSONKeys.Size).GetUInt32()));
						data.AddRange(WalletUtils.HexStringToByteArray(chcount.Current.GetProperty(JSONKeys.Hex).GetString())!);
					}
					data.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32()));
					data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString())!);
				}
				data.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32()));
				data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Hash).GetString())!);
				data.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Data).GetString())!);
			}
			string? txid = null;
			if (element.TryGetProperty(JSONKeys.TxId, out JsonElement id))
				txid = string.IsNullOrEmpty(id.GetString()) ? null : id.GetString();
			return new Transaction() { TxId = txid, Data = [.. data] };
		}
		private static Transaction? FormatV3TxJSON(JsonDocument document)
		{
			List<byte> data = [];
			JsonElement element = document.RootElement;
			data.AddRange(BitConverter.GetBytes(WalletUtils.UInt32Exchange(element.GetProperty(JSONKeys.Version).GetUInt32() | (UInt32)TransportIdentifier.Transaction)));
			byte[] hash = WalletUtils.HexStringToByteArray(element.GetProperty(JSONKeys.PrevId).GetString())!;
			data.AddRange(WalletUtils.VLEncode(hash.Length));
			data.AddRange(hash);
			JsonElement.ArrayEnumerator rpcount = element.GetProperty(JSONKeys.Recipients).EnumerateArray();
			data.AddRange(WalletUtils.VLEncode(rpcount.Count()));
			List<byte> append = [];
			while (rpcount.MoveNext())
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(rpcount.Current.GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				append.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
				append.AddRange(pk.Value.PubKey);
			}
			if (rpcount.Any())
				data.AddRange(WalletUtils.VLEncode(append.Count));
			data.AddRange(append);
			UInt64 ts = 0;
			if (element.TryGetProperty(JSONKeys.TimeStamp, out JsonElement t))
				ts = (UInt64)((DateTimeOffset)DateTime.Parse(t.GetString()!)).ToUnixTimeSeconds();
			data.AddRange(BitConverter.GetBytes(ts));
			if (element.TryGetProperty(JSONKeys.MetaData, out JsonElement m))
			{
				hash = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
				data.AddRange(WalletUtils.VLEncode(hash.Length));
				data.AddRange(hash);
			}
			else
				data.Add(0);
			if (element.TryGetProperty(JSONKeys.Signature, out JsonElement sig))
			{
				hash = WalletUtils.HexStringToByteArray(sig.GetString()) ?? [];
				append.Clear();
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(element.GetProperty(JSONKeys.Sender).GetString());
				if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
					return null;
				append.AddRange(WalletUtils.VLEncode(pk.Value.PubKey.Length));
				append.AddRange(pk.Value.PubKey);
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
			}
			data.AddRange(WalletUtils.VLEncode(element.GetProperty(JSONKeys.PayloadCount).GetUInt32()));
			rpcount = element.GetProperty(JSONKeys.Payloads).EnumerateArray();
			while (rpcount.MoveNext())
			{
				append.Clear();
				append.AddRange(BitConverter.GetBytes(Convert.ToUInt16(rpcount.Current.GetProperty(JSONKeys.Flags).GetString(), 16)));
				JsonElement.ArrayEnumerator wacount = rpcount.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				UInt32 count = (UInt32)wacount.Count();
				append.AddRange(WalletUtils.VLEncode(count));
				if (count > 0)
				{
					List<byte> prsbs = [];
					JsonElement.ArrayEnumerator chcount = rpcount.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					while (wacount.MoveNext() && chcount.MoveNext())
					{
						(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(wacount.Current.GetString());
						if (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute())
							return null;
						List<byte> prbs =
						[
							.. WalletUtils.VLEncode(pk.Value.PubKey.Length),
							.. pk.Value.PubKey,
							.. WalletUtils.VLEncode(chcount.Current.GetProperty(JSONKeys.Size).GetUInt32()),
							.. WalletUtils.HexStringToByteArray(chcount.Current.GetProperty(JSONKeys.Hex).GetString())!,
						];
						prsbs.AddRange(WalletUtils.VLEncode(prbs.Count));
						prsbs.AddRange(prbs);
					}
					append.AddRange(WalletUtils.VLEncode(prsbs.Count));
					append.AddRange(prsbs);
					append.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32()));
					append.AddRange(WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString())!);
				}
				hash = WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Hash).GetString())!;
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
				append.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32()));
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
				data.AddRange(Convert.FromBase64String(rpcount.Current.GetProperty(JSONKeys.Data).GetString()!));
			}
			string? txid = null, rid = null;
			if (element.TryGetProperty(JSONKeys.TxId, out JsonElement id))
				txid = string.IsNullOrEmpty(id.GetString()) ? null : id.GetString();
			if (element.TryGetProperty(JSONKeys.MetaData, out JsonElement meta))
			{
				if (meta.TryGetProperty(JSONKeys.Register, out JsonElement idr))
					rid = string.IsNullOrEmpty(idr.GetString()) ? null : idr.GetString();
			}
			return new Transaction() { TxId = txid, RegisterId = rid, Data = [.. data] };
		}
		private static Transaction? FormatV4TxJSON(JsonDocument document)
		{
			static (byte Network, byte[] PubKey)? ReadWallet(JsonElement element)
			{
				(byte Network, byte[] PubKey)? pk = WalletUtils.WalletToPubKey(element.GetProperty(JSONKeys.Wallet).GetString());
				byte defnet = Convert.ToByte(element.GetProperty(JSONKeys.Network).GetString(), 16);
				return (pk == null || pk.Value.PubKey.Length != ((WalletNetworks)pk.Value.Network).GetPubKeySizeAttribute() || pk.Value.Network != defnet) ? null : pk;
			}

			List<byte> data = [];
			JsonElement element = document.RootElement;
			string? tp = element.GetProperty(JSONKeys.Type).GetString();
			if (tp == null || !Enum.IsDefined(typeof(TransportIdentifier), tp))
				return null;
			data.AddRange(BitConverter.GetBytes(WalletUtils.UInt32Exchange(element.GetProperty(JSONKeys.Version).GetUInt32() |
				(UInt32)Enum.Parse(typeof(TransportIdentifier), tp))));
			List<byte> append = [];
			byte[] hash = WalletUtils.HexStringToByteArray(element.GetProperty(JSONKeys.PrevId).GetString())!;
			append.AddRange(WalletUtils.VLEncode(hash.Length));
			append.AddRange(hash);
			JsonElement.ArrayEnumerator rpcount = element.GetProperty(JSONKeys.Recipients).EnumerateArray();
			append.AddRange(WalletUtils.VLEncode(rpcount.Count()));
			if (rpcount.Any())
			{
				List<byte> subdata = [];
				while (rpcount.MoveNext())
				{
					(byte Network, byte[] PubKey)? pk = ReadWallet(rpcount.Current);
					if (pk == null)
						return null;
					List<byte> entry = [pk.Value.Network, .. WalletUtils.VLEncode(pk.Value.PubKey.Length), .. pk.Value.PubKey];
					subdata.AddRange(WalletUtils.VLEncode(entry.Count));
					subdata.AddRange(entry);
				}
				append.AddRange(WalletUtils.VLEncode(subdata.Count));
				append.AddRange(subdata);
			}
			UInt64 ts = 0;
			if (element.TryGetProperty(JSONKeys.TimeStamp, out JsonElement t))
				ts = (UInt64)((DateTimeOffset)DateTime.Parse(t.GetString()!)).ToUnixTimeSeconds();
			append.AddRange(BitConverter.GetBytes(ts));
			if (element.TryGetProperty(JSONKeys.MetaData, out JsonElement m))
			{
				hash = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
			}
			else
				append.Add(0);
			data.AddRange(WalletUtils.VLEncode(append.Count));
			data.AddRange(append);
			if (element.TryGetProperty(JSONKeys.Signature, out JsonElement sig))
			{
				append.Clear();
				append.AddRange(WalletUtils.HexStringToByteArray(sig.GetProperty(JSONKeys.Hex).GetString())!);
				append.InsertRange(0, WalletUtils.VLEncode(append.Count));
				(byte Network, byte[] PubKey)? pk = ReadWallet(element.GetProperty(JSONKeys.Sender));
				if (pk == null)
					return null;
				List<byte> entry = [pk.Value.Network, .. WalletUtils.VLEncode(pk.Value.PubKey.Length), .. pk.Value.PubKey];
				append.AddRange(WalletUtils.VLEncode(entry.Count));
				append.AddRange(entry);
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
			}
			data.AddRange(WalletUtils.VLEncode(element.GetProperty(JSONKeys.PayloadCount).GetUInt32()));
			rpcount = element.GetProperty(JSONKeys.Payloads).EnumerateArray();
			while (rpcount.MoveNext())
			{
				append.Clear();
				append.AddRange(BitConverter.GetBytes(Convert.ToUInt16(rpcount.Current.GetProperty(JSONKeys.UserField).GetString(), 16)));
				append.AddRange(BitConverter.GetBytes((UInt16)Enum.Parse(typeof(PayloadTypeField), rpcount.Current.GetProperty(JSONKeys.TypeField).GetString()!)));
				UInt32 pf = (UInt32)Convert.ToUInt32(rpcount.Current.GetProperty(JSONKeys.Options).GetString(), 16);
				append.AddRange(BitConverter.GetBytes(pf));
				JsonElement.ArrayEnumerator wacount = rpcount.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				UInt32 count = (UInt32)wacount.Count();
				append.AddRange(WalletUtils.VLEncode(count));
				if (count > 0)
				{
					List<byte> prsbs = [];
					while (wacount.MoveNext())
					{
						(byte Network, byte[] PubKey)? pk = ReadWallet(wacount.Current);
						if (pk == null)
							return null;
						List<byte> prbs =
						[
							pk.Value.Network,
							.. WalletUtils.VLEncode(pk.Value.PubKey.Length),
							.. pk.Value.PubKey,
							.. WalletUtils.VLEncode(wacount.Current.GetProperty("Challenge").GetProperty(JSONKeys.Size).GetUInt32()),
							.. WalletUtils.HexStringToByteArray(wacount.Current.GetProperty("Challenge").GetProperty(JSONKeys.Hex).GetString())!,
						];
						prsbs.AddRange(WalletUtils.VLEncode(prbs.Count));
						prsbs.AddRange(prbs);
					}
					append.AddRange(WalletUtils.VLEncode(prsbs.Count));
					append.AddRange(prsbs);
				}
				hash = WalletUtils.HexStringToByteArray(rpcount.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString())!;
				append.AddRange(WalletUtils.VLEncode(hash.Length));
				append.AddRange(hash);
				append.AddRange(WalletUtils.VLEncode(rpcount.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32()));
				data.AddRange(WalletUtils.VLEncode(append.Count));
				data.AddRange(append);
				data.AddRange(Convert.FromBase64String(rpcount.Current.GetProperty(JSONKeys.Data).GetString()!));
			}
			string? txid = null, rid = null;
			if (element.TryGetProperty(JSONKeys.TxId, out JsonElement id))
				txid = string.IsNullOrEmpty(id.GetString()) ? null : id.GetString();
			if (element.TryGetProperty(JSONKeys.MetaData, out JsonElement meta))
			{
				if (meta.TryGetProperty(JSONKeys.Register, out JsonElement idr))
					rid = string.IsNullOrEmpty(idr.GetString()) ? null : idr.GetString();
			}
			return new Transaction() { TxId = txid, RegisterId = rid, Data = [.. data] };
		}
		private static string ValueWrap(string? value) { return "\"" + value + "\""; }
		private static string ItemValueWrap(string? value) { return ValueWrap(value) + ','; }
	}
}
