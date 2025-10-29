// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

// TransactionV3 Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class TransactionV3Tests
	{
		private readonly string sender_pkey = "BpBmquFmWkz3tw6cF69WSoyY9Jw4aWUAsvZDCPZ4h52Da5Fcyso6hBEwSsT3p124CFWnVaJq4zZ7nLP2GU8nHSbnWByCvP";
		private readonly string prev_txid = "b798420963c36ac370486c1877f392df6805a15daf228b6ed9c0ec88c85507d3";
		private readonly string meta_data = "{\"RegisterId\":\"40daacacf4ef407cb5c4b9b7d0e7fe36\",\"TransactionType\":\"Action\",\"BlueprintId\":\"a72e679a89ecf611ee22fdfba7f1242e29545559ba422c81d856b492aadc0d81\",\"InstanceId\":\"8726415b-b443-4631-953e-a391cfbef632\",\"ActionId\":1,\"NextActionId\":1,\"TrackingData\":{}}";
		private readonly CryptoModule module = new();
		private readonly string? sender_address;
		
		public TransactionV3Tests()
		{
			byte Network = (byte)WalletNetworks.ED25519;
			sender_address = WalletUtils.PubKeyToWallet(module.CalculatePublicKey(Network, WalletUtils.WIFToPrivKey(sender_pkey)!.Value.PrivateKey).pubkey, Network);
		}

/* GetTxVersion() Tests */
		[Fact]
		public void GetTxVersionTest()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal((UInt32)3, tx.GetTxVersion());
		}

/* GetTxSender() Tests */
		[Fact]
		public void GetTxSenderEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderOnePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderOnePayloadSigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(sender);
			Assert.Equal(sender_address, sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxAddPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxImportPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			IPayloadContainer? container = tx.GetTxPayloadManager().GetPayload(1).payload;
			Assert.NotNull(container);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			tx.GetTxPayloadManager().ImportPayload(container);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxRemovePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			tx.GetTxPayloadManager().RemovePayload(1);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxReplacePayloadData()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			tx.GetTxPayloadManager().ReplacePayloadData(1, [0x00]);
			var (result, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxRawRebuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Null(tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			var (result2, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxSignedRebuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(sender_address, transaction.GetTxSender().sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxRawMultiRebuild()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Null(tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			var (result2, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxSignedMultiRebuild()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(sender_address, transaction.GetTxSender().sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxRawMultiRebuildEncrypted()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Null(tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			var (result2, sender) = tx.GetTxSender();
			Assert.Equal(Status.TX_NOT_SIGNED, result2);
			Assert.Null(sender);
		}
		[Fact]
		public void GetTxSenderModifiedTxSignedMultiRebuildEncrypted()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sender_address, tx.GetTxSender().sender);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(sender_address, transaction.GetTxSender().sender);
		}

/* GetTxRecipients() Tests */
		[Fact]
		public void GetTxRecipientsEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string[] recipients) = tx.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.Empty(recipients);
		}
		[Fact]
		public void GetTxRecipientsSingleRecipientNoPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([sender_address!]);
			(Status status, string[] recipients) = tx.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.Single(recipients);
			Assert.Equal(sender_address, recipients[0]);
		}
		[Fact]
		public void GetTxRecipientsSingleRecipientSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([sender_address!]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string[] recipients) = tx.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.Single(recipients);
			Assert.Equal(sender_address, recipients[0]);
		}
		[Fact]
		public void GetTxRecipientsMultiRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			(Status status, string[] recipients) = tx.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.Equal(wallet.Length, recipients.Length);
			Assert.Equal(wallet, recipients);
		}
		[Fact]
		public void GetTxRecipientsMultiRecipientsSinglePayload()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string[] recipients) = tx.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.Equal(wallet.Length, recipients.Length);
			Assert.Equal(wallet, recipients);
			(Status status2, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status3, string[] recipients2) = transaction.GetTxRecipients();
			Assert.Equal(Status.STATUS_OK, status3);
			Assert.Equal(wallet.Length, recipients2.Length);
			Assert.Equal(wallet, recipients2);
		}

/* GetTxTimeStamp() Tests */
		[Fact]
		public void GetTxTimeStampEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
		}
		[Fact]
		public void GetTxTimeStampUnsignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
		}
		[Fact]
		public void GetTxTimeStampUnsignedSinglePayloadRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
		}
		[Fact]
		public void GetTxTimeStampSignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(time2);
			Assert.NotEmpty(time2);
		}
		[Fact]
		public void GetTxTimeStampSignedSinglePayloadRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(time2);
			Assert.NotEmpty(time2);
		}
		[Fact]
		public void GetTxTimeStampSignedSinglePayloadModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(time2);
			Assert.NotEmpty(time2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status3, string? time3) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status3);
			Assert.Null(time3);
		}
		[Fact]
		public void GetTxTimeStampSignedSinglePayloadRecipientsModified()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(time2);
			Assert.NotEmpty(time2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status3, string? time3) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status3);
			Assert.Null(time3);
		}
		[Fact]
		public void GetTxTimeStampUnsignedSinglePayloadRebuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			(Status status2, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status3, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status3);
			Assert.Null(time2);
		}
		[Fact]
		public void GetTxTimeStampSignedSinglePayloadRebuild()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? time) = tx.GetTxTimeStamp();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(time);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, string? time2) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(time2);
			Assert.NotEmpty(time2);
			(Status status3, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status3);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status4, string? time3) = tx.GetTxTimeStamp();
			Assert.Equal(Status.STATUS_OK, status4);
			Assert.NotNull(time3);
			Assert.NotEmpty(time3);
		}

