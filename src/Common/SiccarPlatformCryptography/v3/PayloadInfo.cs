// Transaction V3 PayloadInfo Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV3
{
	[Serializable]
	public class PayloadInfo : IPayloadInfo
	{
		private readonly PayloadFlags PayloadFlags;
		private readonly UInt64 PayloadSize;
		private readonly byte[]? PayloadHash;
		private readonly string[] PayloadWallets;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked constructor")]
		private PayloadInfo(UInt64 size, byte[]? hash, string[]? wallets, PayloadFlags flags)
		{
			PayloadSize = size;
			PayloadHash = hash;
			PayloadWallets = wallets ?? [];
			PayloadFlags = flags;
		}
		public bool GetPayloadEncrypted() { return (PayloadFlags & PayloadFlags.PAYLOAD_ENCRYPTION) == PayloadFlags.PAYLOAD_ENCRYPTION; }
		public bool GetPayloadCompressed() { return (PayloadFlags & PayloadFlags.PAYLOAD_COMPRESSION) == PayloadFlags.PAYLOAD_COMPRESSION; }
		public bool GetPayloadProtected() { return false; }
		public bool GetPayloadSparse() { return false; }
		public UInt64 GetPayloadSize() { return PayloadSize; }
		public byte[]? GetPayloadHash() { return PayloadHash; }
		public string[] GetPayloadWallets() { return PayloadWallets; }
		public UInt32 GetPayloadWalletsCount() { return (UInt32)PayloadWallets.Length; }
		public HashType GetPayloadHashType() { return HashType.SHA256; }
		public EncryptionType GetPayloadEncryptionType() { return GetPayloadEncrypted() ? EncryptionType.AES_256 : EncryptionType.None; }
		public CompressionType GetPayloadCompressionType() { return GetPayloadCompressed() ? CompressionType.Max : CompressionType.None; }
		public PayloadTypeField GetPayloadTypeField() { return PayloadTypeField.Transaction; }
		public UInt16 GetPayloadUserField() { return 0; }
		public PayloadFlags GetPayloadFlags() { return 0; }
	}
}

