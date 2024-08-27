// Payload Options V3 File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV3
{
	public class PayloadOptions : IPayloadOptions
	{
		public EncryptionType EType { get; set; } = EncryptionType.AES_256;
		public CompressionType CType { get; set; } = CompressionType.Balanced;
		public PayloadTypeField TType { get; set; } = PayloadTypeField.Transaction;
		public HashType HType { get; set; } = HashType.SHA256;
		public bool PType { get; set; } = false;
		public bool ECType { get; set; } = false;
		public UInt16 UType { get; set; } = 0;
	}
}
