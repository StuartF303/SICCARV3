// Payload Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface IPayload
	{
		public IPayloadInfo? GetInfo();
		public bool VerifyHash();
		public byte[]? DataHash();
		public UInt64 DataSize();
		public bool DataSparse();
		public bool DataProtected();
		public bool DataCompressed();
		public bool DataEncrypted();
	}
}
