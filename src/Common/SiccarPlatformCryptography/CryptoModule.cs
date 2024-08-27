// CryptoModule Implementation - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sodium;
#nullable enable
#pragma warning disable CS0219

namespace Siccar.Platform.Cryptography
{
	public sealed class CryptoModule : CryptoModuleBase
	{
		public override (Status status, WalletUtils.KeySet? keyset) GenerateKeySet(WalletNetworks network, ref byte[]? data)
		{
			try
			{
				switch (network)
				{
					case WalletNetworks.ED25519:
					{
						using KeyPair kp = PublicKeyAuth.GenerateKeyPair(data!);
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(network, kp.PrivateKey),
							PublicKey = new WalletUtils.CryptoKey(network, kp.PublicKey)
						});
					}
					case WalletNetworks.NISTP256:
					{
						ECDiffieHellman ec = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
						ECParameters p = ec.ExportParameters(true);
						List<byte> pk =
						[
							.. WalletUtils.VLEncode(p.Q.X!.Length),
							.. p.Q.X,
							.. WalletUtils.VLEncode(p.Q.Y!.Length),
							.. p.Q.Y,
						];
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(network, p.D),
							PublicKey = new WalletUtils.CryptoKey(network, [.. pk])
						});
					}
					case WalletNetworks.RSA4096:
					{
						RSA rsa = RSA.Create(4096);
						data = Encoding.ASCII.GetBytes(data != null && data.Length > 0 ?
							rsa.ExportEncryptedPkcs8PrivateKeyPem(Encoding.ASCII.GetString(data).AsSpan(), new(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 310000)) : rsa.ExportPkcs8PrivateKeyPem());
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(network, rsa.ExportRSAPrivateKey()),
							PublicKey = new WalletUtils.CryptoKey(network, rsa.ExportRSAPublicKey())
						});
					}
					default: { break; }
				}
			}
			catch (Exception) {}
			return (Status.KM_GENERATE_FAIL, null);
		}
		public override (Status status, WalletUtils.KeySet? keyset) RecoverKeySet(WalletNetworks network, byte[] data, string? password = null)
		{
			try
			{
				switch (network)
				{
					case WalletNetworks.ED25519:
					{
						using KeyPair kp = PublicKeyAuth.GenerateKeyPair(data);
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(network, kp.PrivateKey),
							PublicKey = new WalletUtils.CryptoKey(network, kp.PublicKey)
						});
					}
					case WalletNetworks.NISTP256:
					{
						ECDiffieHellman ec = ECDiffieHellmanCng.Create(new ECParameters { Curve = ECCurve.NamedCurves.nistP256, D = data.AsSpan(1, data.Length - 2).ToArray() });
						ECParameters p = ec.ExportParameters(false);
						List<byte> pk =
						[
							.. WalletUtils.VLEncode(p.Q.X!.Length),
							.. p.Q.X,
							.. WalletUtils.VLEncode(p.Q.Y!.Length),
							.. p.Q.Y,
						];
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(network, data.AsSpan(1, data.Length - 2).ToArray()),
							PublicKey = new WalletUtils.CryptoKey(network, [.. pk])
						});
					}
					case WalletNetworks.RSA4096:
					{
						RSA rsa = RSA.Create();
						if (password == null || password.Length < 1)
							rsa.ImportFromPem(Encoding.ASCII.GetString(data).AsSpan());
						else
						{
							try { rsa.ImportFromEncryptedPem(Encoding.ASCII.GetString(data).AsSpan(), password.AsSpan()); }
							catch (Exception) { return (Status.KM_PASSWORD_FAIL, null); }
						}
						return (Status.STATUS_OK, new WalletUtils.KeySet()
						{
							PrivateKey = new WalletUtils.CryptoKey(WalletNetworks.RSA4096, rsa.ExportRSAPrivateKey()),
							PublicKey = new WalletUtils.CryptoKey(WalletNetworks.RSA4096, rsa.ExportRSAPublicKey())
						});
					}
					default: { break; }
				}
			}
			catch (Exception) {}
			return (Status.KM_GENERATE_FAIL, null);
		}
		public override (Status status, byte[]? signature) Sign(byte[] hash, byte network, byte[] privkey)
		{
			static byte[]? RSASign(byte[] h, byte[] pk)
			{
				RSA rsa = RSA.Create(4096);
				rsa.ImportRSAPrivateKey(pk, out _);
				return rsa.SignHash(h, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
			}
			try
			{
				if (AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
				{
					return (WalletNetworks)network switch
					{
						WalletNetworks.ED25519 => (Status.STATUS_OK, PublicKeyAuth.SignDetached(hash, privkey)),
						WalletNetworks.NISTP256 => (Status.STATUS_OK, ECDsa.Create(new ECParameters() { Curve = ECCurve.NamedCurves.nistP256, D = privkey }).SignHash(hash)),
						WalletNetworks.RSA4096 => (Status.STATUS_OK, RSASign(hash, privkey)),
						_ => (Status.TX_SIGNING_FAILURE, null)
					};
				}
				string hsm_path = "hsm://azure.security.portals";
				string? hsm_slot = HSMSlot;
				string? hsm_password = HSMPassword;
			}
			catch (Exception) {}
			return (Status.TX_SIGNING_FAILURE, null);
		}
		public override Status Verify(byte[] signature, byte[] hash, byte network, byte[] pubkey)
		{
			static bool ECVerify(byte[] s, byte[] h, byte[] pk)
			{
				BinaryReader reader = new(new MemoryStream(pk));
				return ECDsa.Create(new ECParameters() { Curve = ECCurve.NamedCurves.nistP256, Q = new ECPoint() { X = WalletUtils.ReadVLArray(reader), Y = WalletUtils.ReadVLArray(reader) } }).VerifyHash(h, s);
			}
			static bool RSAVerify(byte[] s, byte[] h, byte[] pk)
			{
				RSA rsa = RSA.Create(4096);
				rsa.ImportRSAPublicKey(pk, out _);
				return rsa.VerifyHash(h, s, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
			}
			try
			{
				if (AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
				{
					return (WalletNetworks)network switch
					{
						WalletNetworks.ED25519 => PublicKeyAuth.VerifyDetached(signature, hash, pubkey) ? Status.STATUS_OK : Status.TX_BAD_SIGNATURE,
						WalletNetworks.NISTP256 => ECVerify(signature, hash, pubkey) ? Status.STATUS_OK : Status.TX_BAD_SIGNATURE,
						WalletNetworks.RSA4096 => RSAVerify(signature, hash, pubkey) ? Status.STATUS_OK : Status.TX_BAD_SIGNATURE,
						_ => Status.TX_SIGNING_FAILURE
					};
				}
				string hsm_path = "hsm://azure.security.portals";
				string? hsm_slot = HSMSlot;
				string? hsm_password = HSMPassword;
			}
			catch (Exception) {}
			return Status.TX_BAD_SIGNATURE;
		}
		public override (Status status, byte[]? ciphertext) Encrypt(byte[] data, byte network, byte[] pubkey)
		{
			static byte[] ECEncrypt(byte[] d, byte[] pk)
			{
				ECDiffieHellman dhc = ECDiffieHellman.Create();
				BinaryReader reader = new(new MemoryStream(pk));
				dhc.ImportParameters(new ECParameters() { Curve = ECCurve.NamedCurves.nistP256, Q = new ECPoint() { X = WalletUtils.ReadVLArray(reader), Y = WalletUtils.ReadVLArray(reader) } });
				ECDiffieHellman ekc = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
				byte[] dk = ekc.DeriveKeyFromHash(dhc.PublicKey, HashAlgorithmName.SHA256);
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.KeySize = dk.Length << 3;
				aes.Key = dk;
				aes.GenerateIV();
				using ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
				using MemoryStream stream = new();
				using CryptoStream writer = new(stream, transform, CryptoStreamMode.Write);
				writer.Write(d);
				writer.FlushFinalBlock();
				byte[] ed = stream.ToArray();
				List<byte> fd = [.. ekc.ExportSubjectPublicKeyInfo()];
				fd.InsertRange(0, WalletUtils.VLEncode(fd.Count));
				fd.AddRange(WalletUtils.VLEncode(aes.IV.Length));
				fd.AddRange(aes.IV);
				fd.AddRange(WalletUtils.VLEncode(ed.Length));
				fd.AddRange(ed);
				return [.. fd];
			}
			static byte[] RSAEncrypt(byte[] d, byte[] pk)
			{
				RSA rsa = RSA.Create(4096);
				rsa.ImportRSAPublicKey(pk, out _);
				return rsa.Encrypt(d, RSAEncryptionPadding.OaepSHA256);
			}
			try
			{
				if (AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
				{
					return (WalletNetworks)network switch
					{
						WalletNetworks.ED25519 => (Status.STATUS_OK, SealedPublicKeyBox.Create(data, PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(pubkey))),
						WalletNetworks.NISTP256 => (Status.STATUS_OK, ECEncrypt(data, pubkey)),
						WalletNetworks.RSA4096 => (Status.STATUS_OK, RSAEncrypt(data, pubkey)),
						_ => (Status.TX_BAD_CRYPTOBOX, null)
					};
				}
				string hsm_path = "hsm://azure.security.portals";
				string? hsm_slot = HSMSlot;
				string? hsm_password = HSMPassword;
			}
			catch (Exception) {}
			return (Status.TX_BAD_CRYPTOBOX, null);
		}
		public override (Status status, byte[]? plaintext) Decrypt(byte[] data, byte network, byte[] privkey)
		{
			static byte[] ED25519Decrypt(byte[] d, byte[] pk)
			{
				using KeyPair pair = new(PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(PublicKeyAuth.ExtractEd25519PublicKeyFromEd25519SecretKey(pk)),
					PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(pk));
				return SealedPublicKeyBox.Open(d, pair);
			}
			static byte[] ECDecrypt(byte[] d, byte[] pk)
			{
				ECDiffieHellman ekc = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
				BinaryReader reader = new(new MemoryStream(d));
				ekc.ImportSubjectPublicKeyInfo(WalletUtils.ReadVLArray(reader), out _);
				ECDiffieHellman dhc = ECDiffieHellman.Create();
				dhc.ImportParameters(new ECParameters() { Curve = ECCurve.NamedCurves.nistP256, D = pk });
				byte[] dk = dhc.DeriveKeyFromHash(ekc.PublicKey, HashAlgorithmName.SHA256);
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.KeySize = dk.Length << 3;
				aes.Key = dk;
				aes.IV = WalletUtils.ReadVLArray(reader);
				using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
				using MemoryStream stream = new();
				using CryptoStream writer = new(stream, transform, CryptoStreamMode.Write);
				writer.Write(WalletUtils.ReadVLArray(reader));
				writer.FlushFinalBlock();
				return stream.ToArray();
			}
			static byte[] RSADecrypt(byte[] d, byte[] pk)
			{
				RSA rsa = RSA.Create(4096);
				rsa.ImportRSAPrivateKey(pk, out _);
				return rsa.Decrypt(d, RSAEncryptionPadding.OaepSHA256);
			}
			try
			{
				if (AsymmetricMethod == AsymmetricMethod.USE_SW_IMPL)
				{
					return (WalletNetworks)network switch
					{
						WalletNetworks.ED25519 => (Status.STATUS_OK, ED25519Decrypt(data, privkey)),
						WalletNetworks.NISTP256 => (Status.STATUS_OK, ECDecrypt(data, privkey)),
						WalletNetworks.RSA4096 => (Status.STATUS_OK, RSADecrypt(data, privkey)),
						_ => (Status.TX_BAD_CRYPTOBOX, null)
					};
				}
				string? hsm_slot = HSMSlot;
				string? hsm_password = HSMPassword;
			}
			catch (Exception) {}
			return (Status.TX_BAD_CRYPTOBOX, null);
		}
		public override (Status status, byte[]? pubkey) CalculatePublicKey(byte network, byte[] privkey)
		{
			static byte[] ECGetPublicKey(byte[] pk)
			{
				ECDiffieHellman dhc = ECDiffieHellman.Create();
				dhc.ImportParameters(new ECParameters() { Curve = ECCurve.NamedCurves.nistP256, D = pk });
				ECParameters p = dhc.ExportParameters(false);
				List<byte> ed =
				[
					.. WalletUtils.VLEncode(p.Q.X!.Length),
					.. p.Q.X,
					.. WalletUtils.VLEncode(p.Q.Y!.Length),
					.. p.Q.Y,
				];
				return [.. ed];
			}
			static byte[] RSAGetPublicKey(byte[] pk)
			{
				RSA rsa = RSA.Create(4096);
				rsa.ImportRSAPrivateKey(pk, out _);
				return rsa.ExportRSAPublicKey();
			}
			try
			{
				return (WalletNetworks)network switch
				{
					WalletNetworks.ED25519 => (Status.STATUS_OK, PublicKeyAuth.ExtractEd25519PublicKeyFromEd25519SecretKey(privkey)),
					WalletNetworks.NISTP256 => (Status.STATUS_OK, ECGetPublicKey(privkey)),
					WalletNetworks.RSA4096 => (Status.STATUS_OK, RSAGetPublicKey(privkey)),
					_ => (Status.TX_INVALID_KEY, null)
				};
			}
			catch (Exception) { return (Status.TX_INVALID_KEY, null); }
		}
	}
}
