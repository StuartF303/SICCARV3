// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

// PayloadManager V1 Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Collections.Generic;
using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class TransactionV1PManagerTests
	{
		private readonly string sender_pkey = "BpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";

/* GetPayloadsCount() Tests */
		[Fact]
		public void GetPayloadsCountEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Assert.Equal((UInt32)0, tx.GetTxPayloadManager().GetPayloadsCount());
		}
		[Fact]
		public void GetPayloadsCountOnePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal((UInt32)1, tx.GetTxPayloadManager().GetPayloadsCount());
		}
		[Fact]
		public void GetPayloadsCountMultiPayload()
		{
			for (int i = 0; i < 10; i++)
			{
				Random rnd = new();
				int count = rnd.Next(1, 10);
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
				IPayloadManager manager = tx.GetTxPayloadManager();
				for (int j = 0; j < count; j++)
					manager.AddPayload([0x00]);
				Assert.Equal(count, (int)manager.GetPayloadsCount());
			}
		}
		[Fact]
		public void GetPayloadsCountAfterBinBuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}
		[Fact]
		public void GetPayloadsInfoOnePayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
				Random rnd = new();
				int count = rnd.Next(1, 10);
				IPayloadManager manager = tx.GetTxPayloadManager();
				for (int j = 0; j < count; j++)
					manager.AddPayload([0x00]);
				var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.Equal(count, info.Length);
			}
		}
		[Fact]
		public void GetPayloadsInfoCheckSize()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
				IPayloadManager manager = tx.GetTxPayloadManager();
				int[] sizes = new int[10];
				for (int j = 0; j < 10; j++)
				{
					Random rnd = new();
					int count = rnd.Next(10, 1000);
					manager.AddPayload(new byte[count]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.NotNull(info[0].GetPayloadHash());
			Assert.NotEmpty(info[0].GetPayloadHash()!);
			Assert.Equal(TxDefs.SHA256HashSize, (uint)info[0].GetPayloadHash()!.Length);
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayloadHash()));
		}
		[Fact]
		public void GetPayloadsInfoCheckHashMulti()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			for (int i = 0; i < 10; i++)
				tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			for (int i = 0; i < 10; i++)
			{
				Assert.NotNull(info[i].GetPayloadHash());
				Assert.NotEmpty(info[i].GetPayloadHash()!);
				Assert.Equal(TxDefs.SHA256HashSize, (uint)info[i].GetPayloadHash()!.Length);
				Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[i].GetPayloadHash()));
			}
		}
		[Fact]
		public void GetPayloadsInfoCheckHashVariedMulti()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				"{\"Transaction\": \"Type\"}"u8.ToArray(),
				[0x02, 0x00, 0x00, 0x00, 0x01, 0xa4, 0x17, 0xbc, 0x81, 0x30, 0x11, 0x41, 0x41]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102",
				"e41c86b377347af9c8f852c085b48b40afcdb800680d1f5d2b493d5a23373f47",
				"199430778155199dffb61b1ae5834bd793bc3f4e462c722d793387f66a6d5359"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			foreach (byte[] b in TestData)
				tx.GetTxPayloadManager().AddPayload(b);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestHash.Length; i++)
			{
				Assert.NotNull(info[i].GetPayloadHash());
				Assert.NotEmpty(info[i].GetPayloadHash()!);
				Assert.Equal(TxDefs.SHA256HashSize, (uint)info[i].GetPayloadHash()!.Length);
				Assert.Equal(TestHash[i], WalletUtils.ByteArrayToHexString(info[i].GetPayloadHash()));
			}
		}
		[Fact]
		public void GetPayloadsInfoHashAfterBinBuild()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.False(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoEncryptedCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(["ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz"]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.True(info[0].GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoEncryptedMultiCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(["ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz"]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(10, info.Length);
			foreach (IPayloadInfo pi in info)
				Assert.False(pi.GetPayloadEncrypted());
		}
		[Fact]
		public void GetPayloadsInfoUnencryptedRebuildCheck()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(["ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz"]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Empty(info[0].GetPayloadWallets());
		}
		[Fact]
		public void GetPayloadsInfoNoWalletsMulti()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadsInfo();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			for (int i = 0; i < 10; i++)
				Assert.Equal(wallet, info[i].GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetPayloadsInfoMultiWalletsPerPayload()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
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

/* GetPayloadInfo() Tests */
		[Fact]
		public void GetPayloadInfoNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(0);
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadInfoZeroIndex()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(0);
			Assert.Null(info);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void GetPayloadInfoInvalidIndex()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(2);
			Assert.Null(info);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void GetPayloadInfoSingleValidCheck()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(3, info.GetPayloadWallets().Length);
			for (int i = 0; i < 3; i++)
				Assert.Equal(wallet[i], info.GetPayloadWallets()[i]);
		}
		[Fact]
		public void GetPayloadInfoSingleEncryptedRebuildCheck()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo(1);
			Assert.NotNull(info);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.True(info.GetPayloadEncrypted());
			Assert.NotNull(info.GetPayloadWallets());
			Assert.Equal(3, info.GetPayloadWallets().Length);
			for (int i = 0; i < 3; i++)
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
			Assert.Equal(3, info_r.GetPayloadWallets().Length);
			for (int i = 0; i < 3; i++)
				Assert.Equal(wallet[i], info_r.GetPayloadWallets()[i]);
			Assert.Equal(info_r.GetPayloadHash(), info.GetPayloadHash());
			Assert.Equal(info_r.GetPayloadSize(), info.GetPayloadSize());
		}
		[Fact]
		public void GetPayloadInfoMultiPayloadCheck()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			for (int i = 1; i <= TestData.Count; i++)
			{
				var (result, info) = tx.GetTxPayloadManager().GetPayloadInfo((uint)i);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.NotNull(info.GetPayloadWallets());
				for (int j = 0; j < wallet.Length; j++)
					Assert.Equal(wallet[j], info.GetPayloadWallets()[j]);
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
				for (int j = 0; j < wallet.Length; j++)
					Assert.Equal(wallet[j], info.GetPayloadWallets()[j]);
			}
		}

/* GetAllPayloads() Tests */
		[Fact]
		public void GetAllPayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAllPayloadsSinglePayload()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.Equal(1, (int)p.DataSize());
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(info[0].GetPayload().DataHash()));
			Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
			Assert.Empty(p.GetInfo()!.GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsSinglePayloadEncrypted()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.True((int)p.DataSize() > 0);
			Assert.Equal(wallet.Length, p.GetInfo()!.GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsSinglePayloadEncryptedRebuild()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			IPayload p = info[0].GetPayload();
			Assert.Equal(1, (int)info[0].GetPayloadId());
			Assert.NotNull(p);
			Assert.True((int)p.DataSize() > 0);
			Assert.Equal(wallet.Length, p.GetInfo()!.GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
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
			Assert.Equal(wallet.Length, p.GetInfo()!.GetPayloadWallets().Length);
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
		}
		[Fact]
		public void GetAllPayloadsMultiPayload()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload(TestData[0]);
			tx.GetTxPayloadManager().AddPayload(TestData[1]);
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadEncrypted()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadRebuild()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload(TestData[0]);
			tx.GetTxPayloadManager().AddPayload(TestData[1]);
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAllPayloadsMultiPayloadEncryptedRebuild()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
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
				Assert.NotNull(p.GetInfo()!.GetPayloadWallets());
				Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
			}
		}