/* GetPrevTxHash() Tests */
		[Fact]
		public void GetPrevTxHashEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(TxDefs.NullTxId, hash);
		}
		[Fact]
		public void GetPrevTxHashUnsignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(TxDefs.NullTxId, hash);
		}
		[Fact]
		public void GetPrevTxHashSignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(TxDefs.NullTxId, hash);
		}
		[Fact]
		public void GetPrevTxHashUnsignedMultiPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(TxDefs.NullTxId, hash);
		}
		[Fact]
		public void GetPrevTxHashUnsignedMultiPayloadPrTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(prev_txid, hash);
		}
		[Fact]
		public void GetPrevTxHashSignedMultiPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(TxDefs.NullTxId, hash);
		}
		[Fact]
		public void GetPrevTxHashSignedMultiPayloadPrTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetPrevTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(prev_txid, hash);
		}

/* GetTxMetaData() Tests */
		[Fact]
		public void GetTxMetaDataEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.TX_BAD_METADATA, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxMetaDataUnsignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.TX_BAD_METADATA, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxMetaDataSignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.TX_BAD_METADATA, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxMetaDataUnsignedMultiPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.TX_BAD_METADATA, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxMetaDataSignedMultiPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.TX_BAD_METADATA, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxMetaDataUnsignedMultiPayloadSetRandomMeta()
		{
			string meta = "{\"Hello\":\"World\"}";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			tx.SetTxMetaData(meta);
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(meta, hash);
		}
		[Fact]
		public void GetTxMetaDataSignedMultiPayloadSetRandomMeta()
		{
			string meta = "{\"Hello\":\"World\"}";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			tx.SetTxMetaData(meta);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.Equal(meta, hash);
		}
		[Fact]
		public void GetTxMetaDataUnsignedMultiPayloadSetValidMeta()
		{
			string rid = "40daacacf4ef407cb5c4b9b7d0e7fe36";
			string bid = "a72e679a89ecf611ee22fdfba7f1242e29545559ba422c81d856b492aadc0d81";
			string iid = "8726415b-b443-4631-953e-a391cfbef632";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			TransactionMetaData? meta = JsonSerializer.Deserialize<TransactionMetaData>(hash);
			Assert.NotNull(meta);
			Assert.Equal(meta.RegisterId, rid);
			Assert.Equal(meta.BlueprintId, bid);
			Assert.Equal(meta.InstanceId, iid);
		}
		[Fact]
		public void GetTxMetaDataSignedMultiPayloadSetValidMeta()
		{
			string rid = "40daacacf4ef407cb5c4b9b7d0e7fe36";
			string bid = "a72e679a89ecf611ee22fdfba7f1242e29545559ba422c81d856b492aadc0d81";
			string iid = "8726415b-b443-4631-953e-a391cfbef632";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 10; i++)
				manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxMetaData();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			TransactionMetaData? meta = JsonSerializer.Deserialize<TransactionMetaData>(hash);
			Assert.NotNull(meta);
			Assert.Equal(meta.RegisterId, rid);
			Assert.Equal(meta.BlueprintId, bid);
			Assert.Equal(meta.InstanceId, iid);
		}

/* GetTxHash() Tests */
		[Fact]
		public void GetTxHashEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxHashUnsignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxHashSignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
		}
		[Fact]
		public void GetTxHashUnsignedMultiPayload()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxHashSignedMultiPayload()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
		}
		[Fact]
		public void GetTxHashUnsignedMultiPayloadRecipients()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
		}
		[Fact]
		public void GetTxHashSignedMultiPayloadRecipients()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
		}
		[Fact]
		public void GetTxHashUnsignedMultiPayloadRecipientsRebuild()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
			(Status status2, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status3, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status3);
			Assert.Null(hash2);
		}
		[Fact]
		public void GetTxHashSignedMultiPayloadRecipientsRebuild()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
			(Status status3, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status3);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status2, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(hash2);
			Assert.NotEmpty(hash2);
		}
		[Fact]
		public void GetTxHashUnsignedMultiPayloadRecipientsModified()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
			manager.AddPayload([0x00]);
			(Status status2, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(hash2);
		}
		[Fact]
		public void GetTxHashSignedMultiPayloadRecipientsModified()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
			manager.AddPayload([0x00]);
			(Status status2, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(hash2);
		}
		[Fact]
		public void GetTxHashUnsignedMultiPayloadRecipientsChanged()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(hash);
			tx.SetTxRecipients(wallet);
			(Status status2, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(hash2);
		}
		[Fact]
		public void GetTxHashSignedMultiPayloadRecipientsChanged()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? hash) = tx.GetTxHash();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(hash);
			Assert.NotEmpty(hash);
			tx.SetTxRecipients(wallet);
			(Status status2, string? hash2) = tx.GetTxHash();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(hash2);
		}

