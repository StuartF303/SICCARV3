// PayloadManager V4 Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Collections.Generic;
using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class TransactionV4PManagerTests
	{
		private readonly string sender_pkey = "BpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";

/* GetPayloadsCount() Tests */
		[Fact]
		public void GetPayloadsCountEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Assert.Equal((UInt32)0, tx.GetTxPayloadManager().GetPayloadsCount());
		}
		[Fact]
		public void GetPayloadsCountOnePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal((UInt32)1, tx.GetTxPayloadManager().GetPayloadsCount());
		}
		[Fact]
		public void GetPayloadsCountMultiPayload()
		{
			for (int i = 0; i < 10; i++)
			{
				Random rnd = new();
				int count = rnd.Next(1, 10);
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
				IPayloadManager manager = tx.GetTxPayloadManager();
				for (int j = 0; j < count; j++)
					manager.AddPayload(new byte[] { 0x00 });
				Assert.Equal(count, (int)manager.GetPayloadsCount());
			}
		}
		[Fact]
		public void GetPayloadsCountAfterBinBuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			Assert.Equal(1, (int)transaction.GetTxPayloadManager().GetPayloadsCount());
		}

/* GetPayloadsInfo() Tests */
		[Fact]
		public void GetPayloadsInfoNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetPayloadsInfoOnePayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
		}
		[Fact]
		public void GetPayloadsInfoMultiPayloads()
		{
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
				Random rnd = new();
				int count = rnd.Next(1, 10);
				IPayloadManager manager = tx.GetTxPayloadManager();
				for (int j = 0; j < count; j++)
					manager.AddPayload(new byte[] { 0x00 });
				var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(count, info.Length);
			}
		}
		[Fact]
		public void GetPayloadsInfoCheckSize()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(1, (int)info[0].GetPayloadSize());
		}
		[Fact]
		public void GetPayloadsInfoCheckSizes()
		{
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
				IPayloadManager manager = tx.GetTxPayloadManager();
				int[] sizes = new int[10];
				for (int j = 0; j < 10; j++)
				{
					Random rnd = new();
					int count = rnd.Next(10, 1000);
					manager.AddPayload(new byte[count], null, new PayloadOptions { CType = CompressionType.None });
					sizes[j] = count;
				}
				var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(10, info.Length);
				for (int j = 0; j < 10; j++)
					Assert.Equal(sizes[j], (int)info[j].GetPayloadSize());
			}
		}
		[Fact]
		public void GetPayloadsInfoCheckHash()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.NotNull(info[0].GetPayloadHash());
			Assert.NotEmpty(info[0].GetPayloadHash()!);
			Assert.Equal(SizeFields.SHA256HashSize, (uint)info[0].GetPayloadHash()!.Length);
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayloadHash()));
		}
		[Fact]
		public void GetPayloadsInfoCheckHashMulti()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			for (int i = 0; i < 10; i++)
				tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			for (int i = 0; i < 10; i++)
			{
				Assert.NotNull(info[i].GetPayloadHash());
				Assert.NotEmpty(info[i].GetPayloadHash()!);
				Assert.Equal(SizeFields.SHA256HashSize, (uint)info[i].GetPayloadHash()!.Length);
				Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[i].GetPayloadHash()));
			}
		}
		[Fact]
		public void GetPayloadsInfoCheckHashVariedMulti()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 },
				"{\"Transaction\": \"Type\"}"u8.ToArray(),
				new byte[]{ 0x02, 0x00, 0x00, 0x00, 0x01, 0xa4, 0x17, 0xbc, 0x81, 0x30, 0x11, 0x41, 0x41 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102",
				"e41c86b377347af9c8f852c085b48b40afcdb800680d1f5d2b493d5a23373f47",
				"199430778155199dffb61b1ae5834bd793bc3f4e462c722d793387f66a6d5359"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			foreach (byte[] b in TestData)
				tx.GetTxPayloadManager().AddPayload(b, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestHash.Length; i++)
			{
				Assert.NotNull(info[i].GetPayloadHash());
				Assert.NotEmpty(info[i].GetPayloadHash()!);
				Assert.Equal(SizeFields.SHA256HashSize, (uint)info[i].GetPayloadHash()!.Length);
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info[i].GetPayloadHash()));
			}
		}
		[Fact]
		public void GetPayloadsInfoHashAfterBinBuild()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.NotEmpty(info[0].GetPayloadHash()!);
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayloadHash()));
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.NotEmpty(info[0].GetPayloadHash()!);
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayloadHash()));
		}
		[Fact]
		public void GetPayloadsInfoUnencryptedCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.False(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoEncryptedCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.True(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoEncryptedMultiCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 }, new string[] { "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			foreach (IPayloadInfo pi in info)
				Assert.True(pi.GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoUnencryptedMultiCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			foreach (IPayloadInfo pi in info)
				Assert.False(pi.GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoMixedEncryptedMultiCheck()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
			{
				if ((i & 1) == 1)
					manager.AddPayload(new byte[] { 0x00 });
				else
					manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			}
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
			{
				if ((i & 1) == 1)
					Assert.False(info[i].GetPayloadEncrypted());
				else
					Assert.True(info[i].GetPayloadEncrypted());
			}
		}
		[Fact]
		public void GetPayloadsInfoUnencryptedRebuildCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.False(info[0].GetPayloadEncrypted());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.False(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoEncryptedRebuildCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.True(info[0].GetPayloadEncrypted());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.True(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoNoWallets()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Empty(info[0].GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadsInfoNoWalletsMulti()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			for (int i = 0; i < 10; i++)
				Assert.Empty(info[i].GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadsInfoOneWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(wallet, info[0].GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetPayloadsInfoMultiWallets()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			for (int i = 0; i < 10; i++)
				Assert.Equal(wallet, info[i].GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetPayloadsInfoMultiWalletsPerPayload()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 3; j++)
					Assert.Equal(wallet[j], info[i].GetPayloadWallets()[j]);
			}
		}
		[Fact]
		public void GetPayloadsInfoMultiWalletsPerPayloadRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 3; j++)
					Assert.Equal(wallet[j], info[i].GetPayloadWallets()[j]);
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 3; j++)
					Assert.Equal(wallet[j], info[i].GetPayloadWallets()[j]);
			}
		}
		[Fact]
		public void GetPayloadsInfoNoPerPayloadRebuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
				Assert.Empty(info[i].GetPayloadWallets());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			for (int i = 0; i < 10; i++)
				Assert.Empty(info[i].GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadsInfoDifferWalletPerPayload()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < wallet.Length; i++)
				manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet[i] });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(wallet.Length, info.Length);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.Single(info[i].GetPayloadWallets());
				Assert.Equal(wallet[i], info[i].GetPayloadWallets()[0]);
			}
		}
		[Fact]
		public void GetPayloadsInfoDifferEncryptionPerPayload()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			manager.AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
			Assert.Equal(wallet, info[0].GetPayloadWallets());
			Assert.Empty(info[1].GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadsInfoDifferWalletPerPayloadRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < wallet.Length; i++)
				manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet[i] });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(wallet.Length, info.Length);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.Single(info[i].GetPayloadWallets());
				Assert.Equal(wallet[i], info[i].GetPayloadWallets()[0]);
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(wallet.Length, info.Length);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.Single(info[i].GetPayloadWallets());
				Assert.Equal(wallet[i], info[i].GetPayloadWallets()[0]);
			}
		}
		[Fact]
		public void GetPayloadsInfoDifferEncryptionPerPayloadRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			manager.AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
			Assert.Equal(wallet, info[0].GetPayloadWallets());
			Assert.Empty(info[1].GetPayloadWallets());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
			Assert.Equal(wallet, info[0].GetPayloadWallets());
			Assert.Empty(info[1].GetPayloadWallets());
		}

