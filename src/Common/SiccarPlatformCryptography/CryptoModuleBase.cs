// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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