/* GetTxSignature() Tests */
		[Fact]
		public void GetTxSignatureEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
		}
		[Fact]
		public void GetTxSignatureUnsignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
		}
		[Fact]
		public void GetTxSignatureSignedSinglePayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
		}
		[Fact]
		public void GetTxSignatureUnsignedMultiPayload()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
		}
		[Fact]
		public void GetTxSignatureSignedMultiPayload()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
		}
		[Fact]
		public void GetTxSignatureUnsignedMultiPayloadRecipients()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
		}
		[Fact]
		public void GetTxSignatureSignedMultiPayloadRecipients()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
		}
		[Fact]
		public void GetTxSignatureUnsignedMultiPayloadRecipientsRebuild()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
			(Status status2, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status3, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status3);
			Assert.Null(signature2);
		}
		[Fact]
		public void GetTxSignatureSignedMultiPayloadRecipientsRebuild()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
			(Status status3, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status3);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(Status status2, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(signature2);
			Assert.NotEmpty(signature2);
		}
		[Fact]
		public void GetTxSignatureUnsignedMultiPayloadRecipientsModified()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
			manager.AddPayload([0x00]);
			(Status status2, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(signature2);
		}
		[Fact]
		public void GetTxSignatureSignedMultiPayloadRecipientsModified()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
			manager.AddPayload([0x00]);
			(Status status2, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(signature2);
		}
		[Fact]
		public void GetTxSignatureUnsignedMultiPayloadRecipientsChanged()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status);
			Assert.Null(signature);
			tx.SetTxRecipients(wallet);
			(Status status2, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(signature2);
		}
		[Fact]
		public void GetTxSignatureSignedMultiPayloadRecipientsChanged()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? signature) = tx.GetTxSignature();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
			tx.SetTxRecipients(wallet);
			(Status status2, string? signature2) = tx.GetTxSignature();
			Assert.Equal(Status.TX_NOT_SIGNED, status2);
			Assert.Null(signature2);
		}