/* GetPayloadInfo() Tests */
		[Fact]
		public void GetPayloadInfoNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(0);
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadInfoZeroIndex()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(0);
			Assert.Null(info);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void GetPayloadInfoInvalidIndex()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(2);
			Assert.Null(info);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void GetPayloadInfoSingleValidCheck()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(1, (int)info.GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadInfoSingleValidRebuildCheck()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(1, (int)info.GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(1, (int)info.GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadInfoSingleEncryptedCheck()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet.Length, info.GetPayloadWallets().Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], info.GetPayloadWallets()[i]);
		}
		[Fact]
		public void GetPayloadInfoSingleEncryptedRebuildCheck()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet.Length, info.GetPayloadWallets().Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], info.GetPayloadWallets()[i]);
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			var (result_r, info_r) = transaction.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info_r);
			Assert.Equal(Status.STATUS_OK, result_r);
			Assert.True(info_r.GetPayloadEncrypted());
			Assert.NotNull(info_r.GetPayloadWallets());
			Assert.Equal(wallet.Length, info_r.GetPayloadWallets().Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], info_r.GetPayloadWallets()[i]);
			Assert.Equal(info_r.GetPayloadHash(), info.GetPayloadHash());
			Assert.Equal(info_r.GetPayloadSize(), info.GetPayloadSize());
		}
		[Fact]
		public void GetPayloadInfoMultiPayloadCheck()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			for (int i = 1; i <= TestData.Count; i++)
			{
				var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo((uint)i);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(TestData[i - 1].Length, (int)info.GetPayloadSize());
				Assert.Equal(TestHash[i - 1], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
				Assert.False(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadWallets());
				Assert.Empty(info.GetPayloadWallets());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			for (int i = 1; i <= TestData.Count; i++)
			{
				var (result, info) = transaction.GetTxPayloadManager().GetPayloadInfo((uint)i);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(TestData[i - 1].Length, (int)info.GetPayloadSize());
				Assert.Equal(TestHash[i - 1], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
				Assert.False(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadWallets());
				Assert.Empty(info.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetPayloadInfoMultiPayloadCheckEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			for (int i = 1; i <= TestData.Count; i++)
			{
				var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo((uint)i);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadWallets());
				Assert.Equal(wallet, info.GetPayloadWallets());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			for (int i = 1; i <= TestData.Count; i++)
			{
				var (result, info) = transaction.GetTxPayloadManager().GetPayloadInfo((uint)i);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadWallets());
				Assert.Equal(wallet, info.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetPayloadInfoMixedWalletPayload()
		{
			string wallet1 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet1 });
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet2 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet1, info.GetPayloadWallets()[0]);
			(result, info) = tx.GetTxPayloadManager().GetPayloadInfo(2);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
			(result, info) = tx.GetTxPayloadManager().GetPayloadInfo(3);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet2, info.GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetPayloadInfoMixedWalletPayloadRebuild()
		{
			string wallet1 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet1 });
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet2 });
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet1, info.GetPayloadWallets()[0]);
			(result, info) = tx.GetTxPayloadManager().GetPayloadInfo(2);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
			(result, info) = tx.GetTxPayloadManager().GetPayloadInfo(3);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet2, info.GetPayloadWallets()[0]);
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet1, info.GetPayloadWallets()[0]);
			(result, info) = transaction.GetTxPayloadManager().GetPayloadInfo(2);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.False(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Empty(info.GetPayloadWallets());
			(result, info) = transaction.GetTxPayloadManager().GetPayloadInfo(3);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(wallet2, info.GetPayloadWallets()[0]);
		}

/* GetAllPayloads() Tests */
		[Fact]
		public void GetAllPayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAllPayloadsSinglePayload()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.Equal(1, (int)p.DataSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayload().DataHash()));
			Assert.NotNull(p.GetInfo().GetPayloadWallets());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsSinglePayloadEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.True((int)p.DataSize() > 0);
			Assert.Equal(wallet.Length, p.GetInfo().GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			Assert.True(p.GetInfo().GetPayloadEncrypted());
		}
		[Fact]
		public void GetAllPayloadsSinglePayloadEncryptedRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.True((int)p.DataSize() > 0);
			Assert.Equal(wallet.Length, p.GetInfo().GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.True((int)p.DataSize() > 0);
			Assert.Equal(wallet.Length, p.GetInfo().GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsMultiPayload()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(TestData[0], null, new PayloadOptions { CType = CompressionType.None });
			tx.GetTxPayloadManager().AddPayload(TestData[1], null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.Equal(TestData[i].Length, (int)p.DataSize());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.True(p.DataSize() > 0);
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadRebuild()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(TestData[0], null, new PayloadOptions { CType = CompressionType.None });
			tx.GetTxPayloadManager().AddPayload(TestData[1], null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.Equal(TestData[i].Length, (int)p.DataSize());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.Equal(TestData[i].Length, (int)p.DataSize());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadEncryptedRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.True(p.DataSize() > 0);
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayload p = info[i].GetPayload();
				Assert.Equal(i + 1, (int)info[i].GetPayloadId());
				Assert.NotNull(p);
				Assert.True(p.DataSize() > 0);
				Assert.NotNull(p.GetInfo().GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMixedMultiPayloadEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(TestData[0], null, new PayloadOptions { CType = CompressionType.None });
			tx.GetTxPayloadManager().AddPayload(TestData[1], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.Equal((ulong)TestData[0].Length, p.DataSize());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
			p = info[1].GetPayload();
			Assert.Equal(2, (int)info[1].GetPayloadId());
			Assert.NotNull(p);
			Assert.True(p.DataSize() > 0);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsMixedMultiPayloadEncryptedRebuild()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray()
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(TestData[0], null, new PayloadOptions { CType = CompressionType.None });
			tx.GetTxPayloadManager().AddPayload(TestData[1], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.Equal((ulong)TestData[0].Length, p.DataSize());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
			p = info[1].GetPayload();
			Assert.Equal(2, (int)info[1].GetPayloadId());
			Assert.NotNull(p);
			Assert.True(p.DataSize() > 0);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			(result, info) = transaction.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.Equal((ulong)TestData[0].Length, p.DataSize());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
			p = info[1].GetPayload();
			Assert.Equal(2, (int)info[1].GetPayloadId());
			Assert.NotNull(p);
			Assert.True(p.DataSize() > 0);
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
		}

/* GetAccessiblePayloads() Tests */
		[Fact]
		public void GetAccessiblePayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadUnencrypted()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(p.DataHash()));
			Assert.Equal((uint)1, p.DataSize());
			Assert.False(p.GetInfo().GetPayloadEncrypted());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayload p = info[i].GetPayload();
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.Equal((ulong)TestData[i].Length, p.DataSize());
				Assert.False(p.GetInfo().GetPayloadEncrypted());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(p.DataHash()));
			Assert.Equal((uint)1, p.DataSize());
			Assert.False(p.GetInfo().GetPayloadEncrypted());
			Assert.Empty(p.GetInfo().GetPayloadWallets());
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayload p = info[i].GetPayload();
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.Equal((ulong)TestData[i].Length, p.DataSize());
				Assert.False(p.GetInfo().GetPayloadEncrypted());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncryptedWithOkWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo().GetPayloadEncrypted());
			Assert.Single(p.GetInfo().GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncryptedWithBadWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet2);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncryptedWithWallets()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			for (int i = 0; i < wallet.Length; i++)
			{
				var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet[i]);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(TestData.Count, info.Length);
				for (int j = 0; j < TestData.Count; j++)
				{
					Assert.Equal((uint)j + 1, info[j].GetPayloadId());
					IPayload p = info[j].GetPayload();
					Assert.True(p.DataSize() > 0);
					Assert.True(p.GetInfo().GetPayloadEncrypted());
					Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
				}
			}
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencryptedWithWalletRebuild()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			var (result, info) = transaction.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayload p = info[i].GetPayload();
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(p.DataHash()));
				Assert.Equal((ulong)TestData[i].Length, p.DataSize());
				Assert.False(p.GetInfo().GetPayloadEncrypted());
				Assert.Empty(p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncryptedWithWalletsRebuild()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			manager = transaction.GetTxPayloadManager();
			for (int i = 0; i < wallet.Length; i++)
			{
				var (result, info) = manager.GetAccessiblePayloads(wallet[i]);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(TestData.Count, info.Length);
				for (int j = 0; j < TestData.Count; j++)
				{
					Assert.Equal((uint)j + 1, info[j].GetPayloadId());
					IPayload p = info[j].GetPayload();
					Assert.True(p.DataSize() > 0);
					Assert.True(p.GetInfo().GetPayloadEncrypted());
					Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
				}
			}
		}
		[Fact]
		public void GetAccessiblePayloadsMixedPayloadsNoKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string wallet2 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[1]);
			manager.AddPayload(TestData[2], new string[] { wallet2 });
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			var (result, info) = manager.GetAccessiblePayloads();
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(2, (int)info[0].GetPayloadId());
			IPayloadInfo i = info[0].GetPayload().GetInfo();
			Assert.False(i.GetPayloadEncrypted());
			Assert.Empty(i.GetPayloadWallets());
		}
		[Fact]
		public void GetAccessiblePayloadsMixedPayloadsOkayKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string wallet2 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[1]);
			manager.AddPayload(TestData[2], new string[] { wallet2 });
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			var (result, info) = manager.GetAccessiblePayloads(wallet[0]);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(wallet.Length, info.Length);
			Assert.Equal(1, (int)info[0].GetPayloadId());
			IPayloadInfo i = info[0].GetPayload().GetInfo();
			Assert.True(i.GetPayloadEncrypted());
			Assert.Equal(wallet, i.GetPayloadWallets());
			Assert.Equal(2, (int)info[1].GetPayloadId());
			i = info[1].GetPayload().GetInfo();
			Assert.False(i.GetPayloadEncrypted());
			Assert.Empty(i.GetPayloadWallets());
			Assert.Equal(3, (int)info[2].GetPayloadId());
			i = info[2].GetPayload().GetInfo();
			Assert.True(i.GetPayloadEncrypted());
			Assert.Equal(wallet2, i.GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetAccessiblePayloadsMixedPayloadsOkaySingleKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[1], new string[] { wallet[2] });
			manager.AddPayload(TestData[2], new string[] { wallet[0] });
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			var (result, info) = manager.GetAccessiblePayloads(wallet[1]);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Single(info);
			Assert.Equal(1, (int)info[0].GetPayloadId());
			IPayloadInfo i = info[0].GetPayload().GetInfo();
			Assert.True(i.GetPayloadEncrypted());
			Assert.Equal(wallet, i.GetPayloadWallets());
		}

