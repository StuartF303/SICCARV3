// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

// KeyManager Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class KeyManagerTests
	{
		private readonly KeyManager manager = new();
		private readonly (Status status, KeyManager.KeyRing? keyring) kr1;
		private readonly (Status status, KeyManager.KeyRing? keyring) kr2;
		private readonly (Status status, KeyManager.KeyRing? keyring) kr3;
		private readonly (Status status, KeyManager.KeyRing? keyring) rk1;
		private readonly (Status status, KeyManager.KeyRing? keyring) rk2;
		private readonly (Status status, KeyManager.KeyRing? keyring) rk3;

		public KeyManagerTests()
		{
			var task1 = Task.Run(() => manager.CreateMasterKeyRing(WalletNetworks.ED25519));
			var task2 = Task.Run(() => manager.CreateMasterKeyRing(WalletNetworks.NISTP256));
			var task3 = Task.Run(() => manager.CreateMasterKeyRing(WalletNetworks.RSA4096));
			task1.Wait();
			kr1 = task1.Result;
			task1 = Task.Run(() => manager.RecoverMasterKeyRing(kr1.keyring?.Mnemonic()));
			task2.Wait();
			kr2 = task2.Result;
			task2 = Task.Run(() => manager.RecoverMasterKeyRing(kr2.keyring?.Mnemonic()));
			task3.Wait();
			kr3 = task3.Result;
			task3 = Task.Run(() => manager.RecoverMasterKeyRing(kr3.keyring?.Mnemonic()));
			task1.Wait();
			rk1 = task1.Result;
			task2.Wait();
			rk2 = task2.Result;
			task3.Wait();
			rk3 = task3.Result;
		}

/** CreateMasterKeyRing Tests **/
		[Fact]
		public void CreateMasterKeyRingResult()
		{
			Assert.Equal(Status.STATUS_OK, kr1.status);
			Assert.Equal(Status.STATUS_OK, kr2.status);
			Assert.Equal(Status.STATUS_OK, kr3.status);
		}
		[Fact]
		public void CreateMasterKeyRingNonNullKeys()
		{
			Assert.NotNull(kr1.keyring);
			Assert.NotNull(kr1.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(kr1.keyring.KeySet().PublicKey.Key!);
			Assert.NotNull(kr1.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(kr1.keyring.KeySet().PrivateKey.Key!);
			Assert.NotNull(kr2.keyring);
			Assert.NotNull(kr2.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(kr2.keyring.KeySet().PublicKey.Key!);
			Assert.NotNull(kr2.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(kr2.keyring.KeySet().PrivateKey.Key!);
			Assert.NotNull(kr3.keyring);
			Assert.NotNull(kr3.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(kr3.keyring.KeySet().PublicKey.Key!);
			Assert.NotNull(kr3.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(kr3.keyring.KeySet().PrivateKey.Key!);
		}
		[Fact]
		public void CreateMasterKeyRingPublicKeySize()
		{
			Assert.Equal(kr1.keyring?.KeySet().PublicKey.Key?.Length, kr1.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
			Assert.Equal(kr2.keyring?.KeySet().PublicKey.Key?.Length, kr2.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
			Assert.Equal(kr3.keyring?.KeySet().PublicKey.Key?.Length, kr3.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
		}
		[Fact]
		public void CreateMasterKeyRingKeyNetworks()
		{
			Assert.Equal(WalletNetworks.ED25519, kr1.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.ED25519, kr1.keyring?.KeySet().PrivateKey.Network);
			Assert.Equal(WalletNetworks.NISTP256, kr2.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.NISTP256, kr2.keyring?.KeySet().PrivateKey.Network);
			Assert.Equal(WalletNetworks.RSA4096, kr3.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.RSA4096, kr3.keyring?.KeySet().PrivateKey.Network);
		}
		[Fact]
		public void CreateMasterKeyRingWIFTests()
		{
			Assert.NotNull(kr1.keyring?.WIFKey());
			Assert.NotEmpty(kr1.keyring?.WIFKey()!);
			(byte Network, byte[] PrivateKey)? kd = WalletUtils.WIFToPrivKey(kr1.keyring?.WIFKey());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PrivateKey, kr1.keyring?.KeySet().PrivateKey.Key));
			Assert.Equal(WalletNetworks.ED25519, (WalletNetworks)kd.Value.Network);
			Assert.NotNull(kr2.keyring?.WIFKey());
			Assert.NotEmpty(kr2.keyring?.WIFKey()!);
			kd = WalletUtils.WIFToPrivKey(kr2.keyring?.WIFKey());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PrivateKey, kr2.keyring?.KeySet().PrivateKey.Key));
			Assert.Equal(WalletNetworks.NISTP256, (WalletNetworks)kd.Value.Network);
			Assert.NotNull(kr3.keyring?.WIFKey());
			Assert.NotEmpty(kr3.keyring?.WIFKey()!);
			kd = WalletUtils.WIFToPrivKey(kr3.keyring?.WIFKey());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PrivateKey, kr3.keyring?.KeySet().PrivateKey.Key));
			Assert.Equal(WalletNetworks.RSA4096, (WalletNetworks)kd.Value.Network);
		}
		[Fact]
		public void CreateMasterKeyRingWalletTests()
		{
			Assert.NotNull(kr1.keyring?.Wallet());
			Assert.NotEmpty(kr1.keyring?.Wallet()!);
			(byte Network, byte[] PubKey)? kd = WalletUtils.WalletToPubKey(kr1.keyring?.Wallet());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PubKey, kr1.keyring?.KeySet().PublicKey.Key));
			Assert.Equal(WalletNetworks.ED25519, (WalletNetworks)kd.Value.Network);
			Assert.True(kr1.keyring?.Wallet()?.StartsWith("ws1"));
			Assert.NotNull(kr2.keyring?.Wallet());
			Assert.NotEmpty(kr2.keyring?.Wallet()!);
			kd = WalletUtils.WalletToPubKey(kr2.keyring?.Wallet());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PubKey, kr2.keyring?.KeySet().PublicKey.Key));
			Assert.Equal(WalletNetworks.NISTP256, (WalletNetworks)kd.Value.Network);
			Assert.True(kr2.keyring?.Wallet()?.StartsWith("ws1"));
			Assert.NotNull(kr3.keyring?.Wallet());
			Assert.NotEmpty(kr3.keyring?.Wallet()!);
			kd = WalletUtils.WalletToPubKey(kr3.keyring?.Wallet());
			Assert.NotNull(kd);
			Assert.True(ByteArrayCompare(kd!.Value.PubKey, kr3.keyring?.KeySet().PublicKey.Key));
			Assert.Equal(WalletNetworks.RSA4096, (WalletNetworks)kd.Value.Network);
			Assert.True(kr3.keyring?.Wallet()?.StartsWith("ws1"));
		}
		[Fact]
		public void CreateMasterKeyRingMnemonicTests()
		{
			Assert.NotNull(kr1.keyring?.Mnemonic());
			Assert.NotEmpty(kr1.keyring?.Mnemonic()!);
			Assert.Equal(25, kr1.keyring?.MnemonicList().Length);
			Assert.NotNull(kr2.keyring?.Mnemonic());
			Assert.NotEmpty(kr2.keyring?.Mnemonic()!);
			Assert.Equal(25, kr2.keyring?.MnemonicList().Length);
			Assert.NotNull(kr3.keyring?.Mnemonic());
			Assert.NotEmpty(kr3.keyring?.Mnemonic()!);
			Assert.True(kr3.keyring?.Mnemonic().StartsWith("-----BEGIN PRIVATE KEY-----"));
			Assert.True(kr3.keyring?.Mnemonic().EndsWith("-----END PRIVATE KEY-----"));
		}