/* GetAccessiblePayloads() Tests */
		[Fact]
		public void GetAccessiblePayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadUnencrypted()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(p.DataHash()));
			Assert.Equal((uint)1, p.DataSize());
			Assert.False(p.GetInfo()!.GetPayloadEncrypted());
			Assert.Empty(p.GetInfo()!.GetPayloadWallets());
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
				Assert.False(p.GetInfo()!.GetPayloadEncrypted());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(p.DataHash()));
			Assert.Equal((uint)1, p.DataSize());
			Assert.False(p.GetInfo()!.GetPayloadEncrypted());
			Assert.Empty(p.GetInfo()!.GetPayloadWallets());
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
				Assert.False(p.GetInfo()!.GetPayloadEncrypted());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncryptedWithOkWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo()!.GetPayloadEncrypted());
			Assert.Single(p.GetInfo()!.GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetAccessiblePayloadsOnePayloadEncryptedWithBadWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloads(wallet2);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncryptedWithWallets()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
					Assert.True(p.GetInfo()!.GetPayloadEncrypted());
					Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
				}
			}
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadUnencryptedWithWalletRebuild()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
				Assert.False(p.GetInfo()!.GetPayloadEncrypted());
				Assert.Empty(p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetAccessiblePayloadsMultiPayloadEncryptedWithWalletsRebuild()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
					Assert.True(p.GetInfo()!.GetPayloadEncrypted());
					Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
				}
			}
		}

