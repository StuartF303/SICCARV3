// PayloadInfo Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface IPayloadInfo
	{
		public bool GetPayloadEncrypted();
		public bool GetPayloadCompressed();
		public bool GetPayloadProtected();
		public bool GetPayloadSparse();
		public UInt64 GetPayloadSize();
		public byte[]? GetPayloadHash();
		public string[] GetPayloadWallets();
		public UInt32 GetPayloadWalletsCount();
		public HashType GetPayloadHashType();
		public EncryptionType GetPayloadEncryptionType();
		public CompressionType GetPayloadCompressionType();
		public PayloadTypeField GetPayloadTypeField();
		public UInt16 GetPayloadUserField();
		public PayloadFlags GetPayloadFlags();
	}
}