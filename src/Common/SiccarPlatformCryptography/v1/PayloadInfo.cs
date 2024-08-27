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

// Transaction V1 PayloadInfo Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV1
{
	[Serializable]
	public class PayloadInfo : IPayloadInfo
	{
		private readonly bool PayloadEncrypted;
		private readonly UInt64 PayloadSize;
		private readonly byte[]? PayloadHash;
		private readonly string[] PayloadWallets;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked constructor")]
		private PayloadInfo(bool encrypted, UInt64 size, byte[]? hash, string[]? wallets)
		{
			PayloadEncrypted = encrypted;
			PayloadSize = size;
			PayloadHash = hash;
			PayloadWallets = wallets ?? [];
		}
		public bool GetPayloadEncrypted() { return PayloadEncrypted; }
		public bool GetPayloadCompressed() { return false; }
		public bool GetPayloadProtected() { return false; }
		public bool GetPayloadSparse() { return false; }
		public UInt64 GetPayloadSize() { return PayloadSize; }
		public byte[]? GetPayloadHash() { return PayloadHash; }
		public string[] GetPayloadWallets() { return PayloadWallets; }
		public UInt32 GetPayloadWalletsCount() { return (UInt32)PayloadWallets.Length; }
		public HashType GetPayloadHashType() { return HashType.SHA256; }
		public EncryptionType GetPayloadEncryptionType() { return PayloadEncrypted ? EncryptionType.AES_256 : EncryptionType.None; }
		public CompressionType GetPayloadCompressionType() { return CompressionType.None; }
		public PayloadTypeField GetPayloadTypeField() { return PayloadTypeField.Transaction; }
		public UInt16 GetPayloadUserField() { return 0; }
		public PayloadFlags GetPayloadFlags() { return 0; }
	}
}
