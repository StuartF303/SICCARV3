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

// Key Utilities Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Numerics;
using System.IO.Compression;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sodium;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public enum CryptoMode
	{
		ENCRYPT,
		DECRYPT
	}
	public class WalletUtils
	{
		private const string bech_def = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
		private static readonly byte[] bech_rev = [
			0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
			0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
			0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
			0x0f, 0xff, 0x0a, 0x11, 0x15, 0x14, 0x1a, 0x1e, 0x07, 0x05, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
			0xff, 0x1d, 0xff, 0x18, 0x0d, 0x19, 0x09, 0x08, 0x17, 0xff, 0x12, 0x16, 0x1f, 0x1b, 0x13, 0xff,
			0x01, 0x00, 0x03, 0x10, 0x0b, 0x1c, 0x0c, 0x0e, 0x06, 0x04, 0x02, 0xff, 0xff, 0xff, 0xff, 0xff,
			0xff, 0x1d, 0xff, 0x18, 0x0d, 0x19, 0x09, 0x08, 0x17, 0xff, 0x12, 0x16, 0x1f, 0x1b, 0x13, 0xff,
			0x01, 0x00, 0x03, 0x10, 0x0b, 0x1c, 0x0c, 0x0e, 0x06, 0x04, 0x02, 0xff, 0xff, 0xff, 0xff, 0xff ];
		private const string base58_def = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
		private static readonly byte[] base58_rev = [
				0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xff, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xff, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0xff, 0x11, 0x12, 0x13, 0x14, 0x15, 0xff,
				0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x20, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xff, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0xff, 0x2c, 0x2d, 0x2e,
				0x2f, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0xff, 0xff, 0xff, 0xff, 0xff ];
		private static readonly byte[] comp_id = [ 0x57, 0x4c, 0x44, 0x01 ];
		public static class Tokens
		{
			internal class SubRule
			{
				public (byte[] p_bytes, UInt32 p_off) Match;
				public bool Predicate;
			}
			public class Rule
			{
				public (byte[] p_bytes, UInt32 p_off) Match;
				internal SubRule[]? SubRules;
			}
			public static readonly Rule[] rules =
			[
				new Rule()
				{
					Match = ("BM"u8.ToArray(), 0x00),																// BMP
					SubRules = [new SubRule { Match = (new byte[] { 0x00 }, 0x1e), Predicate = true }]
				},
				new Rule() { Match = ("JFIF"u8.ToArray(), 0x06) },													// JFIF
				new Rule() { Match = ("PK"u8.ToArray(), 0x00) },													// PK, DOCX, ZIP
				new Rule() { Match = (new byte[] { 0xff, 0xfb }, 0x00) },											// MP3
				new Rule() { Match = ("ID3"u8.ToArray(), 0x00) },													// MP3
				new Rule() { Match = ("Rar!"u8.ToArray(), 0x00) },													// RAR
				new Rule() { Match = ("%PDF"u8.ToArray(), 0x00) },													// PDF
				new Rule() { Match = (new byte[] { 0x1f, 0x8b }, 0x00) },											// GZ
				new Rule() { Match = (new byte[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c }, 0x00) },					// 7z
				new Rule() { Match = (new byte[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00 }, 0x00) },					// XZ
				new Rule() { Match = (new byte[] { 0x89, 0x50, 0x4e, 0x47 }, 0x00) },								// PNG
				new Rule() { Match = ("OggS"u8.ToArray(), 0x00) },													// OGG
				new Rule() { Match = ("GIF"u8.ToArray(), 0x00) }													// GIF
			];
		}
		public struct CryptoKey(WalletNetworks network, byte[]? key)
		{
			public WalletNetworks Network = network;
			public byte[]? Key = key;
		}
		public struct KeySet
		{
			public WalletUtils.CryptoKey PrivateKey { get; set; }
			public WalletUtils.CryptoKey PublicKey { get; set; }
		}

		public static byte[]? Base58Decode(string? data)
		{
			if (data == null || data.Length < 1)
				return null;
			byte[] decode = new byte[(data.Length * 138 / 100) + 1];
			int resultlen = 1;
			for (int i = 0; i < data.Length; i++)
			{
				UInt32 carry = base58_rev[data[i] & 0x7f];
				for (int j = 0; j < resultlen; j++, carry >>= 8)
				{
					carry += (UInt32)(decode[j] * 58);
					decode[j] = (byte)carry;
				}
				for (; carry > 0; carry >>= 8)
					decode[resultlen++] = (byte)(carry & 0xff);
			}
			List<byte> result = [];
			for (int i = 0; i < resultlen; i++)
				result.Add(decode[i]);
			for (int i = 0; i < (data.Length - 1) && data[i] == base58_def[0]; i++)
				result.Add(0);
			result.Reverse();
			return [.. result];
		}
		public static string? Base58Encode(byte[]? data)
		{
			if (data == null || data.Length < 1)
				return null;
			byte[] digits = new byte[(data.Length * 138 / 100) + 1];
			int digitslen = 1;
			for (int i = 0; i < data.Length; i++)
			{
				UInt32 carry = data[i];
				for (int j = 0; j < digitslen; j++)
				{
					carry += (UInt32)(digits[j] << 8);
					digits[j] = (byte)(carry % 58);
					carry /= 58;
				}
				for (; carry > 0; carry /= 58)
					digits[digitslen++] = (byte)(carry % 58);
			}
			StringBuilder builder = new();
			for (int i = 0; i < (data.Length - 1) && data[i] == 0; i++)
				builder.Append(base58_def[0]);
			for (int i = 0; i < digitslen; i++)
				builder.Append(base58_def[digits[digitslen - 1 - i]]);
			return builder.ToString();
		}
		public static List<byte> VLEncode(BigInteger value)
		{
			List<byte> sz = [];
			if (value > 0)
			{
				do
				{
					sz.Add((value & -128) != 0 ? (byte)(value & 0x7f | 0x80) : (byte)value);
					value >>= 7;
				} while (value != 0);
			}
			else
				sz.Add(0);
			return sz;
		}
		public static BigInteger VLDecode(List<byte> value)
		{
			BigInteger vl = 0;
			for (int i = value.Count - 1; i >= 0; i--)
			{
				vl <<= 7;
				vl |= (byte)(value[i] & 0x7f);
			}
			return vl;
		}
		public static BigInteger ReadVLSize(byte[] data, UInt32 offset = 0)
		{
			List<byte> sz = [];
			if (data != null && offset < data.Length)
			{
				int i = 0;
				do
				{
					sz.Add(data[offset + i++]);
				} while ((sz[^1] & 0x80) != 0 && ((offset + i) < data.Length));
			}
			return VLDecode(sz);
		}
		public static BigInteger ReadVLSize(BinaryReader reader)
		{
			List<byte> sz = [];
			if (reader != null && reader.BaseStream.CanRead)
			{
				try
				{
					while (reader.BaseStream.Length != reader.BaseStream.Position)
					{
						sz.Add(reader.ReadByte());
						if ((sz[^1] & 0x80) == 0)
							break;
					}
				}
				catch (Exception) {}
			}
			return VLDecode(sz);
		}
		public static (BigInteger Size, UInt32 Count) ReadVLCount(BinaryReader reader)
		{
			List<byte> sz = [];
			if (reader != null && reader.BaseStream.CanRead)
			{
				try
				{
					while (reader.BaseStream.Length != reader.BaseStream.Position)
					{
						sz.Add(reader.ReadByte());
						if ((sz[^1] & 0x80) == 0)
							break;
					}
				}
				catch (Exception) {}
			}
			return (VLDecode(sz), (UInt32)sz.Count);
		}
		public static byte[] ReadVLArray(BinaryReader reader)
		{
			byte[] array = [];
			if (reader != null && reader.BaseStream.CanRead)
			{
				long c = (long)ReadVLSize(reader);
				if (c > 0)
				{
					try
					{
						if ((reader.BaseStream.Position + c) <= reader.BaseStream.Length)
							array = reader.ReadBytes((int)c);
					}
					catch (Exception) {}
				}
			}
			return array;
		}
		public static (byte[] Data, UInt32 Count) ReadVLArrayCount(BinaryReader reader)
		{
			byte[] array = [];
			(BigInteger Size, UInt32 Count) rd = (BigInteger.Zero, 0);
			if (reader != null && reader.BaseStream.CanRead)
			{
				rd = ReadVLCount(reader);
				if (rd.Size > 0)
				{
					try
					{
						if ((reader.BaseStream.Position + rd.Size) <= reader.BaseStream.Length)
							array = reader.ReadBytes((int)rd.Size);
					}
					catch (Exception) {}
				}
			}
			return (array, rd.Count);
		}
		public static BinaryReader SkipVLSize(BinaryReader reader)
		{
			try
			{
				if (reader.BaseStream.CanRead)
					while (reader.BaseStream.Length != reader.BaseStream.Position && ((reader.ReadByte() & 0x80) != 0));
			}
			catch (Exception) {}
			return reader;
		}
		public static byte[]? HexStringToByteArray(string? hex_str)
		{
			static int HexVal(char h) { return h - (h < 58 ? 48 : (h < 97 ? 55 : 87)); }

			if (hex_str == null || hex_str.Length < 1 || (hex_str.Length & 1) != 0)
				return null;
			foreach (char c in hex_str)
			{
				if (c < 48 || c > 102 || (c > 57 && c < 65) || (c > 70 && c < 97))
					return null;
			}
			byte[] hex_bytes = new byte[hex_str.Length >> 1];
			for (int i = 0; i < hex_str.Length >> 1; i++)
				hex_bytes[i] = (byte)((HexVal(hex_str[i << 1]) << 4) + HexVal(hex_str[(i << 1) + 1]));
			return hex_bytes;
		}
		public static string? ByteArrayToHexString(byte[]? byte_array)
		{
			if (byte_array == null || byte_array.Length < 1)
				return null;
			return BitConverter.ToString(byte_array).Replace("-", "").ToLower();
		}
		public static string? PubKeyToWallet(byte[]? public_key, byte network)
		{
			static List<byte> ConvertBits(byte[] data)
			{
				int acc = 0;
				int bits = 0;
				List<byte> bit_data = [];
				for (int i = 0; i < data.Length; i++)
				{
					byte value = data[i];
					acc = ((acc << 8) | value) & 0x00000fff;
					bits += 8;
					while (bits >= 5)
					{
						bits -= 5;
						bit_data.Add((byte)((acc >> bits) & 0x1f));
					}
				}
				if (bits > 0)
					bit_data.Add((byte)((acc << (5 - bits)) & 0x1f));
				return bit_data;
			}

			static byte[] CalculateSum(List<byte> data)
			{
				List<byte> sum = [0x03, 0x03, 0x00, 0x17, 0x13, .. data, .. new byte[6]];
				UInt32 poly = 1;
				sum.ForEach(v =>
				{
					byte c0 = (byte)(poly >> 25);
					poly = ((poly & 0x1ffffff) << 5) ^ v;
					if ((c0 & 0x01) == 0x01)
						poly ^= 0x3b6a57b2;
					if ((c0 & 0x02) == 0x02)
						poly ^= 0x26508e6d;
					if ((c0 & 0x04) == 0x04)
						poly ^= 0x1ea119fa;
					if ((c0 & 0x08) == 0x08)
						poly ^= 0x3d4233dd;
					if ((c0 & 0x10) == 0x10)
						poly ^= 0x2a1462b3;
				});
				poly ^= 0x2bc830a3;
				byte[] csum = new byte[6];
				for (int i = 0; i < 6; i++)
					csum[i] = (byte)((poly >> (5 * (5 - i))) & 31);
				return csum;
			}

			if (public_key == null || public_key.Length < 0x20 || network >= bech_def.Length || !Enum.IsDefined(typeof(WalletNetworks), network))
				return null;
			List<byte> data = ConvertBits(public_key);
			data.Insert(0, network);
			byte[] csum = CalculateSum(data);
			data.AddRange(csum);
			StringBuilder sb = new();
			sb.Append("ws1");
			data.ForEach(b => sb.Append(bech_def[b]));
			return sb.ToString();
		}
		public static (byte Network, byte[] PubKey)? WalletToPubKey(string? wallet)
		{
			static List<byte>? ConvertBits(byte[] data)
			{
				int acc = 0;
				int bits = 0;
				List<byte> bit_data = [];
				for (int i = 0; i < data.Length; i++)
				{
					byte value = data[i];
					acc = ((acc << 5) | value) & 0x00000fff;
					bits += 5;
					while (bits >= 8)
					{
						bits -= 8;
						bit_data.Add((byte)((acc >> bits) & 0xff));
					}
				}
				if (bits >= 5 || ((acc << (8 - bits)) & 0xff) > 0)
					return null;
				return bit_data;
			}

			static uint CalculateSum(byte[] data)
			{
				List<byte> sum = [0x03, 0x03, 0x00, 0x17, 0x13, .. data];
				UInt32 poly = 1;
				sum.ForEach(v =>
				{
					byte c0 = (byte)(poly >> 25);
					poly = ((poly & 0x1ffffff) << 5) ^ v;
					if ((c0 & 0x01) == 0x01)
						poly ^= 0x3b6a57b2;
					if ((c0 & 0x02) == 0x02)
						poly ^= 0x26508e6d;
					if ((c0 & 0x04) == 0x04)
						poly ^= 0x1ea119fa;
					if ((c0 & 0x08) == 0x08)
						poly ^= 0x3d4233dd;
					if ((c0 & 0x10) == 0x10)
						poly ^= 0x2a1462b3;
				});
				return poly;
			}

			if (wallet != null)
			{
				int index = wallet.LastIndexOf('1');
				if (wallet.Length > 0x22 && index > 0 && ((index + 7) <= wallet.Length) && wallet.StartsWith("ws1"))
				{
					for (int i = index + 1; i < wallet.Length; i++)
					{
						if (wallet[i] < '0' || wallet[i] == '1' || wallet[i] == 'b' || wallet[i] == 'i' || wallet[i] == 'o' || wallet[i] > 'z' || (wallet[i] > '9' && wallet[i] < 'a'))
							return null;
					}
					byte[] values = new byte[wallet.Length - 1 - index];
					for (int i = 0; i < (wallet.Length - 1 - index); ++i)
						values[i] = bech_rev[wallet[i + index + 1]];
					if (CalculateSum(values) == 0x2bc830a3)
					{
						byte[] data = new byte[values.Length - 7];
						Array.Copy(values, 1, data, 0, data.Length);
						byte[]? bytes = ConvertBits(data)?.ToArray();
						return bytes != null ? (values[0], bytes) : null;
					}
				}
			}
			return null;
		}
		public static string? PrivKeyToWIF(byte[]? private_key, byte network)
		{
			if (private_key == null || private_key.Length < 1)
				return null;
			List<byte> key = new(private_key);
			key.Insert(0, network);
			byte[] csum = SHA256.HashData(SHA256.HashData(key.ToArray()));
			for (int i = 0; i < 4; i++)
				key.Add(csum[i]);
			return Base58Encode([.. key]);
		}
		public static (byte Network, byte[] PrivateKey)? WIFToPrivKey(string? wif_key)
		{
			if (wif_key == null || wif_key.Length < 1)
				return null;
			byte[]? data = Base58Decode(wif_key);
			if (data != null && data.Length > 5)
			{
				List<byte> csum_data = new(data);
				csum_data.RemoveRange(csum_data.Count - 4, 4);
				byte[] csum = SHA256.HashData(SHA256.HashData(csum_data.ToArray()));
				csum_data.RemoveAt(0);
				for (int i = 0; i < 4; i++)
				{
					if (csum[i] != data[data.Length - 4 + i])
						return null;
				}
				return (data[0], csum_data.ToArray());
			}
			return null;
		}
		public static byte[]? ComputeSHA256Hash(byte[]? data)
		{
			if (data == null || data.Length < 1)
				return null;
			return ComputeSHA256Hash(data.AsSpan());
		}
		public static byte[] ComputeSHA256Hash(Span<byte> data) { return SHA256.HashData(data); }
		public static byte[]? ComputeSHA384Hash(byte[]? data)
		{
			if (data == null || data.Length < 1)
				return null;
			return ComputeSHA384Hash(data.AsSpan());
		}
		public static byte[] ComputeSHA384Hash(Span<byte> data) { return SHA384.HashData(data); }
		public static byte[]? ComputeSHA512Hash(byte[]? data)
		{
			if (data == null || data.Length < 1)
				return null;
			return ComputeSHA512Hash(data.AsSpan());
		}
		public static byte[] ComputeSHA512Hash(Span<byte> data) { return SHA512.HashData(data); }
		public static byte[]? ComputeBlake2bHash(byte[]? data, UInt32 size = 32)
		{
			if (data == null || data.Length < 1 || size < 32 || size > 64)
				return null;
			return GenericHash.Hash(data, null, (int)size);
		}
		public static byte[]? HashData(byte[]? data, HashType type)
		{
			return (data == null || data.Length < 1) ? null : type switch
			{
				HashType.SHA384 => SHA384.HashData(data),
				HashType.SHA512 => SHA512.HashData(data),
				HashType.Blake2b_256 => ComputeBlake2bHash(data),
				HashType.Blake2b_512 => ComputeBlake2bHash(data, 64),
				_ => SHA256.HashData(data)
			};
		}
		public static byte[]? CryptoData(CryptoMode mode, byte[]? data, ref Aes aes)
		{
			if (data == null || data.Length < 1)
				return null;
			using ICryptoTransform transform = mode == CryptoMode.ENCRYPT ? aes.CreateEncryptor(aes.Key, aes.IV) : aes.CreateDecryptor(aes.Key, aes.IV);
			using MemoryStream mem_stream = new();
			using CryptoStream writer = new(mem_stream, transform, CryptoStreamMode.Write);
			writer.Write(data);
			writer.FlushFinalBlock();
			return mem_stream.ToArray();
		}
		public static uint UInt32Exchange(uint data) { return (data << 24) | ((data & 0x0000ff00) << 8) | ((data & 0x00ff0000) >> 8) | (data >> 24); }
		public static (byte[]? Data, bool IsCompressed) Compress(byte[]? data, CompressionType type = CompressionType.Balanced)
		{
			static bool IsCompressable(byte[] data)
			{
				for (int i = 0; i < Tokens.rules.Length; i++)
				{
					if (new ReadOnlySpan<byte>(data, (int)Tokens.rules[i].Match.p_off, Tokens.rules[i].Match.p_bytes.Length).SequenceEqual(Tokens.rules[i].Match.p_bytes))
					{
						if (Tokens.rules[i].SubRules != null && Tokens.rules[i].SubRules!.Length > 0)
						{
							for (int j = 0; j < Tokens.rules[i].SubRules?.Length; j++)
							{
								if (!(new ReadOnlySpan<byte>(data, (int)Tokens.rules[i].SubRules![j].Match.p_off, Tokens.rules[i].SubRules![j].Match.p_bytes.Length)
									.SequenceEqual(Tokens.rules[i].SubRules![j].Match.p_bytes) == Tokens.rules[i].SubRules![j].Predicate))
									return false;
							}
							return true;
						}
						return false;
					}
				}
				return true;
			}
			if (data == null || data.Length < 256 || type == CompressionType.None || !IsCompressable(data))
				return (data, false);
			try
			{
				using MemoryStream in_spec = new(data);
				using MemoryStream out_spec = new();
				out_spec.Write(comp_id);
				CompressionLevel level = type switch
				{
					CompressionType.Max => CompressionLevel.SmallestSize,
					CompressionType.Fast => CompressionLevel.Fastest,
					_ => CompressionLevel.Optimal
				};
				using DeflateStream stream = new(out_spec, level);
				in_spec.CopyTo(stream);
				stream.Flush();
				return (out_spec.ToArray(), true);
			}
			catch (Exception) {}
			return (null, false);
		}
		public static byte[]? Decompress(byte[]? data)
		{
			if (data == null || data.Length < 1)
				return data;
			try
			{
				using MemoryStream in_spec = new(data!);
				byte[] isc = new byte[4];
				in_spec.Read(isc);
				if (!isc.AsSpan(0, 4).SequenceEqual(comp_id))
					return data;
				using MemoryStream out_spec = new();
				using DeflateStream stream = new(in_spec, CompressionMode.Decompress);
				stream.CopyTo(out_spec);
				stream.Flush();
				return out_spec.ToArray();
			}
			catch (Exception) {}
			return null;
		}
		public static (byte[]? Data, byte[]? Key, byte[]? IV) Encrypt(byte[]? data, EncryptionType type = EncryptionType.XCHACHA20_POLY1305)
		{
			static (byte[] Data, byte[] Key, byte[] IV) CryptoCBC(byte[] data, int keysize)
			{
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.KeySize = keysize << 3;
				aes.GenerateKey();
				aes.GenerateIV();
				using ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
				using MemoryStream mem_stream = new();
				using CryptoStream writer = new(mem_stream, transform, CryptoStreamMode.Write);
				writer.Write(data);
				writer.FlushFinalBlock();
				return (mem_stream.ToArray(), aes.Key, aes.IV);
			}
			static (byte[] Data, byte[] Key, byte[] IV) CryptoAESGCM(byte[] data)
			{
				byte[] key = SodiumCore.GetRandomBytes(EncryptionType.AES_GCM.GetSymKeySizeAttribute());
				byte[] iv = SecretAeadAes.GenerateNonce();
				return (SecretAeadAes.Encrypt(data, iv, key), key, iv);
			}
			static (byte[] Data, byte[] Key, byte[] IV) CryptoCHACHA20(byte[] data)
			{
				byte[] key = SodiumCore.GetRandomBytes(EncryptionType.CHACHA20_POLY1305.GetSymKeySizeAttribute());
				byte[] iv = SecretAeadChaCha20Poly1305.GenerateNonce();
				return (SecretAeadChaCha20Poly1305.Encrypt(data, iv, key), key, iv);
			}
			static (byte[] Data, byte[] Key, byte[] IV) CryptoXCHACHA20(byte[] data)
			{
				byte[] key = SodiumCore.GetRandomBytes(EncryptionType.XCHACHA20_POLY1305.GetSymKeySizeAttribute());
				byte[] iv = SecretAeadXChaCha20Poly1305.GenerateNonce();
				return (SecretAeadXChaCha20Poly1305.Encrypt(data, iv, key), key, iv);
			}
			try
			{
				if (data != null && data.Length > 0)
				{
					return type switch
					{
						EncryptionType.None => (data, null, null),
						EncryptionType.AES_128 => CryptoCBC(data, EncryptionType.AES_128.GetSymKeySizeAttribute()),
						EncryptionType.AES_256 => CryptoCBC(data, EncryptionType.AES_256.GetSymKeySizeAttribute()),
						EncryptionType.AES_GCM => CryptoAESGCM(data),
						EncryptionType.CHACHA20_POLY1305 => CryptoCHACHA20(data),
						_ => CryptoXCHACHA20(data),
					};
				}
			}
			catch (Exception) {}
			return (null, null, null);
		}
		public static byte[]? Decrypt(byte[]? data, byte[]? key = null, byte[]? iv = null, EncryptionType type = EncryptionType.XCHACHA20_POLY1305)
		{
			static byte[] CryptoAES128(byte[] data, byte[] key, byte[] iv)
			{
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.Key = key;
				aes.IV = iv;
				using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
				using MemoryStream mem_stream = new();
				using CryptoStream writer = new(mem_stream, transform, CryptoStreamMode.Write);
				writer.Write(data);
				writer.FlushFinalBlock();
				return mem_stream.ToArray();
			}
			static byte[] CryptoAES256(byte[] data, byte[] key, byte[] iv)
			{
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.Key = key;
				aes.IV = iv;
				using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
				using MemoryStream mem_stream = new();
				using CryptoStream writer = new(mem_stream, transform, CryptoStreamMode.Write);
				writer.Write(data);
				writer.FlushFinalBlock();
				return mem_stream.ToArray();
			}
			static byte[]? CryptoAESGCM(byte[] data, byte[] key, byte[] iv) { return SecretAeadAes.Decrypt(data, iv, key); }
			static byte[]? CryptoCHACHA20(byte[] data, byte[] key, byte[] iv) { return SecretAeadChaCha20Poly1305.Decrypt(data, iv, key); }
			static byte[]? CryptoXCHACHA20(byte[] data, byte[] key, byte[] iv) { return SecretAeadXChaCha20Poly1305.Decrypt(data, iv, key); }
			if (type == EncryptionType.None)
				return data;
			try
			{
				if (data != null && data.Length > 0 && key != null && key.Length == type.GetSymKeySizeAttribute() && iv != null && iv.Length == type.GetIVSizeAttribute())
				{
					return type switch
					{
						EncryptionType.AES_128 => CryptoAES128(data, key, iv),
						EncryptionType.AES_256 => CryptoAES256(data, key, iv),
						EncryptionType.AES_GCM => CryptoAESGCM(data, key, iv),
						EncryptionType.CHACHA20_POLY1305 => CryptoCHACHA20(data, key, iv),
						_ => CryptoXCHACHA20(data, key, iv)
					};
				}
			}
			catch (Exception) {}
			return null;
		}
		public static (bool[] Valid, CryptoKey[]? Wallets) WalletsCheck(string[]? wallets)
		{
			if (wallets == null)
				return ([], null);
			else if (wallets.Length < 1)
				return (Array.Empty<bool>(), Array.Empty<CryptoKey>());
			bool[] acpt = new bool[wallets.Length];
			List<CryptoKey> addrs = [];
			HashSet<string> dup = [];
			for (int i = 0; i < wallets.Length; i++)
			{
				(byte Network, byte[] PubKey)? wa = WalletToPubKey(wallets[i]);
				if (wa != null && Enum.IsDefined((WalletNetworks)wa.Value.Network) &&
					wa.Value.PubKey.Length == ((WalletNetworks)wa.Value.Network).GetPubKeySizeAttribute() && dup.Add(wallets[i]))
				{
					addrs.Add(new CryptoKey((WalletNetworks)wa.Value.Network, wa.Value.PubKey));
					acpt[i] = true;
				}
			}
			return (acpt, addrs.ToArray());
		}
	}
}