/* SetTxRecipients() Tests */
		[Fact]
		public void SetTxRecipientsNull()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(null);
			Assert.Equal(Status.TX_INVALID_WALLET, status);
			Assert.Null(recipients);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Empty(r);
		}
		[Fact]
		public void SetTxRecipientsEmpty()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Empty(recipients);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Empty(r);
		}
		[Fact]
		public void SetTxRecipientsSingleValid()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			Assert.True(recipients[0]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet, r[0]);
		}
		[Fact]
		public void SetTxRecipientsSingleCorrupt()
		{
			string wallet = "ts1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			Assert.False(recipients[0]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Empty(r);
		}
		[Fact]
		public void SetTxRecipientsSetAfterPayload()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Single(r);
			Assert.Equal(wallet, r[0]);
		}
		[Fact]
		public void SetTxRecipientsMulti()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], r[i]);
		}
		[Fact]
		public void SetTxRecipientsMultiDuplicates()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz" ];
			string[] res_wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] results = [true, true, false, true, false];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(results[i], recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(res_wallet.Length, r.Length);
			for (int i = 0; i < res_wallet.Length; i++)
				Assert.Equal(res_wallet[i], r[i]);
		}
		[Fact]
		public void SetTxRecipientsMultiMixedDuplicatesCorrupt()
		{
			string[] wallet = [
				"rs1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"rs1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			string[] res_wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] results = [false, true, true, false, true, false, false];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(results[i], recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(res_wallet.Length, r.Length);
			for (int i = 0; i < res_wallet.Length; i++)
				Assert.Equal(res_wallet[i], r[i]);
		}
		[Fact]
		public void SetTxRecipientsOneRecipientSameReplace()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			Assert.True(recipients[0]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Single(r);
			Assert.Equal(wallet, r[0]);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Single(recipients2);
			Assert.True(recipients2[0]);
			(_, string[] r2) = tx.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Single(r2);
			Assert.Equal(wallet, r2[0]);
		}
		[Fact]
		public void SetTxRecipientsOneRecipientDifferentReplace()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			Assert.True(recipients[0]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Single(r);
			Assert.Equal(wallet, r[0]);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients([wallet2]);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Single(recipients2);
			Assert.True(recipients2[0]);
			(_, string[] r2) = tx.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Single(r2);
			Assert.Equal(wallet2, r2[0]);
		}
		[Fact]
		public void SetTxRecipientsMultiRecipientsSameReplace()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			Assert.Equal(wallet, r);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Equal(wallet.Length, recipients2.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients2[i]);
			(_, string[] r2) = tx.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Equal(wallet.Length, r2.Length);
			Assert.Equal(wallet, r2);
		}
		[Fact]
		public void SetTxRecipientsMultiRecipientsDifferentReplace()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jnzlgmqaewq09439t4wja6ekm26hup4l9mfagwmz30l670ctdks7qfu28vm",
				"ws1jaekdjgc2nvzzn9da5qedfq8nmvwrm9n6w8t8d3jqdsre0nhucr6qrn7swa",
				"ws1jfd3sk9el68pcuc5slda98hrpytptjnvt58gvzmj9h86866vw84sq8w4pd9" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			Assert.Equal(wallet, r);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients(wallet2);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Equal(wallet2.Length, recipients2.Length);
			for (int i = 0; i < wallet2.Length; i++)
				Assert.True(recipients2[i]);
			(_, string[] r2) = tx.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Equal(wallet2.Length, r2.Length);
			Assert.Equal(wallet2, r2);
		}
		[Fact]
		public void SetTxRecipientsMultiRecipientsSameReordered()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string wallet2 = "ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			Assert.Equal(wallet, r);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients([wallet2]);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Single(recipients2);
			Assert.True(recipients2[0]);
			(_, string[] r2) = tx.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Single(r2);
			Assert.Equal(wallet2, r2[0]);
		}
		[Fact]
		public void SetTxRecipientsSingleValidRebuild()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients([wallet]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Single(recipients);
			Assert.True(recipients[0]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet, r[0]);
			(Status status2, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(_, string[] r2) = transaction.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Equal(wallet, r2[0]);
		}
		[Fact]
		public void SetTxRecipientsMultiValidRebuild()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], r[i]);
			(Status status3, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status3);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			(_, string[] r2) = transaction.GetTxRecipients();
			Assert.NotNull(r2);
			Assert.Equal(wallet, r2);
		}
		[Fact]
		public void SetTxRecipientsResetRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet.Length, r.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], r[i]);
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients([]);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Empty(recipients2);
		}
		[Fact]
		public void SetTxRecipientsSameSignature()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients(wallet2);
			Assert.Equal(Status.STATUS_OK, status2);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet2.Length, r.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.Equal(wallet[i], r[i]);
			Assert.NotNull(recipients2);
			Assert.NotNull(tx.GetTxSender().sender);
			Assert.NotNull(tx.GetTxHash().hash);
		}
		[Fact]
		public void SetTxRecipientsDiffSignature()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] wallet2 = [
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			for (int i = 0; i < wallet.Length; i++)
				Assert.True(recipients[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients(wallet2);
			Assert.Equal(Status.STATUS_OK, status2);
			(_, string[] r) = tx.GetTxRecipients();
			Assert.NotNull(r);
			Assert.Equal(wallet2.Length, r.Length);
			for (int i = 0; i < wallet2.Length; i++)
				Assert.Equal(wallet2[i], r[i]);
			Assert.NotNull(recipients2);
			Assert.Null(tx.GetTxSender().sender);
			Assert.Null(tx.GetTxHash().hash);
		}
		[Fact]
		public void SetTxRecipientsResetNoRecipients()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, bool[]? recipients) = tx.SetTxRecipients([]);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Empty(recipients);
			Assert.NotNull(tx.GetTxSender().sender);
			Assert.NotNull(tx.GetTxHash().hash);
		}
		[Fact]
		public void SetTxRecipientsResetMultiRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, bool[]? recipients) = tx.SetTxRecipients(wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(recipients);
			Assert.Equal(wallet.Length, recipients.Length);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, bool[]? recipients2) = tx.SetTxRecipients([]);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(recipients2);
			Assert.Empty(recipients2);
			Assert.Null(tx.GetTxSender().sender);
			Assert.Null(tx.GetTxHash().hash);
		}

/* SetTxMetaData() Tests */
		[Fact]
		public void SetTxMetaDataNull()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_BAD_METADATA, tx.SetTxMetaData(null));
		}
		[Fact]
		public void SetTxMetaDataEmpty()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_BAD_METADATA, tx.SetTxMetaData(string.Empty)); ;
		}
		[Fact]
		public void SetTxMetaDataValid()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData("{\"Test\":\"Data\"}"));
		}
		[Fact]
		public void SetTxMetaDataInvalid()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_BAD_METADATA, tx.SetTxMetaData("Hello World"));
		}
		[Fact]
		public void SetTxMetaDataResetSign()
		{
			string meta = "{\"Test\":\"Data\"}";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.Null(transport.Id);
			Assert.NotNull(transport.Data);
		}
		[Fact]
		public void SetTxMetaDataSameSign()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotNull(transport.Data);
		}
		[Fact]
		public void SetTxMetaDataSameSignInvalid()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.TX_BAD_METADATA, tx.SetTxMetaData("Hello World"));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotNull(transport.Data);
		}