/* GetInaccessiblePayloads() Tests */
		[Fact]
		public void GetInaccessiblePayloadsNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadUnencrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadUnencryptedWithWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo()!.GetPayloadEncrypted());
			Assert.Single(p.GetInfo()!.GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadEncrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Equal(TestData.Count, info.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((uint)i + 1, info[i].GetPayloadId());
				IPayload p = info[i].GetPayload();
				Assert.True(p.DataSize() > 0);
				Assert.True(p.GetInfo()!.GetPayloadEncrypted());
				Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets());
			}
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadEncryptedWithOkWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetInaccessiblePayloadsOnePayloadEncryptedWithBadWallet()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetInaccessiblePayloads(wallet2);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Single(info);
			Assert.Equal((uint)1, info[0].GetPayloadId());
			IPayload p = info[0].GetPayload();
			Assert.True(p.DataSize() > 0);
			Assert.True(p.GetInfo()!.GetPayloadEncrypted());
			Assert.Single(p.GetInfo()!.GetPayloadWallets());
			Assert.Equal(wallet, p.GetInfo()!.GetPayloadWallets()[0]);
		}
		[Fact]
		public void GetInaccessiblePayloadsMultiPayloadEncryptedWithWallets()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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

/* GetAccessiblePayloadsData() Tests */
		[Fact]
		public void GetAccessiblePayloadsDataNoPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Null(info);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencryptedRaw()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencryptedRawModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(info);
		}
		[Fact]
		public void GetAccessiblePayloadsDataOnePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, info) = tx.GetTxPayloadManager().GetAccessiblePayloadsData(pkey);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(info);
			Assert.Empty(info);
		}

/* ImportPayloads() Tests */
		[Fact]
		public void ImportPayloadsNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id) = tx.GetTxPayloadManager().ImportPayloads(null);
			Assert.Null(id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ImportPayloadsOnePayloadUnencryptedNoRecipientsEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet]);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.False(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadsOnePayloadEncryptedNoRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.False(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadsOnePayloadEncryptedBothEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet2]);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads(p);
			Assert.NotNull(id);
			Assert.Single(id);
			Assert.False(id[0]);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadsOnePayloadMultiWalletsReorderedEmptyTx()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Single(p);
			IPayloadInfo? info = p[0].GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = p[i].GetPayload().GetInfo();
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = p[i].GetPayload().GetInfo();
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = p[i].GetPayload().GetInfo();
				Assert.NotNull(info);
				Assert.False(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			var (result, p) = manager.GetAllPayloads();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal(TestData.Count, (int)manager.GetPayloadsCount());
			Assert.Equal((int)manager.GetPayloadsCount(), p.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				IPayloadInfo? info = p[i].GetPayload().GetInfo();
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
			}
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients(wallet);
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
				Assert.True(info2[TestData.Count + i].GetPayloadEncrypted());
				Assert.Equal((uint)wallet.Length, info2[TestData.Count + i].GetPayloadWalletsCount());
				Assert.Equal(wallet, info2[TestData.Count + i].GetPayloadWallets());
				Assert.True(info2[TestData.Count + i].GetPayloadSize() > 0);
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffWalletsUnencryptedEmptyTx()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1]);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2]);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads([p, p2, p3]);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(2, (int)manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(id[0]);
			Assert.True(id[2]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Equal(manager.GetPayloadsCount(), (uint)info2.Length);
			for (int i = 0; i < manager.GetPayloadsCount(); i++)
			{
				Assert.False(info2[i].GetPayloadEncrypted());
				Assert.Equal((uint)0, info2[i].GetPayloadWalletsCount());
				Assert.Empty(info2[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffWalletsEncryptedEmptyTx()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1]);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2]);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads([p, p2, p3]);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(1, (int)manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(id[1]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.Equal((uint)wallet.Length, info2[0].GetPayloadWalletsCount());
			Assert.Equal(wallet, info2[0].GetPayloadWallets());
		}
		[Fact]
		public void ImportPayloadsMultiPayloadMultiDiffPayloadWalletsEncryptedEmptyTx()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			string[] wallet3 = [
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet2);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[1]);
			var (result2, p2) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.NotNull(p2);
			info = p2.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[2]);
			var (result3, p3) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result3);
			Assert.NotNull(p3);
			info = p3.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayloads([p, p2, p3]);
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.Equal(1, (int)manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(id[2]);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Single(info2);
			Assert.True(info2[0].GetPayloadEncrypted());
			Assert.Equal((uint)wallet.Length, info2[0].GetPayloadWalletsCount());
			Assert.Equal(wallet, info2[0].GetPayloadWallets());
		}