/* GetInaccessiblePayloads() Tests */
		[Fact]
		public void GetInaccessiblePayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo().GetPayloadEncrypted());
			Assert.Single(p.GetInfo().GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadEncrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayload p = info[i].GetPayload();
				Assert.True(p.DataSize() > 0);
				Assert.True(p.GetInfo().GetPayloadEncrypted());
				Assert.Equal(wallet, p.GetInfo().GetPayloadWallets());
			}
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadEncryptedWithOkWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadEncryptedWithBadWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet2);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo().GetPayloadEncrypted());
			Assert.Single(p.GetInfo().GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo().GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadEncryptedWithWallets()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			for (int i = 0; i < wallet.Length; i++)
			{
				var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet[i]);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.Null(info);
			}
		}
		[Fact]
		public void GetInccessiblePayloadsMultiPayloadUnencryptedWithWalletRebuild()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			var (result, info) = transaction.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadEncryptedWithWalletsRebuild()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			manager = transaction.GetTxPayloadManager();
			for (int i = 0; i < wallet.Length; i++)
			{
				var (result, info) = manager.GetInaccessiblePayloads(wallet[i]);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.Null(info);
			}
		}
		[Fact]
		public void GetInaccessiblePayloadsMixedPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet[1] });
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[0]);
			var (result, info) = manager.GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
			Assert.Equal(1, (int)info[0].GetPayloadId());
			IPayloadInfo i = info[0].GetPayload().GetInfo();
			Assert.True(i.GetPayloadEncrypted());
			Assert.Equal(wallet[1], i.GetPayloadWallets()[0]);
			Assert.Equal(2, (int)info[1].GetPayloadId());
			i = info[1].GetPayload().GetInfo();
			Assert.True(i.GetPayloadEncrypted());
			Assert.Equal(wallet, i.GetPayloadWallets());
		}
		[Fact]
		public void GetInaccessibleAllPayloadsMixedPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet[1] });
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[0], new string[] { wallet[2] });
			var (result, info) = manager.GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(3, info.Length);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayloadInfo inf = info[i].GetPayload().GetInfo();
				Assert.True(inf.GetPayloadEncrypted());
				Assert.NotEmpty(inf.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetInaccessibleAllPayloadsMixedPayloadsWallet()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet[1] });
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[0], new string[] { wallet[2] });
			var (result, info) = manager.GetInaccessiblePayloads(wallet[0]);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayloadInfo inf = info[0].GetPayload().GetInfo();
			Assert.True(inf.GetPayloadEncrypted());
			Assert.Equal(wallet[1], inf.GetPayloadWallets()[0]);
			Assert.Equal((uint)3, info[1].GetPayloadId());
			inf = info[1].GetPayload().GetInfo();
			Assert.True(inf.GetPayloadEncrypted());
			Assert.Equal(wallet[2], inf.GetPayloadWallets()[0]);
		}

