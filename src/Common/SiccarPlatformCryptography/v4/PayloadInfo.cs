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

// Transaction V4 PayloadInfo Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV4
{
	[Serializable]
	public class PayloadInfo : IPayloadInfo
	{
		private readonly PayloadFlags PayloadFlags;
		private readonly bool PayloadSparse;
		private readonly UInt64 PayloadSize;
		private readonly byte[]? PayloadHash;
		private readonly string[] PayloadWallets;
		private readonly HashType PayloadHashType;
		private readonly EncryptionType PayloadEncryptionType;
		private readonly CompressionType PayloadCompressionType;
		private readonly PayloadTypeField PayloadFieldType;
		private readonly UInt16 PayloadUserField;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked constructor")]
		private PayloadInfo(UInt64 size, byte[]? hash, string[]? wallets, UInt32 optflags, UInt32 typefields, bool sparse)
		{
			PayloadSize = size;
			PayloadHash = hash;
			PayloadWallets = wallets ?? [];
			PayloadFlags = (PayloadFlags)(optflags >> 16);
			PayloadHashType = (HashType)((optflags >> 10) & 0x1f);
			PayloadEncryptionType = (EncryptionType)((optflags >> 5) & 0x1f);
			PayloadCompressionType = (CompressionType)(optflags & 0x1f);
			PayloadFieldType = (PayloadTypeField)(typefields >> 16);
			PayloadUserField = (UInt16)(typefields & 0xffff);
			PayloadSparse = sparse;
		}
		public bool GetPayloadEncrypted() { return (PayloadFlags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION; }
		public bool GetPayloadCompressed() { return (PayloadFlags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION; }
		public bool GetPayloadProtected() { return (PayloadFlags & PayloadFlags.PAYLOAD_PROTECTED) == PayloadFlags.PAYLOAD_PROTECTED; }
		public bool GetPayloadSparse() { return PayloadSparse; }
		public UInt64 GetPayloadSize() { return PayloadSize; }
		public byte[]? GetPayloadHash() { return PayloadHash; }
		public string[] GetPayloadWallets() { return PayloadWallets; }
		public UInt32 GetPayloadWalletsCount() { return (UInt32)PayloadWallets.Length; }
		public HashType GetPayloadHashType() { return PayloadHashType; }
		public EncryptionType GetPayloadEncryptionType() { return GetPayloadEncrypted() ? PayloadEncryptionType : EncryptionType.None; }
		public CompressionType GetPayloadCompressionType() { return GetPayloadCompressed() ? PayloadCompressionType : CompressionType.None; }
		public PayloadTypeField GetPayloadTypeField() { return PayloadFieldType; }
		public UInt16 GetPayloadUserField() { return PayloadUserField; }
		public PayloadFlags GetPayloadFlags() { return PayloadFlags; }
	}
}

