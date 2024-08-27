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