/** RecoverMasterKeyRing Tests **/
		[Fact]
		public void RecoverMasterKeyRingResult()
		{
			Assert.Equal(Status.STATUS_OK, rk1.status);
			Assert.Equal(Status.STATUS_OK, rk2.status);
			Assert.Equal(Status.STATUS_OK, rk3.status);
		}
		[Fact]
		public void RecoverMasterKeyRingNonNullKeys()
		{
			Assert.NotNull(rk1.keyring);
			Assert.NotNull(rk1.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(rk1.keyring.KeySet().PublicKey.Key!);
			Assert.True(ByteArrayCompare(rk1.keyring.KeySet().PublicKey.Key, kr1.keyring?.KeySet().PublicKey.Key));
			Assert.NotNull(rk1.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(rk1.keyring.KeySet().PrivateKey.Key!);
			Assert.True(ByteArrayCompare(rk1.keyring.KeySet().PrivateKey.Key, kr1.keyring?.KeySet().PrivateKey.Key));
			Assert.NotNull(rk2.keyring);
			Assert.NotNull(rk2.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(rk2.keyring.KeySet().PublicKey.Key!);
			Assert.True(ByteArrayCompare(rk2.keyring.KeySet().PublicKey.Key, kr2.keyring?.KeySet().PublicKey.Key));
			Assert.NotNull(rk2.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(rk2.keyring.KeySet().PrivateKey.Key!);
			Assert.True(ByteArrayCompare(rk2.keyring.KeySet().PrivateKey.Key, kr2.keyring?.KeySet().PrivateKey.Key));
			Assert.NotNull(rk3.keyring);
			Assert.NotNull(rk3.keyring.KeySet().PublicKey.Key);
			Assert.NotEmpty(rk3.keyring.KeySet().PublicKey.Key!);
			Assert.True(ByteArrayCompare(rk3.keyring.KeySet().PublicKey.Key, kr3.keyring?.KeySet().PublicKey.Key));
			Assert.NotNull(rk3.keyring.KeySet().PrivateKey.Key);
			Assert.NotEmpty(rk3.keyring.KeySet().PrivateKey.Key!);
			Assert.True(ByteArrayCompare(rk3.keyring.KeySet().PrivateKey.Key, kr3.keyring?.KeySet().PrivateKey.Key));
		}
		[Fact]
		public void RecoverMasterKeyRingPublicKeySize()
		{
			Assert.Equal(rk1.keyring?.KeySet().PublicKey.Key?.Length, rk1.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
			Assert.Equal(rk2.keyring?.KeySet().PublicKey.Key?.Length, rk2.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
			Assert.Equal(rk3.keyring?.KeySet().PublicKey.Key?.Length, rk3.keyring?.KeySet().PublicKey.Network.GetPubKeySizeAttribute());
		}
		[Fact]
		public void RecoverMasterKeyRingKeyNetworks()
		{
			Assert.Equal(WalletNetworks.ED25519, rk1.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.ED25519, rk1.keyring?.KeySet().PrivateKey.Network);
			Assert.Equal(WalletNetworks.NISTP256, rk2.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.NISTP256, rk2.keyring?.KeySet().PrivateKey.Network);
			Assert.Equal(WalletNetworks.RSA4096, rk3.keyring?.KeySet().PublicKey.Network);
			Assert.Equal(WalletNetworks.RSA4096, rk3.keyring?.KeySet().PrivateKey.Network);
		}
		[Fact]
		public void RecoverMasterKeyRingWIFTests()
		{
			Assert.NotNull(rk1.keyring?.WIFKey());
			Assert.NotEmpty(rk1.keyring?.WIFKey()!);
			Assert.Equal(rk1.keyring?.WIFKey(), kr1.keyring?.WIFKey());
			Assert.NotNull(rk2.keyring?.WIFKey());
			Assert.NotEmpty(rk2.keyring?.WIFKey()!);
			Assert.Equal(rk2.keyring?.WIFKey(), kr2.keyring?.WIFKey());
			Assert.NotNull(rk3.keyring?.WIFKey());
			Assert.NotEmpty(rk3.keyring?.WIFKey()!);
			Assert.Equal(rk3.keyring?.WIFKey(), kr3.keyring?.WIFKey());
		}
		[Fact]
		public void RecoverMasterKeyRingWalletTests()
		{
			Assert.NotNull(kr1.keyring?.Wallet());
			Assert.NotEmpty(kr1.keyring?.Wallet()!);
			Assert.Equal(rk1.keyring?.Wallet(), kr1.keyring?.Wallet());
			Assert.NotNull(kr2.keyring?.Wallet());
			Assert.NotEmpty(kr2.keyring?.Wallet()!);
			Assert.Equal(rk2.keyring?.Wallet(), kr2.keyring?.Wallet());
			Assert.NotNull(kr3.keyring?.Wallet());
			Assert.NotEmpty(kr3.keyring?.Wallet()!);
			Assert.Equal(rk3.keyring?.Wallet(), kr3.keyring?.Wallet());
		}
		[Fact]
		public void RecoverMasterKeyRingMnemonicTests()
		{
			Assert.NotNull(kr1.keyring?.Mnemonic());
			Assert.NotEmpty(kr1.keyring?.Mnemonic()!);
			Assert.Equal(rk1.keyring?.Mnemonic(), kr1.keyring?.Mnemonic());
			Assert.NotNull(kr2.keyring?.Mnemonic());
			Assert.NotEmpty(kr2.keyring?.Mnemonic()!);
			Assert.Equal(rk2.keyring?.Mnemonic(), kr2.keyring?.Mnemonic());
			Assert.NotNull(kr3.keyring?.Mnemonic());
			Assert.NotEmpty(kr3.keyring?.Mnemonic()!);
			Assert.Equal(rk3.keyring?.Mnemonic(), kr3.keyring?.Mnemonic());
		}

/** KeyChain Tests **/
		[Fact]
		public void AddKeyRingNullNameKeyRing()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.KM_BAD_KEYRING, chain.AddKeyRing(null!, null!));
			Assert.Equal(Status.KM_BAD_KEYRING, chain.AddKeyRing("Default", null!));
			Assert.Equal(Status.KM_BAD_KEYRING, chain.AddKeyRing(null!, kr1.keyring!));
		}
		[Fact]
		public void AddKeyRingSingle()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Default");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keyring);
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PrivateKey.Key, keyring.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PublicKey.Key, keyring.KeySet().PublicKey.Key));
			Assert.Equal(kr1.keyring?.WIFKey(), keyring.WIFKey());
			Assert.Equal(kr1.keyring?.Wallet(), keyring.Wallet());
			Assert.Equal(kr1.keyring?.Mnemonic(), keyring.Mnemonic());
		}
		[Fact]
		public void AddKeyRingDuplicateKeyRing()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			Assert.Equal(Status.KM_DUPLICATE_KEYRING, chain.AddKeyRing("Default", kr1.keyring!));
			Assert.Equal(Status.KM_DUPLICATE_KEYRING, chain.AddKeyRing("Default", kr2.keyring!));
		}
		[Fact]
		public void AddKeyRingMultipleTypes()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_1", kr1.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_2", kr2.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_3", kr3.keyring!));
		}
		[Fact]
		public void GetKeyRingEmpty()
		{
			KeyManager.KeyChain chain = new();
			(Status status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Default");
			Assert.Equal(Status.KM_EMPTY_KEYCHAIN, status);
			Assert.Null(keyring);
		}
		[Fact]
		public void GetKeyRingUnknown()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Type_1");
			Assert.Equal(Status.KM_UNKNOWN_KEYRING, status);
			Assert.Null(keyring);
		}
		[Fact]
		public void GetKeyRingSingle()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Default");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keyring);
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PrivateKey.Key, keyring.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PublicKey.Key, keyring.KeySet().PublicKey.Key));
			Assert.Equal(kr1.keyring?.WIFKey(), keyring.WIFKey());
			Assert.Equal(kr1.keyring?.Wallet(), keyring.Wallet());
			Assert.Equal(kr1.keyring?.Mnemonic(), keyring.Mnemonic());
		}
		[Fact]
		public void GetKeyRingMultiple()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_1", kr1.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_2", kr2.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Type_3", kr3.keyring!));
			(Status status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Type_1");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keyring);
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PrivateKey.Key, keyring.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PublicKey.Key, keyring.KeySet().PublicKey.Key));
			Assert.Equal(kr1.keyring?.WIFKey(), keyring.WIFKey());
			Assert.Equal(kr1.keyring?.Wallet(), keyring.Wallet());
			Assert.Equal(kr1.keyring?.Mnemonic(), keyring.Mnemonic());
			(status, keyring) = chain.GetKeyRing("Type_2");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keyring);
			Assert.True(ByteArrayCompare(kr2.keyring?.KeySet().PrivateKey.Key, keyring.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr2.keyring?.KeySet().PublicKey.Key, keyring.KeySet().PublicKey.Key));
			Assert.Equal(kr2.keyring?.WIFKey(), keyring.WIFKey());
			Assert.Equal(kr2.keyring?.Wallet(), keyring.Wallet());
			Assert.Equal(kr2.keyring?.Mnemonic(), keyring.Mnemonic());
			(status, keyring) = chain.GetKeyRing("Type_3");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keyring);
			Assert.True(ByteArrayCompare(kr3.keyring?.KeySet().PrivateKey.Key, keyring.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr3.keyring?.KeySet().PublicKey.Key, keyring.KeySet().PublicKey.Key));
			Assert.Equal(kr3.keyring?.WIFKey(), keyring.WIFKey());
			Assert.Equal(kr3.keyring?.Wallet(), keyring.Wallet());
			Assert.Equal(kr3.keyring?.Mnemonic(), keyring.Mnemonic());
		}
		[Fact]
		public void RemoveKeyRingNullEmptyKeyRing()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.KM_BAD_KEYRING, chain.RemoveKeyRing(null!));
			Assert.Equal(Status.KM_EMPTY_KEYCHAIN, chain.RemoveKeyRing("Default"));
		}
		[Fact]
		public void RemoveKeyRingUnknownKeyRing()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			Assert.Equal(Status.KM_UNKNOWN_KEYRING, chain.RemoveKeyRing("Test"));
		}
		[Fact]
		public void RemoveKeyRingSingle()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_1", kr2.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.RemoveKeyRing("Default"));
			Assert.Equal(Status.KM_UNKNOWN_KEYRING, chain.RemoveKeyRing("Default"));
			Assert.Equal(Status.STATUS_OK, chain.RemoveKeyRing("Test_1"));
			Assert.Equal(Status.KM_EMPTY_KEYCHAIN, chain.RemoveKeyRing("Test_1"));
		}
		[Fact]
		public void ExportNullPassword()
		{
			KeyManager.KeyChain chain = new();
			(Status status, byte[]? keychain) = chain.Export(null!);
			Assert.Equal(Status.KM_PASSWORD_FAIL, status);
			Assert.Null(keychain);
		}
		[Fact]
		public void ExportEmptyKeyChain()
		{
			KeyManager.KeyChain chain = new();
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.KM_EMPTY_KEYCHAIN, status);
			Assert.Null(keychain);
		}
		[Fact]
		public void ExportMultiKeyRings()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_1", kr1.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_2", kr2.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_3", kr3.keyring!));
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keychain);
			Assert.NotEmpty(keychain);
		}
		[Fact]
		public void ImportNullKeyChain()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.KM_CRYPTO_FAILURE, chain.Import(null!, "TestPassword1!"));
		}
		[Fact]
		public void ImportNullPassword()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keychain);
			Assert.NotEmpty(keychain);
			Assert.Equal(Status.KM_PASSWORD_FAIL, chain.Import(keychain, null!));
		}
		[Fact]
		public void ImportBadPassword()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keychain);
			Assert.NotEmpty(keychain);
			Assert.Equal(Status.KM_PASSWORD_FAIL, chain.Import(keychain, "TestPassword1"));
		}
		[Fact]
		public void ImportSingleBelowCompressionSize()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Default", kr1.keyring!));
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keychain);
			Assert.NotEmpty(keychain);
			Assert.Equal(Status.STATUS_OK, chain.Import(keychain, "TestPassword1!"));
			(status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Default");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PrivateKey.Key, keyring?.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PublicKey.Key, keyring?.KeySet().PublicKey.Key));
			Assert.Equal(kr1.keyring?.WIFKey(), keyring?.WIFKey());
			Assert.Equal(kr1.keyring?.Wallet(), keyring?.Wallet());
			Assert.Equal(kr1.keyring?.Mnemonic(), keyring?.Mnemonic());
		}
		[Fact]
		public void ImportMultipleWithCompression()
		{
			KeyManager.KeyChain chain = new();
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_1", kr1.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_2", kr2.keyring!));
			Assert.Equal(Status.STATUS_OK, chain.AddKeyRing("Test_3", kr3.keyring!));
			(Status status, byte[]? keychain) = chain.Export("TestPassword1!");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(keychain);
			Assert.NotEmpty(keychain);
			Assert.Equal(Status.STATUS_OK, chain.Import(keychain, "TestPassword1!"));
			(status, KeyManager.KeyRing? keyring) = chain.GetKeyRing("Test_1");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PrivateKey.Key, keyring?.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr1.keyring?.KeySet().PublicKey.Key, keyring?.KeySet().PublicKey.Key));
			Assert.Equal(kr1.keyring?.WIFKey(), keyring?.WIFKey());
			Assert.Equal(kr1.keyring?.Wallet(), keyring?.Wallet());
			Assert.Equal(kr1.keyring?.Mnemonic(), keyring?.Mnemonic());
			(status, keyring) = chain.GetKeyRing("Test_2");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(ByteArrayCompare(kr2.keyring?.KeySet().PrivateKey.Key, keyring?.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr2.keyring?.KeySet().PublicKey.Key, keyring?.KeySet().PublicKey.Key));
			Assert.Equal(kr2.keyring?.WIFKey(), keyring?.WIFKey());
			Assert.Equal(kr2.keyring?.Wallet(), keyring?.Wallet());
			Assert.Equal(kr2.keyring?.Mnemonic(), keyring?.Mnemonic());
			(status, keyring) = chain.GetKeyRing("Test_3");
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(ByteArrayCompare(kr3.keyring?.KeySet().PrivateKey.Key, keyring?.KeySet().PrivateKey.Key));
			Assert.True(ByteArrayCompare(kr3.keyring?.KeySet().PublicKey.Key, keyring?.KeySet().PublicKey.Key));
			Assert.Equal(kr3.keyring?.WIFKey(), keyring?.WIFKey());
			Assert.Equal(kr3.keyring?.Wallet(), keyring?.Wallet());
			Assert.Equal(kr3.keyring?.Mnemonic(), keyring?.Mnemonic());
		}
		private static bool ByteArrayCompare(byte[]? a, byte[]? b) { return a?.Length == b?.Length && ((ReadOnlySpan<byte>)a).SequenceEqual(b); }
	}
}