/* SetPrevTxHash() Tests */
		[Fact]
		public void SetPrevTxHashNull()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(null));
			Assert.Equal(TxDefs.NullTxId, tx.GetPrevTxHash().hash);
		}
		[Fact]
		public void SetPrevTxHashEmpty()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(string.Empty));
			Assert.Equal(TxDefs.NullTxId, tx.GetPrevTxHash().hash);
		}
		[Fact]
		public void SetPrevTxHashValid()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(prev_txid, tx.GetPrevTxHash().hash);
		}
		[Fact]
		public void SetPrevTxHashInvalid()
		{
			string hash = "0000";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_INVALID_PREVTX_HASH, tx.SetPrevTxHash(hash));
			Assert.Equal(TxDefs.NullTxId, tx.GetPrevTxHash().hash);
		}
		[Fact]
		public void SetPrevTxHashValidInvalid()
		{
			string hash = "0000";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.TX_INVALID_PREVTX_HASH, tx.SetPrevTxHash(hash));
			Assert.Equal(prev_txid, tx.GetPrevTxHash().hash);
		}
		[Fact]
		public void SetPrevTxHashSignedModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.Null(transport.Id);
		}
		[Fact]
		public void SetPrevTxHashSignedSamePrTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
		}
		[Fact]
		public void SetPrevTxHashSignedResetPrTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(null));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.Null(transport.Id);
			Assert.Equal(TxDefs.NullTxId, tx.GetPrevTxHash().hash);
		}

/* SignTx() Tests */
		[Fact]
		public void SignTxNullModule()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_NO_PAYLOAD, tx.SignTx(null!));
		}
		[Fact]
		public void SignTxEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_NO_PAYLOAD, tx.SignTx(sender_pkey));
		}
		[Fact]
		public void SignTxAlreadySigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			string? sig = tx.GetTxSignature().signature;
			Assert.NotNull(sig);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(sig, tx.GetTxSignature().signature);
		}
		[Fact]
		public void SignTxSignWithBadReSign()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			Assert.NotEmpty(sig);
			Assert.NotEmpty(hash);
			Assert.NotEmpty(ts);
			Assert.NotEmpty(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
		}
		[Fact]
		public void SignTxRebuildReSign()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			Assert.NotEmpty(sig);
			Assert.NotEmpty(hash);
			Assert.NotEmpty(ts);
			Assert.NotEmpty(send);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			string? sig2 = transaction.GetTxSignature().signature;
			string? hash2 = transaction.GetTxHash().hash;
			string? ts2 = transaction.GetTxTimeStamp().timestamp;
			string? send2 = transaction.GetTxSender().sender;
			Assert.NotNull(sig2);
			Assert.NotNull(hash2);
			Assert.NotNull(ts2);
			Assert.NotNull(send2);
			Assert.NotEmpty(sig2);
			Assert.NotEmpty(hash2);
			Assert.NotEmpty(ts2);
			Assert.NotEmpty(send2);
			Assert.Equal(sig, sig2);
			Assert.Equal(hash, hash2);
			Assert.Equal(ts, ts2);
			Assert.Equal(send, send2);
		}
		[Fact]
		public void SignTxModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			manager.AddPayload([0x00]);
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			IPayloadContainer? c = manager.GetPayload(1).payload;
			Assert.NotNull(c);
			Assert.Equal(Status.STATUS_OK, manager.ImportPayload(c).result);
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, manager.RemovePayload(2));
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, manager.ReplacePayloadData(1, [0x00]));
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
		}
		[Fact]
		public void SignTxPayloadWalletModified()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, bool added) = manager.AddPayloadWallet(1, wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.True(added);
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, bool removed) = manager.RemovePayloadWallet(1, wallet);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.True(removed);
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
		}
		[Fact]
		public void SignTxPayloadWalletsModified()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, bool[]? added) = manager.AddPayloadWallets(1, wallet);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(added);
			Assert.Equal(wallet.Length, added.Length);
			for (int i = 0; i < added.Length; i++)
				Assert.True(added[i]);
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, bool[]? removed) = manager.RemovePayloadWallets(1, wallet);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(removed);
			Assert.Equal(wallet.Length, removed.Length);
			for (int i = 0; i < removed.Length; i++)
				Assert.True(removed[i]);
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
		}
		[Fact]
		public void SignTxPayloadWalletsReleasePayload()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Status status = manager.ReleasePayload(1, sender_pkey);
			Assert.Equal(Status.TX_NOT_ENCRYPTED, status);
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			manager.AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Status status2 = manager.ReleasePayload(2, sender_pkey);
			Assert.Equal(Status.STATUS_OK, status2);
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
		}
		[Fact]
		public void SignTxPayloadWalletsReleasePayloads()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string wallet2 = "ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00], wallet);
			manager.AddPayload([0x00], [wallet2]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, UInt32[]? ids) = manager.ReleasePayloads(sender_pkey);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(ids);
			Assert.Single(ids);
			Assert.Equal((uint)1, ids[0]);
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status2, UInt32[]? ids2) = manager.ReleasePayloads(sender_pkey);
			Assert.Equal(Status.STATUS_OK, status2);
			Assert.NotNull(ids2);
			Assert.Empty(ids2);
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			Assert.NotEmpty(sig);
			Assert.NotEmpty(hash);
			Assert.NotEmpty(ts);
			Assert.NotEmpty(send);
		}