/* ImportPayload() Tests */
		[Fact]
		public void ImportPayloadNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id) = tx.GetTxPayloadManager().ImportPayload(null);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ImportPayloadUnencryptedNoRecipientsEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo?info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet]);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_ACCESS_DENIED, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadEncryptedNoRecipientsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_ACCESS_DENIED, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadEncryptedBothEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients([wallet2]);
			manager = tx2.GetTxPayloadManager();
			var (status, id) = manager.ImportPayload(p);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_ACCESS_DENIED, status);
			IPayloadInfo[]? info2 = manager.GetPayloadsInfo().info;
			Assert.NotNull(info2);
			Assert.Empty(info2);
		}
		[Fact]
		public void ImportPayloadMultiWalletsReorderedEmptyTx()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx2.SetTxRecipients(wallet);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			var (result, p) = manager.GetPayload(1);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(p);
			Assert.Equal((uint)1, p.GetPayloadId());
			IPayloadInfo? info = p.GetPayload().GetInfo();
			Assert.NotNull(info);
			Assert.False(info.GetPayloadEncrypted());
			ITxFormat tx2 = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			manager = tx2.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id) = tx.GetTxPayloadManager().ReleasePayloads(null);
			Assert.Null(id);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}
		[Fact]
		public void ReleasePayloadsNoPayloadKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id) = tx.GetTxPayloadManager().ReleasePayloads(sender_pkey);
			Assert.Null(id);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}
		[Fact]
		public void ReleasePayloadsOnePayloadKey()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			tx.SetTxRecipients([wallet]);
			manager.AddPayload([0x00]);
			var (result, id) = manager.ReleasePayloads(sender_pkey);
			Assert.Null(id);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}

