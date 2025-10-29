// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

// Payload Options V4 File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV4
{
	public class PayloadOptions : IPayloadOptions
	{
		public EncryptionType EType { get; set; } = EncryptionType.XCHACHA20_POLY1305;
		public CompressionType CType { get; set; } = CompressionType.Balanced;
		public PayloadTypeField TType { get; set; } = PayloadTypeField.Transaction;
		public HashType HType { get; set; } = HashType.Blake2b_256;
		public bool PType { get; set; } = false;
		public bool ECType { get; set; } = false;
		public UInt16 UType { get; set; } = 0;
	}
}