/* GetAccessiblePayloadsData() Tests */
		[Fact]
		public void GetAccessiblePayloadsDataNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencryptedRaw()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencryptedRawModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			(result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(0, info[0][0]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMultiPayloadUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal(TestData[i], info[i]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataUnencryptedBadKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData("hello");
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(0, info[0][0]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataUnencryptedGoodKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(sender_pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(0, info[0][0]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMultiPayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadEncryptedGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(new string[] { wallet });
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(sender_pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal(0, info[0][0]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadEncryptedBadKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData("Hello");
			Assert.Equal(Status.TX_INVALID_KEY, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadEncryptedWrongKey()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMultiPayloadEncryptedGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(new string[] { wallet });
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(sender_pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal(TestData[i], info[i]);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMultiPayloadEncryptedBadKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData("ABC");
			Assert.Equal(Status.TX_INVALID_KEY, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMultiPayloadEncryptedWrongKey()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMixedPayloadNoKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			List< byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet });
			manager.AddPayload(TestData[1]);
			manager.AddPayload(TestData[2], new string[] { wallet2 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMixedPayloadDoubleMatch()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet });
			manager.AddPayload(TestData[1]);
			manager.AddPayload(TestData[2], new string[] { wallet2 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(2, info.Length);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMixedPayloadSingleMatch()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			string wallet3 = "ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet });
			manager.AddPayload(TestData[1], new string[] { wallet3 });
			manager.AddPayload(TestData[2], new string[] { wallet2 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataMixedPayloadMultiMatch()
		{
			string pkey = "BpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] wallet2 = {
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" };
			string[] wallet3 = {
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet });
			manager.AddPayload(TestData[1], wallet2);
			manager.AddPayload(TestData[2], wallet3);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(3, info.Length);
		}

/* ImportPayloads() Tests */
		[Fact]
		public void ImportPayloadsNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, id) = tx.GetTxPayloadManager().ImportPayloads(null);
			Assert.Null(id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ImportPayloadsOnePayloadUnencryptedNoRecipientsEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.True(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.False(info2[0].GetPayloadEncrypted());
			Assert.Equal((uint)1, info2[0].GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info2[0].GetPayloadHash()));
		}
		[Fact]
		public void ImportPayloadsOnePayloadEncryptedRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2[0].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadsOnePayloadEncryptedNoRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(wallet, info2[0].GetPayloadWallets()[0]);
		}
		[Fact]
		public void ImportPayloadsOnePayloadEncryptedBothEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.True(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.True(info2[0].GetPayloadSize() > 0);
		}
		[Fact]
		public void ImportPayloadsOnePayloadDifferingWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet2 });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(wallet, info2[0].GetPayloadWallets()[0]);
		}
		[Fact]
		public void ImportPayloadsOnePayloadMultiWalletsReorderedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] wallet2 = {
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(wallet2);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.True(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.True(info2[0].GetPayloadSize() > 0);
			Assert.Equal((uint)wallet2.Length, info2[0].GetPayloadWalletsCount());
			Assert.Equal(wallet, info2[0].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadsOnePayloadMultiWalletsEncryptedTxPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.True(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(4, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(info2[i].GetPayloadEncrypted());
				Assert.True(info2[i].GetPayloadSize() > 0);
				Assert.Equal((uint)wallet.Length, info2[i].GetPayloadWalletsCount());
				Assert.Equal(wallet, info2[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ImportPayloadsOnePayloadMultiWalletsUnencryptedTxPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo info = p[0].GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.True(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(4, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.False(info2[i].GetPayloadEncrypted());
				Assert.Equal((uint)0, info2[i].GetPayloadWalletsCount());
				Assert.Empty(info2[i].GetPayloadWallets());
				Assert.Equal((ulong)TestData[i].Length, info2[i].GetPayloadSize());
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadNoWalletsUnencryptedEmptyTx()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo info = p[i].GetPayload().GetInfo();
				Assert.False(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(TestData.Count, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.False(info2[i].GetPayloadEncrypted());
				Assert.Equal((uint)0, info2[i].GetPayloadWalletsCount());
				Assert.Empty(info2[i].GetPayloadWallets());
				Assert.Equal((ulong)TestData[i].Length, info2[i].GetPayloadSize());
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiWalletsEncryptedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo info = p[i].GetPayload().GetInfo();
				Assert.True(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(TestData.Count, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(info2[i].GetPayloadEncrypted());
				Assert.Equal((uint)wallet.Length, info2[i].GetPayloadWalletsCount());
				Assert.Equal(wallet, info2[i].GetPayloadWallets());
				Assert.True(info2[i].GetPayloadSize() > 0);
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadNoWalletsUnencryptedTxPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo info = p[i].GetPayload().GetInfo();
				Assert.False(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(TestData.Count << 1, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount() >> 1, id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(TestData.Count << 1, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.False(info2[TestData.Count + i].GetPayloadEncrypted());
				Assert.Equal((uint)0, info2[TestData.Count + i].GetPayloadWalletsCount());
				Assert.Empty(info2[TestData.Count + i].GetPayloadWallets());
				Assert.Equal((ulong)TestData[i].Length, info2[TestData.Count + i].GetPayloadSize());
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiWalletsEncryptedTxPayloads()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo info = p[i].GetPayload().GetInfo();
				Assert.True(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(TestData.Count << 1, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount() >> 1, id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(TestData.Count << 1, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(info2[TestData.Count + i].GetPayloadEncrypted());
				Assert.Equal((uint)wallet.Length, info2[TestData.Count + i].GetPayloadWalletsCount());
				Assert.Equal(wallet, info2[TestData.Count + i].GetPayloadWallets());
				Assert.True(info2[TestData.Count + i].GetPayloadSize() > 0);
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffWalletsUnencryptedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1], wallet);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2]);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(new IPayloadContainer[] { p, p2, p3 });
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(3, info2.Length);
			Assert.Equal(manager.GetPayloadsCount(), (uint)info2.Length);
			Assert.False(info2[0].GetPayloadEncrypted());
			Assert.Equal((uint)0, info2[0].GetPayloadWalletsCount());
			Assert.Empty(info2[0].GetPayloadWallets());
			Assert.True(info2[1].GetPayloadEncrypted());
			Assert.Equal((uint)wallet.Length, info2[1].GetPayloadWalletsCount());
			Assert.Equal(wallet, info2[1].GetPayloadWallets());
			Assert.False(info2[2].GetPayloadEncrypted());
			Assert.Equal((uint)0, info2[0].GetPayloadWalletsCount());
			Assert.Empty(info2[2].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffWalletsEncryptedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1], wallet);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2]);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(new IPayloadContainer[] { p, p2, p3 });
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			for (int i = 0; i < id.Length; i++)
				Assert.True(id[i]);
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(3, info2.Length);
			Assert.False(info2[0].GetPayloadEncrypted());
			Assert.True(info2[1].GetPayloadEncrypted());
			Assert.False(info2[2].GetPayloadEncrypted());
			Assert.Equal((uint)0, info2[0].GetPayloadWalletsCount());
			Assert.Equal((uint)wallet.Length, info2[1].GetPayloadWalletsCount());
			Assert.Equal((uint)0, info2[2].GetPayloadWalletsCount());
			Assert.Empty(info2[0].GetPayloadWallets());
			Assert.Equal(wallet, info2[1].GetPayloadWallets());
			Assert.Empty(info2[2].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffPayloadWalletsEncryptedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] wallet2 = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" };
			string[] wallet3 = {
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet3);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1], wallet2);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2], wallet);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(new IPayloadContainer[] { p, p2, p3 });
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(3, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), id.Length);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(TestData.Count, info2.Length);
			for (int i = 0; i < id.Length; i++)
			{
				Assert.True(id[i]);
				Assert.True(info2[i].GetPayloadEncrypted());
				Assert.True(info2[i].GetPayloadWalletsCount() > 0);
			}
		}

/* ImportPayload() Tests */
		[Fact]
		public void ImportPayloadNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, id) = tx.GetTxPayloadManager().ImportPayload(null);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ImportPayloadUnencryptedNoRecipientsEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.False(info2[0].GetPayloadEncrypted());
			Assert.Equal((uint)1, info2[0].GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info2[0].GetPayloadHash()));
		}
		[Fact]
		public void ImportPayloadEncryptedRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.Empty(info2[0].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadEncryptedNoRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.Equal(wallet, info2[0].GetPayloadWallets()[0]);
		}
		[Fact]
		public void ImportPayloadEncryptedBothEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.True(info2[0].GetPayloadSize() > 0);
		}
		[Fact]
		public void ImportPayloadDifferingWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(new string[] { wallet2 });
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.Equal(wallet, info2[0].GetPayloadWallets()[0]);
		}
		[Fact]
		public void ImportPayloadMultiWalletsReorderedEmptyTx()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] wallet2 = {
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx2.SetTxRecipients(wallet2);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.True(info2[0].GetPayloadSize() > 0);
			Assert.Equal((uint)wallet2.Length, info2[0].GetPayloadWalletsCount());
			Assert.Equal(wallet, info2[0].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadMultiWalletsEncryptedTxPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, wallet);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)4, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(4, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(info2[i].GetPayloadEncrypted());
				Assert.True(info2[i].GetPayloadSize() > 0);
				Assert.Equal((uint)wallet.Length, info2[i].GetPayloadWalletsCount());
				Assert.Equal(wallet, info2[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ImportPayloadMultiWalletsUnencryptedTxPayloads()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo info = p.GetPayload().GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)4, id);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(4, info2.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.False(info2[i].GetPayloadEncrypted());
				Assert.Equal((uint)0, info2[i].GetPayloadWalletsCount());
				Assert.Empty(info2[i].GetPayloadWallets());
				Assert.Equal((ulong)TestData[i].Length, info2[i].GetPayloadSize());
			}
		}

/* ReleasePayloads() Tests */
		[Fact]
		public void ReleasePayloadsNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, id) = tx.GetTxPayloadManager().ReleasePayloads(null);
			Assert.Null(id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadNoKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, id) = tx.GetTxPayloadManager().ReleasePayloads(null);
			Assert.Null(id);
			Assert.Equal(Status.TX_INVALID_KEY, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadBadKey()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, id) = manager.ReleasePayloads(pkey);
			Assert.Null(id);
			Assert.Equal(Status.TX_NOT_SIGNED, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadCorrectKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.Null(id);
			Assert.Equal(Status.TX_NOT_SIGNED, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadBadKeySigned()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(pkey);
			Assert.NotNull(id);
			Assert.Empty(id);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadGoodKeySigned()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.Equal((uint)1, id[0]);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadUnencryptedGoodKeySigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Empty(id);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadCorruptKeySigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads("abaabbabababababa");
			Assert.NotNull(id);
			Assert.Empty(id);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsMultiPayloadSameKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Equal(TestData.Count, id.Length);
			for (int i = 0; i < id.Length; i++)
				Assert.Equal((uint)i + 1, id[i]);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsMultiPayloadDifferentKeys()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet[1] });
			manager.AddPayload(TestData[1], new string[] { wallet[2] });
			manager.AddPayload(TestData[2], new string[] { wallet[0] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.Equal((uint)3, id[0]);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsMultiMixedPayload()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], new string[] { wallet[1] });
			manager.AddPayload(TestData[1], new string[] { wallet[2] });
			manager.AddPayload(TestData[2]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Empty(id);
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void ReleasePayloadsAllPayloadsMultiRelease()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet);
			manager.AddPayload(TestData[1], wallet);
			manager.AddPayload(TestData[2], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.NotNull(id);
			Assert.Equal(3, id.Length);
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal((uint)i + 1, id[i]);
			Assert.Equal(Status.STATUS_OK, result);
		}

/* AddPayload() Tests */
		[Fact]
		public void AddPayloadNoDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, id, _) = tx.GetTxPayloadManager().AddPayload(null);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void AddPayloadEmptyDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, id, _) = tx.GetTxPayloadManager().AddPayload(Array.Empty<byte>());
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void AddPayloadSingleDataUnencryptedEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal((uint)1, manager.GetPayloadsCount());
			IPayloadInfo? info = manager.GetPayloadInfo(id).info;
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			Assert.Equal((uint)1, info.GetPayloadSize());
		}
		[Fact]
		public void AddPayloadSingleDataEncryptedEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal((uint)1, manager.GetPayloadsCount());
			IPayloadInfo? info = manager.GetPayloadInfo(id).info;
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			Assert.True(info.GetPayloadSize() > 0);
		}
		[Fact]
		public void AddPayloadMultiDataUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, manager.GetPayloadsCount());
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
				Assert.Equal((uint)TestData[i].Length, info.GetPayloadSize());
			}
		}
		[Fact]
		public void AddPayloadEncryptionDisabledTxPayloads()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet, new PayloadOptions { EType = EncryptionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
				IPayloadInfo? info = manager.GetPayloadInfo(id).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Empty(info.GetPayloadWallets());
				Assert.True(info.GetPayloadCompressed());
				Assert.Equal(EncryptionType.None, info.GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void AddPayloadCompressionDisabledTxPayloads()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
				IPayloadInfo? info = manager.GetPayloadInfo(id).info;
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.False(info.GetPayloadCompressed());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Equal(wallet, info.GetPayloadWallets());
				Assert.Equal(EncryptionType.AES_256, info.GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void AddPayloadCompressionEncryptionDisabledTxPayloads()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet, new PayloadOptions { CType = CompressionType.None, EType = EncryptionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
				IPayloadInfo? info = manager.GetPayloadInfo(id).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.False(info.GetPayloadCompressed());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Empty(info.GetPayloadWallets());
				Assert.Equal(EncryptionType.None, info.GetPayloadEncryptionType());
				Assert.Equal((ulong)TestData[i].Length, info.GetPayloadSize());
			}
		}
		[Fact]
		public void AddPayloadEncryptionDisabledTxPayloadsSigned()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal(Status.STATUS_OK, manager.AddPayload(TestData[i], wallet, new PayloadOptions { EType = EncryptionType.None }).result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Empty(info.GetPayloadWallets());
				Assert.True(info.GetPayloadCompressed());
				Assert.Equal(EncryptionType.None, info.GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void AddPayloadCompressionDisabledTxPayloadsSigned()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal(Status.STATUS_OK, manager.AddPayload(TestData[i], wallet, new PayloadOptions { CType = CompressionType.None }).result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.False(info.GetPayloadCompressed());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Equal(wallet, info.GetPayloadWallets());
				Assert.Equal(EncryptionType.AES_256, info.GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void AddPayloadCompressionEncryptionDisabledTxPayloadsSigned()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				Assert.Equal(Status.STATUS_OK, manager.AddPayload(TestData[i], wallet, new PayloadOptions { CType = CompressionType.None, EType = EncryptionType.None }).result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count; i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.False(info.GetPayloadCompressed());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Empty(info.GetPayloadWallets());
				Assert.Equal(EncryptionType.None, info.GetPayloadEncryptionType());
				Assert.Equal((ulong)TestData[i].Length, info.GetPayloadSize());
			}
		}
		[Fact]
		public void AddPayloadMultiDataUnencryptedRebuildSingle()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, manager.GetPayloadsCount());
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
				Assert.Equal((uint)TestData[i].Length, info.GetPayloadSize());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			manager = transaction.GetTxPayloadManager();
			var (result2, id2, _) = manager.AddPayload(TestData[0], null, new PayloadOptions { CType = CompressionType.None });
			Assert.Equal((uint)4, id2);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.Equal((uint)4, manager.GetPayloadsCount());
			IPayloadInfo? info2 = manager.GetPayloadInfo(4).info;
			Assert.NotNull(info2);
			Assert.False(info2.GetPayloadEncrypted());
			Assert.Equal(TestHash[0], WalletUtils.ByteArrayToHexString(info2.GetPayloadHash()));
			Assert.Equal((uint)TestData[0].Length, info2.GetPayloadSize());
		}
		[Fact]
		public void AddPayloadSingleDataEncryptedTxPayloadsRebuildSingle()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet);
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, manager.GetPayloadsCount());
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadHash());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Equal(wallet, info.GetPayloadWallets());
				var (_, payload) = manager.GetPayload((uint)i + 1);
				Assert.NotNull(payload);
				Assert.True(payload.GetPayload().VerifyHash());
			}
			Transaction? t = tx.GetTxTransport().transport;
			Assert.NotNull(t);
			var (_, transaction) = TransactionBuilder.Build(t);
			Assert.NotNull(transaction);
			manager = transaction.GetTxPayloadManager();
			var (result2, id2, _) = manager.AddPayload(TestData[0], wallet);
			Assert.Equal((uint)4, id2);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.Equal((uint)4, manager.GetPayloadsCount());
			IPayloadInfo? info2 = manager.GetPayloadInfo(4).info;
			Assert.NotNull(info2);
			Assert.True(info2.GetPayloadEncrypted());
			Assert.NotNull(info2.GetPayloadHash());
			Assert.True(info2.GetPayloadSize() > 0);
			Assert.Equal(wallet, info2.GetPayloadWallets());
			var container2 = manager.GetPayload(4);
			Assert.NotNull(container2.payload);
			Assert.True(container2.payload.GetPayload().VerifyHash());
		}

