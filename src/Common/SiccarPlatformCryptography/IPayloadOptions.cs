// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

// Payload Options Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface IPayloadOptions
	{
		public EncryptionType EType { get; set; }				// Payload Data Symmetric Encryption type
		public CompressionType CType { get; set; }				// Payload Data Compression type
		public PayloadTypeField TType { get; set; }				// Payload System Identifier type
		public HashType HType { get; set; }						// Payload Hahing type
		public bool PType { get; set; }							// Payload Protected Flag type
		public bool ECType { get; set; }						// Payload Error Correcting Code type
		public UInt16 UType { get; set; }						// Payload User Defined type
	}
}