/* VerifyTx() Tests */
		[Fact]
		public void VerifyTxNullModule()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxWithPayload()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxUnInitVerifySM()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxInitVerifySMWrongPubKey()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxMultiPayloadUnencrypted()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxMultiPayloadEncrypted()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxMultiPayloadUnencryptedRebuild()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(Status.STATUS_OK, transaction.VerifyTx());
		}
		[Fact]
		public void VerifyTxMultiPayloadEncryptedRebuild()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(Status.STATUS_OK, transaction.VerifyTx());
		}
		[Fact]
		public void VerifyTxModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxSignWithEmptyReSign()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			Assert.NotEmpty(sig);
			Assert.NotEmpty(hash);
			Assert.NotEmpty(ts);
			Assert.NotEmpty(send);
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
		}
		[Fact]
		public void VerifyTxRebuildReSign()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.NotNull(sig);
			Assert.NotNull(hash);
			Assert.NotNull(ts);
			Assert.NotNull(send);
			Assert.NotEmpty(sig);
			Assert.NotEmpty(hash);
			Assert.NotEmpty(ts);
			Assert.NotEmpty(send);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transaction);
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			string? sig2 = transaction.GetTxSignature().signature;
			string? hash2 = transaction.GetTxHash().hash;
			string? ts2 = transaction.GetTxTimeStamp().timestamp;
			string? send2 = transaction.GetTxSender().sender;
			Assert.NotNull(sig2);
			Assert.NotNull(hash2);
			Assert.NotNull(ts2);
			Assert.NotNull(send2);
			Assert.NotEmpty(sig2);
			Assert.NotEmpty(hash2);
			Assert.NotEmpty(ts2);
			Assert.NotEmpty(send2);
			Assert.Equal(sig, sig2);
			Assert.Equal(hash, hash2);
			Assert.Equal(ts, ts2);
			Assert.Equal(send, send2);
		}
		[Fact]
		public void VerifyTxTxModified()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			manager.AddPayload([0x00]);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
			string? sig = tx.GetTxSignature().signature;
			string? hash = tx.GetTxHash().hash;
			string? ts = tx.GetTxTimeStamp().timestamp;
			string? send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			IPayloadContainer? c = manager.GetPayload(1).payload;
			Assert.NotNull(c);
			Assert.Equal(Status.STATUS_OK, manager.ImportPayload(c).result);
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, manager.RemovePayload(2));
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			Assert.Equal(Status.STATUS_OK, manager.ReplacePayloadData(1, [0x00]));
			Assert.Equal(Status.TX_NOT_SIGNED, tx.VerifyTx());
			sig = tx.GetTxSignature().signature;
			hash = tx.GetTxHash().hash;
			ts = tx.GetTxTimeStamp().timestamp;
			send = tx.GetTxSender().sender;
			Assert.Null(sig);
			Assert.Null(hash);
			Assert.Null(ts);
			Assert.Null(send);
		}

/* ToJSON() Tests */
		[Fact]
		public void ToJSONEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, string? JSON) = tx.ToJSON();
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
			Assert.Null(JSON);
		}
		[Fact]
		public void ToJSONSinglePayloadUnsigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, string? JSON) = tx.ToJSON();
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
			Assert.Null(JSON);
		}
		[Fact]
		public void ToJSONSinglePayloadSigned()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? JSON) = tx.ToJSON();
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
			Assert.Null(JSON);
		}
		[Fact]
		public void ToJSONMultiPayloadUnsigned()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			(Status status, string? JSON) = tx.ToJSON();
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
			Assert.Null(JSON);
		}
		[Fact]
		public void ToJSONMultiPayloadSigned()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			for (int i = 0; i < TestData.Count; i++)
				tx.GetTxPayloadManager().AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
			(Status status, string? JSON) = tx.ToJSON();
			Assert.Equal(Status.TX_NOT_SUPPORTED, status);
			Assert.Null(JSON);
		}