/* GetPayload() Tests */
		[Fact]
		public void GetPayloadEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, payload) = tx.GetTxPayloadManager().GetPayload(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadZeroIdTxPayloadsUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadZeroIdTxPayloadsEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadValidIdTxPayloadsUnencrypted()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.NotNull(payload);
			Assert.Equal(Status.STATUS_OK, result2);
			IPayload p = payload.GetPayload();
			Assert.True(p.VerifyHash());
			IPayloadInfo info = p.GetInfo();
			Assert.False(info.GetPayloadEncrypted());
			Assert.Equal((uint)1, info.GetPayloadSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			Assert.Empty(info.GetPayloadWallets());
			Assert.Equal((uint)0, info.GetPayloadWalletsCount());
		}
		[Fact]
		public void GetPayloadValidIdTxPayloadsEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.NotNull(payload);
			Assert.Equal(Status.STATUS_OK, result2);
			IPayload p = payload.GetPayload();
			Assert.True(p.VerifyHash());
			IPayloadInfo info = p.GetInfo();
			Assert.True(info.GetPayloadEncrypted());
			Assert.True(info.GetPayloadSize() > 0);
			Assert.Equal(wallet, info.GetPayloadWallets()[0]);
			Assert.Equal((uint)1, info.GetPayloadWalletsCount());
		}
		[Fact]
		public void GetPayloadMultiValidIdTxPayloadsUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, payload) = tx.GetTxPayloadManager().GetPayload((uint)i + 1);
				Assert.NotNull(payload);
				Assert.Equal(Status.STATUS_OK, result2);
				IPayload p = payload.GetPayload();
				Assert.True(p.VerifyHash());
				IPayloadInfo info = p.GetInfo();
				Assert.False(info.GetPayloadEncrypted());
				Assert.Equal((uint)TestData[i].Length, info.GetPayloadSize());
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
				Assert.Empty(info.GetPayloadWallets());
				Assert.Equal((uint)0, info.GetPayloadWalletsCount());
			}
		}
		[Fact]
		public void GetPayloadMultiValidIdTxPayloadsEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet);
				Assert.Equal((uint)i + 1, id);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, payload) = tx.GetTxPayloadManager().GetPayload((uint)i + 1);
				Assert.NotNull(payload);
				Assert.Equal(Status.STATUS_OK, result2);
				IPayload p = payload.GetPayload();
				Assert.True(p.VerifyHash());
				IPayloadInfo info = p.GetInfo();
				Assert.True(info.GetPayloadEncrypted());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Equal(wallet, info.GetPayloadWallets());
				Assert.Equal((uint)wallet.Length, info.GetPayloadWalletsCount());
			}
		}
		[Fact]
		public void GetPayloadInvalidIdTxPayloadsUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(2);
			Assert.Null(payload);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadInvalidIdTxPayloadsEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(2);
			Assert.Null(payload);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}