/* AddPayload() Tests */
		[Fact]
		public void AddPayloadNoDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id, _) = tx.GetTxPayloadManager().AddPayload(null);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void AddPayloadEmptyDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, id, _) = tx.GetTxPayloadManager().AddPayload([]);
			Assert.Equal((uint)0, id);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void AddPayloadSingleDataUnencryptedEmptyTx()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
		public void AddPayloadSingleDataEncryptedTxPayloads()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
				var (result, payload) = manager.GetPayload((uint)i + 1);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(payload);
				Assert.True(payload.GetPayload().VerifyHash());
			}
		}
		[Fact]
		public void AddPayloadMultiDataUnencryptedRebuildSingle()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
			var (result2, id2, _) = manager.AddPayload(TestData[0]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
			var (result2, id2, _) = manager.AddPayload(TestData[0]);
			Assert.Equal((uint)4, id2);
			Assert.Equal(Status.STATUS_OK, result2);
			Assert.Equal((uint)4, manager.GetPayloadsCount());
			IPayloadInfo? info2 = manager.GetPayloadInfo((uint)4).info;
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, payload) = tx.GetTxPayloadManager().GetPayload(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadZeroIdTxPayloadsUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.NotNull(payload);
			Assert.Equal(Status.STATUS_OK, result2);
			IPayload p = payload.GetPayload();
			Assert.True(p.VerifyHash());
			IPayloadInfo? info = p.GetInfo();
			Assert.NotNull(info);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
			Assert.Equal((uint)1, id);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, payload) = tx.GetTxPayloadManager().GetPayload(1);
			Assert.NotNull(payload);
			Assert.Equal(Status.STATUS_OK, result2);
			IPayload p = payload.GetPayload();
			Assert.True(p.VerifyHash());
			IPayloadInfo? info = p.GetInfo();
			Assert.NotNull(info);
			Assert.True(info.GetPayloadEncrypted());
			Assert.True(info.GetPayloadSize() > 0);
			Assert.Equal(wallet, info.GetPayloadWallets()[0]);
			Assert.Equal((uint)1, info.GetPayloadWalletsCount());
		}
		[Fact]
		public void GetPayloadMultiValidIdTxPayloadsUnencrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
				IPayloadInfo? info = p.GetInfo();
				Assert.NotNull(info);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, id, _) = manager.AddPayload(TestData[i]);
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
				IPayloadInfo? info = p.GetInfo();
				Assert.NotNull(info);
				Assert.True(info.GetPayloadEncrypted());
				Assert.True(info.GetPayloadSize() > 0);
				Assert.Equal(wallet, info.GetPayloadWallets());
				Assert.Equal((uint)wallet.Length, info.GetPayloadWalletsCount());
			}
		}
		[Fact]
		public void GetPayloadInvalidIdTxPayloadsUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, payload) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadDataZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, payload) = tx.GetTxPayloadManager().GetPayloadData(0);
			Assert.Null(payload);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void GetPayloadDataZeroIdPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(0);
			Assert.Null(data);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadDataBadIdTxPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(2);
			Assert.Null(data);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result2);
		}
		[Fact]
		public void GetPayloadDataValidIdRaw()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
			Assert.Null(data);
		}
		[Fact]
		public void GetPayloadDataValidIdRawModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.NotNull(data);
			Assert.Single(data);
			Assert.Equal(Status.STATUS_OK, result2);
			(result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			(result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(data);
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
		}
		[Fact]
		public void GetPayloadDataValidIdUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1);
			Assert.Null(data);
			Assert.Equal(Status.TX_ACCESS_DENIED, result2);
		}
		[Fact]
		public void GetPayloadDataUnencryptedWithKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result2, data) = tx.GetTxPayloadManager().GetPayloadData(1, pkey);
			Assert.Null(data);
			Assert.Equal(Status.TX_ACCESS_DENIED, result2);
		}
		[Fact]
		public void GetPayloadMultiPayloadDataUnencryptedNoKey()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
		public void GetPayloadMultiPayloadDataUnencryptedWithKey()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result, _, _) = manager.AddPayload(TestData[i]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string pkey = "BeE2mrxEynNwMQarT36sLUkB4nHXXyJBrqm4vssVqJGBKwTUVxFneTEmsHbehPoucd58xDkxGruJUNJhaTyxAmvpfa1ER8";
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
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
				var (result2, data) = tx.GetTxPayloadManager().GetPayloadData((uint)i + 1, pkey);
				Assert.Null(data);
				Assert.Equal(Status.TX_ACCESS_DENIED, result2);
			}
		}

/* RemovePayload() Tests */
		[Fact]
		public void RemovePayloadEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().RemovePayload(1);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void RemovePayloadZeroId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().RemovePayload(0);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void RemovePayloadZeroIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(0);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void RemovePayloadBadIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(2);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void RemovePayloadValidIdTxPayloads()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.RemovePayload(1);
			Assert.Equal((uint)0, manager.GetPayloadsCount());
			Assert.Equal(Status.STATUS_OK, result);
		}
		[Fact]
		public void RemovePayloadMultiPayloadTxPayloadsFirst()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, _, _) = manager.AddPayload(TestData[i]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var result = tx.GetTxPayloadManager().AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletBadId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var result = tx.GetTxPayloadManager().AddPayloadWallet(0, wallet);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletUnencrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var result = manager.AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.False(result.added);
		}
		[Fact]
		public void AddPayloadWalletEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var result = manager.AddPayloadWallet(1, wallet);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.False(result.added);
		}