/* GetTxTransport() Tests */
		[Fact]
		public void GetTransportTxEmptyTx()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.TX_NO_PAYLOAD, status);
			Assert.Null(transport);
		}
		[Fact]
		public void GetTransportTxOnlyRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.TX_NO_PAYLOAD, status);
			Assert.Null(transport);
		}
		[Fact]
		public void GetTransportTxSinglePayloadRawVersion()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Data);
			Assert.Equal((uint)0x03000080, BitConverter.ToUInt32(transport.Data, 0));
			Assert.Null(transport.RegisterId);
			Assert.Null(transport.Id);
		}
		[Fact]
		public void GetTransportTxSinglePayloadRawRecipients()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Data);
			Assert.Equal((uint)0, WalletUtils.ReadVLSize(transport.Data, 37));
			Assert.Null(transport.RegisterId);
			Assert.Null(transport.Id);
		}
		[Fact]
		public void GetTransportTxSinglePayloadRawMultiRecipients()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Data);
			BinaryReader r = new(new MemoryStream(transport.Data));
			r.ReadUInt32();
			WalletUtils.ReadVLArray(r);
			UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
			Assert.Equal((uint)wallet.Length, c);
			WalletUtils.ReadVLSize(r);
			for (int i = 0; i < c; i++)
				Assert.Equal(wallet[i], WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
			Assert.Null(transport.RegisterId);
			Assert.Null(transport.Id);
		}
		[Fact]
		public void GetTransportTxSinglePayloadRawTimeStamp()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status status, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Data);
			BinaryReader r = new(new MemoryStream(transport.Data));
			r.ReadUInt32();
			WalletUtils.ReadVLSize(r);
			Assert.Equal((uint)0, (UInt64)WalletUtils.ReadVLSize(r));
			Assert.Null(transport.RegisterId);
			Assert.Null(transport.Id);
		}
		[Fact]
		public void GetTransportTxMultiPayloadRawPayloadCount()
		{
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00]);
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				r.ReadUInt64();
				WalletUtils.ReadVLArray(r);
				Assert.Equal((uint)count, (UInt32)WalletUtils.ReadVLSize(r));
				Assert.Null(transport.RegisterId);
				Assert.Null(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadRawPayloadFields()
		{
			string? TestHash = null;
			UInt64 size = 0;
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
				{
					UInt32 id = tx.GetTxPayloadManager().AddPayload([0x00]).id;
					IPayloadInfo? info = tx.GetTxPayloadManager().GetPayloadInfo(id).info;
					Assert.NotNull(info);
					TestHash = WalletUtils.ByteArrayToHexString(info.GetPayloadHash());
					size = info.GetPayloadSize();
				}
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				r.ReadUInt64();
				WalletUtils.ReadVLArray(r);
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x01, (short)r.ReadUInt16());
					Assert.Equal((uint)0, (UInt32)WalletUtils.ReadVLSize(r));
					byte[] h = WalletUtils.ReadVLArray(r);
					Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(h));
					Assert.Equal(size, WalletUtils.ReadVLSize(r));
					h = r.ReadBytes((int)size);
					Assert.NotEmpty(h);
				}
				Assert.Null(transport.RegisterId);
				Assert.Null(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadRawPayloadFieldsEncrypted()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				tx.SetTxRecipients(wallet);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00], wallet);
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)wallet.Length, c);
				for (int j = 0; j < c; j++)
					Assert.Equal(wallet[j], WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				r.ReadUInt64();
				WalletUtils.ReadVLArray(r);
				c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x03, (short)r.ReadUInt16());
					UInt32 p = (UInt32)WalletUtils.ReadVLSize(r);
					WalletUtils.ReadVLSize(r);
					Assert.Equal((int)p, wallet.Length);
					for (int k = 0; k < p; k++)
					{
						WalletUtils.ReadVLSize(r);
						byte[] hs = WalletUtils.ReadVLArray(r);
						Assert.NotEmpty(hs);
						Assert.Equal((int)TxDefs.SHA256HashSize, hs.Length);
						Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					}
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				}
				Assert.Null(transport.RegisterId);
				Assert.Null(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadRawPayloadsEncryptedNoRecip()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00], wallet);
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				Assert.Equal((UInt32)0, (UInt32)WalletUtils.ReadVLSize(r));
				r.ReadUInt64();
				WalletUtils.ReadVLArray(r);
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x03, (short)r.ReadUInt16());
					UInt32 p = (UInt32)WalletUtils.ReadVLSize(r);
					WalletUtils.ReadVLSize(r);
					Assert.Equal((int)p, wallet.Length);
					for (int k = 0; k < p; k++)
					{
						WalletUtils.ReadVLSize(r);
						byte[] hs = WalletUtils.ReadVLArray(r);
						Assert.NotEmpty(hs);
						Assert.Equal((int)TxDefs.SHA256HashSize, hs.Length);
						Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					}
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				}
				Assert.Null(transport.RegisterId);
				Assert.Null(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadSignedPayloadFields()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				Assert.True(r.ReadUInt64() > 0);
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				Assert.Equal(sender_address, WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x00, (short)r.ReadUInt16());
					Assert.Equal((uint)0, (UInt32)WalletUtils.ReadVLSize(r));
					Assert.Equal(TestHash, WalletUtils.ByteArrayToHexString(WalletUtils.ReadVLArray(r)));
					Assert.Equal(new byte[] { 0x00 }, WalletUtils.ReadVLArray(r));
				}
				Assert.Null(transport.RegisterId);
				Assert.NotNull(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadSignPayloadFieldsEncrypted()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				tx.SetTxRecipients(wallet);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00], wallet);
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)wallet.Length, c);
				for (int j = 0; j < c; j++)
					Assert.Equal(wallet[j], WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				Assert.True(r.ReadUInt64() > 0);
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				Assert.Equal(sender_address, WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x03, (short)r.ReadUInt16());
					UInt32 p = (UInt32)WalletUtils.ReadVLSize(r);
					Assert.Equal((int)p, wallet.Length);
					WalletUtils.ReadVLSize(r);
					for (int k = 0; k < p; k++)
					{
						WalletUtils.ReadVLSize(r);
						byte[] hs = WalletUtils.ReadVLArray(r);
						Assert.NotEmpty(hs);
						Assert.Equal((int)TxDefs.SHA256HashSize, hs.Length);
						Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					}
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				}
				Assert.Null(transport.RegisterId);
				Assert.NotNull(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadSignPayloadFieldsEncryptedDiff()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string wallet2 = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				tx.SetTxRecipients([wallet2]);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00], wallet);
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				BinaryReader r = new(new MemoryStream(transport.Data));
				r.ReadUInt32();
				WalletUtils.ReadVLArray(r);
				Assert.Equal((uint)1, (UInt32)WalletUtils.ReadVLSize(r));
				WalletUtils.ReadVLSize(r);
				Assert.Equal(wallet2, WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				Assert.True(r.ReadUInt64() > 0);
				WalletUtils.ReadVLArray(r);
				WalletUtils.ReadVLSize(r);
				Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				Assert.Equal(sender_address, WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
				UInt32 c = (UInt32)WalletUtils.ReadVLSize(r);
				Assert.Equal((uint)count, c);
				for (int j = 0; j < c; j++)
				{
					WalletUtils.ReadVLSize(r);
					Assert.Equal((short)0x03, (short)r.ReadUInt16());
					UInt32 p = (UInt32)WalletUtils.ReadVLSize(r);
					WalletUtils.ReadVLSize(r);
					Assert.Equal((int)p, wallet.Length);
					for (int k = 0; k < p; k++)
					{
						WalletUtils.ReadVLSize(r);
						Assert.Equal(wallet[k], WalletUtils.PubKeyToWallet(WalletUtils.ReadVLArray(r), (byte)WalletNetworks.ED25519));
						Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					}
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
					Assert.NotEmpty(WalletUtils.ReadVLArray(r));
				}
				Assert.Null(transport.RegisterId);
				Assert.NotNull(transport.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadSignedRebuild()
		{
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00]);
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(transaction);
				Assert.Equal(Status.STATUS_OK, transaction.VerifyTx());
				(Status status2, Transaction? transport2) = transaction.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status2);
				Assert.NotNull(transport2);
				Assert.NotNull(transport2.Data);
				Assert.Equal(transport.Data, transport2.Data);
				Assert.Null(transport.RegisterId);
				Assert.NotNull(transport.Id);
				Assert.Null(transport2.RegisterId);
				Assert.NotNull(transport2.Id);
				Assert.Equal(transport.Id, transport2.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadRecipientsSignedRebuild()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				tx.SetTxRecipients(wallet);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00]);
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(transaction);
				Assert.Equal(Status.STATUS_OK, transaction.VerifyTx());
				(Status status2, Transaction? transport2) = transaction.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status2);
				Assert.NotNull(transport2);
				Assert.NotNull(transport2.Data);
				Assert.Equal(transport.Data, transport2.Data);
				Assert.Null(transport.RegisterId);
				Assert.NotNull(transport.Id);
				Assert.Null(transport2.RegisterId);
				Assert.NotNull(transport2.Id);
				Assert.Equal(transport.Id, transport2.Id);
			}
		}
		[Fact]
		public void GetTransportTxMultiPayloadRecipientsModifiedSignedRebuild()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			Random rnd = new();
			for (int i = 0; i < 10; i++)
			{
				ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
				tx.SetTxRecipients(wallet);
				int count = rnd.Next(1, 10);
				for (int j = 0; j < count; j++)
					tx.GetTxPayloadManager().AddPayload([0x00]);
				Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey));
				Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
				tx.GetTxPayloadManager().AddPayload([0x00]);
				(Status status, Transaction? transport) = tx.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status);
				Assert.NotNull(transport);
				Assert.NotNull(transport.Data);
				Assert.Null(transport.TxId);
				(Status result, ITxFormat? transaction) = TransactionBuilder.Build(transport);
				Assert.Equal(Status.STATUS_OK, result);
				Assert.NotNull(transaction);
				Assert.Equal(Status.TX_NOT_SIGNED, transaction.VerifyTx());
				(Status status2, Transaction? transport2) = transaction.GetTxTransport();
				Assert.Equal(Status.STATUS_OK, status2);
				Assert.NotNull(transport2);
				Assert.NotNull(transport2.Data);
				Assert.Null(transport2.TxId);
			}
		}
	}
}