/* GetPayloadData() Tests */
		[Fact]
		public void GetPayloadDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, payload) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadDataZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, payload) = tx.GetTxPayloadManager().GetPayloadData(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadDataZeroIdPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(0);
			Assert.Null(data);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadDataBadIdTxPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(2);
			Assert.Null(data);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadDataValidIdRaw()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
			Assert.Null(data);
		}
		[Fact]
		public void GetPayloadDataValidIdRawModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.NotNull(data);
			Assert.Single(data);
			Assert.Equal(Status.STATUS_OK, result2);
			(result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			(result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(data);
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
		}
		[Fact]
		public void GetPayloadDataValidIdUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.NotNull(data);
			Assert.Single(data);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.Equal(0, data[0]);
		}
		[Fact]
		public void GetPayloadDataValidIdEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(data);
			Assert.Equal(Status.TX_ACCESS_DENIED, result2);
		}
		[Fact]
		public void GetPayloadDataUnencryptedWithKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1, sender_pkey);
			Assert.NotNull(data);
			Assert.Single(data);
			Assert.Equal(Status.TX_NOT_ENCRYPTED, result2);
			Assert.Equal(0, data[0]);
		}
		[Fact]
		public void GetPayloadDataEncryptedWithKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1, sender_pkey);
			Assert.NotNull(data);
			Assert.Single(data);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.Equal(0, data[0]);
		}
		[Fact]
		public void GetPayloadDataEncryptedWithWrongKey()
		{
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1, pkey);
			Assert.Null(data);
			Assert.Equal(Status.TX_ACCESS_DENIED, result2);
		}
		[Fact]
		public void GetPayloadDataMultiPayloadDataUnencryptedNoKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, _, _) = manager.AddPayload(TestData[i]);
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, data) = tx.GetTxPayloadManager().GetPayloadData((uint)i + 1);
				Assert.NotNull(data);
				Assert.Equal(TestData[i].Length, data.Length);
				Assert.Equal(Status.STATUS_OK, result2);
				Assert.Equal(TestData[i], data);
			}
		}
		[Fact]
		public void GetPayloadDataMultiPayloadDataUnencryptedWithKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, _, _) = manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, data) = tx.GetTxPayloadManager().GetPayloadData((uint)i + 1, sender_pkey);
				Assert.NotNull(data);
				Assert.Equal(TestData[i].Length, data.Length);
				Assert.Equal(Status.TX_NOT_ENCRYPTED, result2);
				Assert.Equal(TestData[i], data);
			}
		}
		[Fact]
		public void GetPayloadDataMultiPayloadEncryptedWithKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, _, _) = manager.AddPayload(TestData[i], new[] { wallet });
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, data) = tx.GetTxPayloadManager().GetPayloadData((uint)i + 1, sender_pkey);
				Assert.NotNull(data);
				Assert.Equal(TestData[i].Length, data.Length);
				Assert.Equal(Status.STATUS_OK, result2);
				Assert.Equal(TestData[i], data);
			}
		}
		[Fact]
		public void GetPayloadDataMultiPayloadEncryptedWithWrongKey()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, _, _) = manager.AddPayload(TestData[i], new[] { wallet });
				Assert.Equal(Status.STATUS_OK, result);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal((uint)TestData.Count, tx.GetTxPayloadManager().GetPayloadsCount());
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, data) = tx.GetTxPayloadManager().GetPayloadData((uint)i + 1, pkey);
				Assert.Null(data);
				Assert.Equal(Status.TX_ACCESS_DENIED, result2);
			}
		}

/* RemovePayload() Tests */
		[Fact]
		public void RemovePayloadEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().RemovePayload(1);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void RemovePayloadZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().RemovePayload(0);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void RemovePayloadZeroIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(0);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void RemovePayloadBadIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(2);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void RemovePayloadValidIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(1);
			Assert.Equal((uint)0, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void RemovePayloadMultiPayloadTxPayloadsFirst()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, _, _) = manager.AddPayload(TestData[i]);
				Assert.Equal(Status.STATUS_OK, result2);
			}
			Status result = manager.RemovePayload(1);
			Assert.Equal((uint)TestData.Count - 1, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count - 1; i++)
			{
				byte[]? data = manager.GetPayloadData((uint)i + 1).data;
				Assert.Equal(TestData[i + 1].Length, data?.Length);
				Assert.Equal(TestData[i + 1], data!);
			}
		}
		[Fact]
		public void RemovePayloadMultiPayloadTxPayloadsLast()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, _, _) = manager.AddPayload(TestData[i]);
				Assert.Equal(Status.STATUS_OK, result2);
			}
			Status result = manager.RemovePayload(3);
			Assert.Equal((uint)TestData.Count - 1, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count - 1; i++)
			{
				byte[]? data = manager.GetPayloadData((uint)i + 1).data;
				Assert.Equal(TestData[i].Length, data?.Length);
				Assert.Equal(TestData[i], data!);
			}
		}
		[Fact]
		public void RemovePayloadMultiPayloadTxPayloadsMiddle()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, _, _) = manager.AddPayload(TestData[i]);
				Assert.Equal(Status.STATUS_OK, result2);
			}
			Status result = manager.RemovePayload(2);
			Assert.Equal((uint)TestData.Count - 1, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			byte[]? data = manager.GetPayloadData(1).data;
			Assert.Equal(TestData[0].Length, data?.Length);
			Assert.Equal(TestData[0], data!);
			data = manager.GetPayloadData(2).data;
			Assert.Equal(TestData[2].Length, data?.Length);
			Assert.Equal(TestData[2], data!);
		}
		[Fact]
		public void RemovePayloadMultiPayloadTxPayloadsMiddleEncrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, _, _) = manager.AddPayload(TestData[i], wallet);
				Assert.Equal(Status.STATUS_OK, result2);
			}
			Status result = manager.RemovePayload(2);
			Assert.Equal((uint)TestData.Count - 1, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			byte[]? data = manager.GetPayloadData(1, sender_pkey).data;
			Assert.Equal(TestData[0].Length, data?.Length);
			Assert.Equal(TestData[0], data!);
			data = manager.GetPayloadData(2, sender_pkey).data;
			Assert.Equal(TestData[2].Length, data?.Length);
			Assert.Equal(TestData[2], data!);
		}

/* AddPayloadWallet() Tests */
		[Fact]
		public void AddPayloadWalletEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NO_PAYLOAD, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletBadMinId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(0, wallet);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletBadMaxId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(2, wallet);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletUnencryptedNoWallet()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletUnencryptedCorruptWallet()
		{
			string wallet = "ls1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_INVALID_WALLET, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletEncryptedSameWalletNoKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_ACCESS_DENIED, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletEncryptedSameWalletGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet, sender_pkey);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.False(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.Equal(wallet, info.GetPayloadWallets()[0]);
		}
		[Fact]
		public void AddPayloadWalletEncryptedSameWalletCorruptKey()
		{
			string sender = "RpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet, sender);
			Assert.Equal(Status.TX_INVALID_KEY, result.result);
			Assert.False(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.Equal(wallet, info.GetPayloadWallets()[0]);
		}
		[Fact]
		public void AddPayloadWalletEncryptedNewWalletGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet2, sender_pkey);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.True(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Equal(2, info.GetPayloadWallets().Length);
			Assert.Equal(new string[] { wallet, wallet2 }, info.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletEncryptedMultiWalletGoodKey()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" };
			string wallet2 = "ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, wallet);
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet2, sender_pkey);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.True(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Equal(3, info.GetPayloadWallets().Length);
			Assert.Equal(new string[] { wallet[0], wallet[1], wallet2 }, info.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletUnencryptedWalletNoKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.True(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.True(info.GetPayloadEncrypted());
			Assert.Equal(new string[]{ wallet }, info.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletUnencryptedWalletCorruptKey()
		{
			string sender = "RpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet, sender);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.True(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.True(info.GetPayloadEncrypted());
			Assert.Equal(new string[] { wallet }, info.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletUnencryptedWalletWalletGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet, sender_pkey);
			Assert.Equal(Status.STATUS_OK, result.result);
			Assert.True(result.added);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.True(info.GetPayloadEncrypted());
			Assert.Equal(new string[] { wallet }, info.GetPayloadWallets());
		}

/* RemovePayloadWallet() Tests */
		[Fact]
		public void RemovePayloadWalletEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			(Status status, bool removed) = tx.GetTxPayloadManager().RemovePayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NO_PAYLOAD, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletMinId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			(Status status, bool removed) = tx.GetTxPayloadManager().RemovePayloadWallet(0, wallet);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletBadId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			(Status status, bool removed) = tx.GetTxPayloadManager().RemovePayloadWallet(2, wallet);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletUnencryptedNoWallet()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletUnencryptedCorruptWallet()
		{
			string wallet = "ls1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, wallet);
			Assert.Equal(Status.TX_INVALID_WALLET, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletUnencryptedGoodWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NOT_ENCRYPTED, status);
			Assert.False(removed);
		}
		[Fact]
		public void RemovePayloadWalletUnencryptedMatchWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(removed);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Empty(info.GetPayloadWallets());
			Assert.True(info.GetPayloadEncrypted());
		}
		[Fact]
		public void RemovePayloadWalletUnencryptedDiffWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, wallet2);
			Assert.False(removed);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Single(info.GetPayloadWallets());
			Assert.True(info.GetPayloadEncrypted());
		}
		[Fact]
		public void RemoveloadWalletEncryptedMultiWallets()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string wallet2 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result);
			(Status status, bool removed) = manager.RemovePayloadWallet(1, wallet2);
			Assert.True(removed);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.Equal(2, info.GetPayloadWallets().Length);
			Assert.True(info.GetPayloadEncrypted());
			Assert.Equal(new string[] { wallet[1], wallet[2] }, info.GetPayloadWallets());
		}

