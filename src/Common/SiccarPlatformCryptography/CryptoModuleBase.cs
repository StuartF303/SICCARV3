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

// CryptoModule Base Class Implementation - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

#nullable enable

namespace Siccar.Platform.Cryptography
{
	public abstract partial class CryptoModuleBase
	{
		public string? Wallet { get; init; }
		public string? HSMSlot { get; init; }
		public string? HSMPassword { get; init; }
		public AsymmetricMethod AsymmetricMethod { get; init; } = AsymmetricMethod.USE_SW_IMPL;
		public abstract (Status status, WalletUtils.KeySet? keyset) GenerateKeySet(WalletNetworks network, ref byte[]? data);
		public abstract (Status status, WalletUtils.KeySet? keyset) RecoverKeySet(WalletNetworks network, byte[] data, string? password);
		public abstract (Status status, byte[]? signature) Sign(byte[] hash, byte network, byte[] privkey);
		public abstract Status Verify(byte[] signature, byte[] hash, byte network, byte[] privkey);
		public abstract (Status status, byte[]? ciphertext) Encrypt(byte[] data, byte network, byte[] pubkey);
		public abstract (Status status, byte[]? plaintext) Decrypt(byte[] data, byte network, byte[] privkey);
		public abstract (Status status, byte[]? pubkey) CalculatePublicKey(byte network, byte[] privkey);
	}
}