/* AddPayloadWallets() Tests */
		[Fact]
		public void AddPayloadWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var result = tx.GetTxPayloadManager().AddPayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsBadId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var result = tx.GetTxPayloadManager().AddPayloadWallets(0, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsUnencrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var result = manager.AddPayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.Null(result.wallets);
		}
		[Fact]
		public void AddPayloadWalletsEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var result = manager.AddPayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result.result);
			Assert.Null(result.wallets);
		}

/* RemovePayloadWallet() Tests */
		[Fact]
		public void RemovePayloadWalletEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status status = tx.GetTxPayloadManager().RemovePayloadWallet(1, wallet).result;
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
		}
		[Fact]
		public void RemovePayloadWalletBadId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status status = tx.GetTxPayloadManager().RemovePayloadWallet(0, wallet).result;
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
		}
		[Fact]
		public void RemovePayloadWalletUnencrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status status = manager.RemovePayloadWallet(1, wallet).result;
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
		}
		[Fact]
		public void RemovePayloadWalletEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status status = manager.RemovePayloadWallet(1, wallet).result;
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
		}

/* RemovePayloadWallets() Tests */
		[Fact]
		public void RemovePayloadWalletsEmptyTx()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, wallets) = tx.GetTxPayloadManager().RemovePayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsBadId()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var (result, wallets) = tx.GetTxPayloadManager().RemovePayloadWallets(0, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsUnencrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var (result, wallets) = manager.RemovePayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
			Assert.Null(wallets);
		}
		[Fact]
		public void RemovePayloadWalletsEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			var (result, wallets) = manager.RemovePayloadWallets(1, [wallet]);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
			Assert.Null(wallets);
		}

/* ReleasePayload() Tests */
		[Fact]
		public void ReleasePayloadEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}
		[Fact]
		public void ReleasePayloadBadId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().ReleasePayload(0, sender_pkey);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}
		[Fact]
		public void ReleasePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}
		[Fact]
		public void ReleasePayloadEncrypted()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NOT_SUPPORTED, result);
		}

/* ReplacePayloadData() Tests */
		[Fact]
		public void ReplacePayloadDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(1, [0]);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataEmptyTxNoData()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(1, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataEmptyTxBadId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			Status result = tx.GetTxPayloadManager().ReplacePayloadData(0, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadNoData()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(1, null);
			Assert.Equal(Status.TX_NO_PAYLOAD, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadBadId()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(0, [0x00]);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void ReplacePayloadDataTxPayloadBadIdLast()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, _, _) = manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, result2);
			Status result = manager.ReplacePayloadData(2, [0x00]);
			Assert.Equal(Status.TX_BAD_PAYLOAD_ID, result);
		}
		[Fact]
		public void ReplacePayloadDataSinglePayloadUnencrypted()
		{
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
			];
			byte[] TestData = "Hello World"u8.ToArray();
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, id, _) = manager.AddPayload([0x00]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			byte[] TestData = "Hello World"u8.ToArray();
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result2, id, _) = manager.AddPayload([0x00]);
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
		public void ReplacePayloadDataMultiPayloadUnencrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e",
				"96b568ed60d41939ad364c238304e8e1ad547560204555e90cc297b3afef7102"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, id, _) = manager.AddPayload(TestData[i]);
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
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				var (result2, id, _) = manager.AddPayload(TestData[i]);
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

/* VerifyAllPayloadsData() Tests */
		[Fact]
		public void VerifyAllPayloadsDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			var result = tx.GetTxPayloadManager().VerifyAllPayloadsData();
			Assert.Equal(Status.TX_NO_PAYLOAD, result.result);
			Assert.Null(result.verification);
		}
		[Fact]
		public void VerifyAllPayloadsDataSinglePayloadUnencrypted()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			var (result, id, _) = manager.AddPayload([0x00]);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
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
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
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
	}
}