/* ReleasePayload() Tests */
		[Fact]
		public void ReleasePayloadEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReleasePayloadBadMinId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = tx.GetTxPayloadManager().ReleasePayload(0, sender_pkey);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status);
		}
		[Fact]
		public void ReleasePayloadBadMaxId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = tx.GetTxPayloadManager().ReleasePayload(2, sender_pkey);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status);
		}
		[Fact]
		public void ReleasePayloadUnencryptedGoodKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NOT_ENCRYPTED, status);
		}
		[Fact]
		public void ReleasePayloadUnencryptedNoKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, null);
			Assert.Equal(Status.TX_NOT_ENCRYPTED, status);
		}
		[Fact]
		public void ReleasePayloadUnencryptedBadKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, "hello");
			Assert.Equal(Status.TX_NOT_ENCRYPTED, status);
		}
		[Fact]
		public void ReleasePayloadEncryptedGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo? info = manager.GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			Assert.Empty(info.GetPayloadWallets());
		}
		[Fact]
		public void ReleasePayloadEncryptedCorruptKey()
		{
			string wallet = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, "hello");
			Assert.Equal(Status.TX_INVALID_KEY, status);
			IPayloadInfo? info = manager.GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			Assert.Single(info.GetPayloadWallets());
		}
		[Fact]
		public void ReleasePayloadEncryptedWrongKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string sender = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, sender);
			Assert.Equal(Status.TX_ACCESS_DENIED, status);
			IPayloadInfo? info = manager.GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			Assert.Single(info.GetPayloadWallets());
		}
		[Fact]
		public void ReleasePayloadEncryptedNoKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Status status = manager.ReleasePayload(1, null);
			Assert.Equal(Status.TX_ACCESS_DENIED, status);
			IPayloadInfo? info = manager.GetPayloadInfo(1).info;
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			Assert.Single(info.GetPayloadWallets());
		}
		[Fact]
		public void ReleasePayloadMultiWalletPayload()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] keys = {
				"BpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP",
				"BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8",
				"BhaUhsRxQ3zq5BNq7cFWxt7tMuLQ3AbmGDHNnxDVGDdLDVVTiBRxWGYxqhJuc75Dk4VefPWAgMKbaFrDQ3VFKtvu8FxHvo" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < wallet.Length; i++)
			{
				var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
				Assert.Equal(Status.STATUS_OK, result);
				Status status = manager.ReleasePayload((uint)i + 1, keys[i]);
				Assert.Equal(Status.STATUS_OK, status);
				IPayloadInfo? info = manager.GetPayloadInfo((uint)i + 1).info;
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
				Assert.Empty(info.GetPayloadWallets());
			}
		}

/* ReplacePayloadData() Tests */
		[Fact]
		public void ReplacePayloadDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(1, new byte[] { 0 });
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataEmptyTxNoData()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(1, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataEmptyTxBadId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(0, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadNoData()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(1, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadBadId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(0, new byte[] { 0x00 });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadBadIdLast()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(2, new byte[] { 0x00 });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void ReplacePayloadDataSinglePayloadUnencrypted()
		{
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			};
			byte[] TestData = "Hello World"u8.ToArray();
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, id, _) = manager.AddPayload(new byte[] { 0x00 }, null, new PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, result2);
			var pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.Equal(TestHash[0], WalletUtils.ByteArrayToHexString(pi.info.GetPayloadHash()));
			Status result = manager.ReplacePayloadData(1, TestData);
			Assert.Equal(Status.STATUS_OK, result);
			pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.Equal(TestHash[1], WalletUtils.ByteArrayToHexString(pi.info.GetPayloadHash()));
		}
		[Fact]
		public void ReplacePayloadDataSinglePayloadEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			byte[] TestData = "Hello World"u8.ToArray();
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, id, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result2);
			var pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.True(pi.info.GetPayloadEncrypted());
			Assert.Equal(wallet, pi.info.GetPayloadWallets());
			byte[]? phash = pi.info.GetPayloadHash();
			Status result = manager.ReplacePayloadData(1, TestData);
			Assert.Equal(Status.STATUS_OK, result);
			pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.True(pi.info.GetPayloadEncrypted());
			Assert.Equal(wallet, pi.info.GetPayloadWallets());
			Assert.NotEqual(phash, pi.info.GetPayloadHash());
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var d = manager.GetPayloadData(1, sender_pkey);
			Assert.Equal(Status.STATUS_OK, d.result);
			Assert.NotNull(d.data);
			Assert.Equal(TestData, d.data);
		}
		[Fact]
		public void ReplacePayloadDataSinglePayloadLockedEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			byte[] TestData = "Hello World"u8.ToArray();
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, id, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result2);
			var pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.True(pi.info.GetPayloadEncrypted());
			byte[]? phash = pi.info.GetPayloadHash();
			Assert.Equal(new bool[] { true, true, true }, manager.RemovePayloadWallets(id, wallet).wallets);
			pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.True(pi.info.GetPayloadEncrypted());
			Assert.Empty(pi.info.GetPayloadWallets());
			Assert.Equal(Status.STATUS_OK, manager.ReplacePayloadData(id, TestData));
			pi = manager.GetPayloadInfo(id);
			Assert.NotNull(pi.info);
			Assert.True(pi.info.GetPayloadEncrypted());
			Assert.Empty(pi.info.GetPayloadWallets());
			Assert.NotEqual(phash, pi.info.GetPayloadHash());
		}
		[Fact]
		public void ReplacePayloadDataMultiPayloadUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] TestHash =
			{
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, id, _) = manager.AddPayload(TestData[i], null, new PayloadOptions { CType = CompressionType.None });
				Assert.Equal(Status.STATUS_OK, result2);
				var (_, info) = manager.GetPayloadInfo(id);
				Assert.NotNull(info);
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			}
			for (int i = 0; i < TestData.Count - 1; i++)
			{
				Status result = manager.ReplacePayloadData((uint)i + 2, TestData[0]);
				Assert.Equal(Status.STATUS_OK, result);
			}
			for (int i = 0; i < TestData.Count; i++)
			{
				var (_, info) = manager.GetPayloadInfo((uint)i + 1);
				Assert.NotNull(info);
				Assert.Equal(TestHash[0], WalletUtils.ByteArrayToHexString(info.GetPayloadHash()));
			}
		}
		[Fact]
		public void ReplacePayloadDataMultiPayloadEncrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, id, _) = manager.AddPayload(TestData[i], wallet);
				Assert.Equal(Status.STATUS_OK, result2);
				var (_, info) = manager.GetPayloadInfo(id);
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.Equal(wallet, info.GetPayloadWallets());
			}
			for (int i = 0; i < TestData.Count - 1; i++)
			{
				Status result = manager.ReplacePayloadData((uint)i + 2, TestData[0]);
				Assert.Equal(Status.STATUS_OK, result);
				var pi = manager.GetPayloadInfo((uint)i + 1);
				Assert.NotNull(pi.info);
				Assert.True(pi.info.GetPayloadEncrypted());
				Assert.Equal(wallet, pi.info.GetPayloadWallets());
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, data) = manager.GetPayloadData((uint)i + 1, sender_pkey);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(data);
				Assert.Equal(TestData[0], data);
			}
		}
		[Fact]
		public void ReplacePayloadDataMultiPayloadEncryptedResetSigned()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.TxId);
			Assert.Equal(Status.STATUS_OK, manager.ReplacePayloadData((uint)2, TestData[0]));
			(Status result, Transaction? transport2) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport2);
			Assert.Null(transport2.TxId);
		}

/* VerifyAllPayloadsData() Tests */
		[Fact]
		public void VerifyAllPayloadsDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var result = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.TX_NO_PAYLOAD, result.result);
			Assert.Null(result.verification);
		}
		[Fact]
		public void VerifyAllPayloadsDataSinglePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal((uint)1, id);
			var status = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.verification);
			Assert.Single(status.verification);
			Assert.True(status.verification[0]);
		}
		[Fact]
		public void VerifyAllPayloadsDataSinglePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal((uint)1, id);
			var status = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.verification);
			Assert.Single(status.verification);
			Assert.True(status.verification[0]);
		}
		[Fact]
		public void VerifyAllPayloadsDataMultiPayloadsUnencrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.Equal((uint)i + 1, id);
			}
			var status = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.verification);
			Assert.Equal(TestData.Count, status.verification.Length);
			for (int i = 0; i < TestData.Count; i++)
				Assert.True(status.verification[i]);
		}
		[Fact]
		public void VerifyAllPayloadsDataMultiPayloadsEncrypted()
		{
			List<byte[]> TestData = new()
			{
				new byte[]{ 0x00 },
				"Hello World"u8.ToArray(),
				new byte[]{ 0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 }
			};
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i], wallet);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.Equal((uint)i + 1, id);
			}
			var status = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.verification);
			Assert.Equal(TestData.Count, status.verification.Length);
			for (int i = 0; i < TestData.Count; i++)
				Assert.True(status.verification[i]);
		}

/* AddPayloadWallets() Tests */
		[Fact]
		public void AddPayloadWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var result = tx.GetTxPayloadManager().AddPayloadWallets(1, new string[] { wallet });
			Assert.Equal(Status.TX_NO_PAYLOAD, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsNoWallet()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var result = tx.GetTxPayloadManager().AddPayloadWallets(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsNoWalletEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			var result = tx.GetTxPayloadManager().AddPayloadWallets(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsLowId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, _, _) = tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var status = tx.GetTxPayloadManager().AddPayloadWallets(0, new string[] { wallet });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status.result);
			Assert.Null(status.wallets);
		}
		[Fact]
		public void AddPayloadWalletsHighId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, _, _) = tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var status = tx.GetTxPayloadManager().AddPayloadWallets(2, new string[] { wallet });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, status.result);
			Assert.Null(status.wallets);
		}
		[Fact]
		public void AddPayloadWalletsSingleUnencrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, status.result);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal(new string[] { wallet }, pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsMultipleUnencrypted()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, wallet);
			Assert.Equal(Status.STATUS_OK, status.result);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal(wallet, pi.GetPayloadWallets());
			Assert.Equal((uint)wallet.Length, pi.GetPayloadWalletsCount());
		}
		[Fact]
		public void AddPayloadWalletsSingleEncryptedNoKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, new string[] { wallet2 }, null);
			Assert.Equal(Status.TX_ACCESS_DENIED, status.result);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Single(pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsSingleEncryptedBadKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, new string[] { wallet2 }, "hello");
			Assert.Equal(Status.TX_INVALID_KEY, status.result);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Single(pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsSingleEncryptedGoodKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, new string[] { wallet2 }, sender_pkey);
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.wallets);
			Assert.Single(status.wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal((uint)2, pi.GetPayloadWalletsCount());
			Assert.Equal(new string[] { wallet, wallet2 }, pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsMultiEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] wallet2 = {
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, wallet2, sender_pkey);
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.wallets);
			Assert.Equal(wallet2.Length, status.wallets.Length);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal((uint)3, pi.GetPayloadWalletsCount());
			Assert.Equal(new string[] { wallet, wallet2[0], wallet2[1] }, pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsMultiUnencryptedMixed()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ls1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" };
			bool[] res = { true, false, true, false };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, wallet);
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.wallets);
			Assert.Equal(wallet.Length, status.wallets.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(res[i], status.wallets[i]);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal((uint)2, pi.GetPayloadWalletsCount());
			Assert.Equal(new string[] { wallet[0], wallet[2] }, pi.GetPayloadWallets());
		}
		[Fact]
		public void AddPayloadWalletsMultiEncryptedMixed()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" };
			string[] wallet2 = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ls1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			bool[] res = { false, false, false, true, false };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result);
			var status = manager.AddPayloadWallets(1, wallet2, sender_pkey);
			Assert.Equal(Status.STATUS_OK, status.result);
			Assert.NotNull(status.wallets);
			Assert.Equal(wallet2.Length, status.wallets.Length);
			for (int i = 0; i < wallet2.Length; i++)
				Assert.Equal(res[i], status.wallets[i]);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Equal((uint)3, pi.GetPayloadWalletsCount());
			Assert.Equal(new string[] { wallet[0], wallet[1], wallet2[3] }, pi.GetPayloadWallets());
		}

/* RemovePayloadWallets() Tests */
		[Fact]
		public void RemovePayloadWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			var (result, wallets) = tx.GetTxPayloadManager().RemovePayloadWallets(1, new string[] { wallet });
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsLowId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, wallets) = tx.GetTxPayloadManager().RemovePayloadWallets(0, new string[] { wallet });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsHighId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[] { 0x00 });
			var (result, wallets) = tx.GetTxPayloadManager().RemovePayloadWallets(2, new string[] { wallet });
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsNoWallet()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, status);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsNoWalletEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, null);
			Assert.Equal(Status.TX_INVALID_WALLET, status);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsUnencryptedWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.False(pi.GetPayloadEncrypted());
			Assert.Empty(pi.GetPayloadWallets());
			Assert.Single(wallets);
			Assert.False(wallets[0]);
		}
		[Fact]
		public void RemovePayloadWalletsEncryptedWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Empty(pi.GetPayloadWallets());
			Assert.Single(wallets);
			Assert.True(wallets[0]);
		}
		[Fact]
		public void RemovePayloadWalletsEncryptedDifferentWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, new string[] { wallet });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, new string[] { wallet2 });
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Single(pi.GetPayloadWallets());
			Assert.Single(wallets);
			Assert.False(wallets[0]);
		}
		[Fact]
		public void RemovePayloadWalletsEncryptedMultiWallet()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] wallet2 = {
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, wallet2);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Single(pi.GetPayloadWallets());
			Assert.Equal(new string[] { wallet[0] }, pi.GetPayloadWallets());
			Assert.Equal(wallet2.Length, wallets.Length);
			Assert.True(wallets[0]);
			Assert.True(wallets[1]);
		}
		[Fact]
		public void RemovePayloadWalletsEncryptedMultiMixture()
		{
			string[] wallet = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			string[] wallet2 = {
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ls1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			bool[] res = { true, false, false, true, false };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 }, wallet);
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, wallet2);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.True(pi.GetPayloadEncrypted());
			Assert.Single(pi.GetPayloadWallets());
			Assert.Equal(new string[] { wallet[1] }, pi.GetPayloadWallets());
			Assert.Equal(wallet2.Length, wallets.Length);
			for (int i = 0; i < wallet2.Length; i++)
				Assert.Equal(res[i], wallets[i]);
		}
		[Fact]
		public void RemovePayloadWalletsUnencryptedMultiWallet()
		{
			string[] wallet = {
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" };
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload(new byte[] { 0x00 });
			Assert.Equal(Status.STATUS_OK, result);
			var (status, wallets) = manager.RemovePayloadWallets(1, wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(wallets);
			IPayloadInfo? pi = manager.GetPayloadInfo(1).info;
			Assert.NotNull(pi);
			Assert.False(pi.GetPayloadEncrypted());
			Assert.Empty(pi.GetPayloadWallets());
			Assert.Equal(wallet.Length, wallets.Length);
			Assert.False(wallets[0]);
			Assert.False(wallets[1]);
		}
	}
}
