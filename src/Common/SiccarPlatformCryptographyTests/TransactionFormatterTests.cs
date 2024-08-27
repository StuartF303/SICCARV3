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

// TransactionFormatter Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using Siccar.Platform.Cryptography.TransactionV4;
using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class TransactionFormatterTests
	{
		private readonly string sender_pkey1 = "BhaUhsRxQ3zq5BNq7cFWxt7tMuLQ3AbmGDHNnxDVGDdLDVVTiBRxWGYxqhJuc75Dk4VefPWAgMKbaFrDQ3VFKtvu8FxHvo";
		private readonly string sender_address1 = "ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t";
		private readonly string sender_pkey2 = "qCrEo9o9pT6C7D1h5Qh3SmsSTSRAo6D4m8hLpBdzumcuoAFucV";
		private readonly string sender_address2 = "ws1cyzy68yx6dxdspavx4cctjkhn8gax4fdqztcfylf5uscr9v4n66tlygzmq80qy7vudry8h535dnepgujrgxxw6y58f4v2xyn4e7gxql4t4vkmjfdc";
		private readonly string sender_pkey3 =
			"T11AhGTot94bq7188AGsNh5FGHD5Hd7ShHErT4FvcT2HQHeFjJLug3hf2wD1tu1o" +
			"nsYhP3Am4CKQS3Ve55KMrY8ABAJywhUUjK6VhjvRCYMKXEs2rKX8uFykpTUirxZz" +
			"SiETsAfQBcGy4eMLJGWjLvDAGFmKt6GF3CxuNf9tdC4X8ki5jsag8iR4DvsS5C3B" +
			"KRiWzsSNbDiw2BTuWkNPGEEpyKGpMpGjnpxXXeN6VpkKSoRWcJ9C1ACphkDAuzwf" +
			"9nhTDJLEkxsnJahdnqAxtD9hXFxMcn6ArN76xfjK81387PkbPc4JBRqS6GVYkf8X" +
			"dLkeUxnzRE98zK3Zk2JiJcyzxjn4mfL4FbsRFUESJE9gK1iTnAuMn4gcN9i6Ue1w" +
			"EA2NyiMqRL8oYxnrq7o8bn4Roz2DQR8JWHY9PL1FnMcgjXxSnn2ZfiUuG7rXbHKo" +
			"cgogBEuwDcgiByAEAeFePprRP8o2MrdYeEZcgEYptE4heyqaPU2Tu2CdVjTrEabr" +
			"4LK8xCzs1PxpuHxGbijpQG4QFvWbaciDxTnYS1zT6gsxuhvZTaix4YcamwwQjy7g" +
			"7ZoZbTX1JfjHJbP4CKdVdwmmG1H8ZeReWEkwv4MWkZX67e2qyZZFkUkxDPgeCTMK" +
			"KUQovY8QUAPK9fsGxvv9eM957LYnGak7zBFiXXFHmmEG4FVKuboeVg3aLCK56zpr" +
			"gAwnUia54VMN8uN4NBvzdrg5Xm7ks5udg76jZL7NbpL1TSTTeUGj6wQ9EB4er9pA" +
			"UxPuZ4CxTbSrCTpvRqGq6ykfi3KbsWy56NCRFGdgxcPLqhYRV9VuqyALREQvpCCe" +
			"3VsCnfZ7Y8YYo5uVwaTZnv3P2CN9VEE2kZjNBD7C9V2ttmpVZMGoy1hviUYHSp4m" +
			"YEHdrAFBf4qepVsbXhd6Wt89wDo9oQHB9hdLZheKd772HEKRK3uzKSyHGemGA9cG" +
			"s6y63XbWojebqAw8tDT6cym7UhnQJJqJxHYgYEr2uAakGMVH2FwxpyL6Ec5VGvxc" +
			"Y2abQwoNjzm22T94ZC8KvPqqAuWFkpuyhJ6vKn4oZTkm6HSPdyeHaxKYe4a1oMLb" +
			"J1SavhqxZ1Sbewes8K83V2oH3JVAhu6GEvZDXCqubBETG1kGMhL4F856xThbZD7m" +
			"c3q9hd2YkkMnF5AtcbgPRFfd5WS7KtfJT3Y7afRUQjVarkJUaFRtn9NZd9QUiN6d" +
			"fZdZKTZERkw7jNg7Z8NTgPVGo86v5f1zoSP4kxiiJvGvuY3z1Ycm2AW1NqTUqGUW" +
			"VJ3otQsynLkNXJnt7ognMxj9e2fbXcyKe7PVt6CVJXNLpfv8zgMRRp5CtigAt8xJ" +
			"LhPJBG72pWhW5ijKNs6GJs2iEcUe3BiYkkfZZUCToXRj9uw486HsXNCtN2xhPZ3d" +
			"UvBGAmraMJnJMibksBfiGWML7qBbni6jLfvjzaGqN6Scy6s1WCdjoGtgBbvaBWmM" +
			"C7q92xBdvKMqqKkWvAvUuGmRb4j4nF2iNyHAdS91NSi1DRH8gcKdpA1vhcbrvcn6" +
			"h5pgn3HyPjqEVf1UX58hy2h6LrXkTVBPNSf25WFYyXAHzBeECyNctCjfAhAeJJ7z" +
			"wHs8nBwdnHhhGPHDHj2chjCvJ2WxyyxkyPtaq9PVzUij2eZiGeEtiypGdzquyeXs" +
			"x31fbo1u1vA6LqwgySBSPLPRN38KYmreqxYMBcfTV5crfzSzDT2AmY6vLhBncyJE" +
			"qJMtsKKQEt4tqmH6afDRPmEGFCoqU1Ny97tm8nMNjD5ZkAS2inA3m9MpsPNZXsFm" +
			"CEpm3bxKm1LuafEbqfiuDTzCW5Q1rJTzjGnCu5b753KCxoGh4Nq49MfKk31eqRAq" +
			"G5awHBW2oSHcDGgpFPKnsyb16UPzsod8toY4RBbw6yG7rQYU4rrUnVBo2dvmrxpr" +
			"i4PWWQFjQz25KbrESGjRPykzhRAqwr8FRwkEbFfyse5prfEr36ZUK9DviPhZjMVg" +
			"t8kcR2KL3G6ogaiXZFSv5bnckzyJPmDKNXSCuLBy6xTJC6Bt5LYvmAWZ9sQQMzQV" +
			"wXPNodJpdD2YNzkKJxbNe6EGSHzqiXXu5EKWp9xVixNuNkcpsuU6WfhfGcKYdva4" +
			"5RA8YHmUz1Md2KcUKT8Q8e3NA1jdedUCqX9oPraXMbCa3WtbJ6X76fJfNi2bEmpV" +
			"Vi3UKgMJTYYDqnPnge7cMUs4iBySipqfLLTZkFAmeg3xFEX47cCYFFhJyr1Ueorb" +
			"dd16n7LRJG1yWjjU3KdeGcV92a2rnddFAQV2nwcNbe2uoTKLWNgjXXRmDZAsi8xq" +
			"78YWoFTM3cDcW76fR8SoAZaCEDQa8RuCotKp16PBN6Q2yZsY1eEGh9qbKY1rVoPD" +
			"xiQzRadbAnhJ4cuxgSzp7GN1vT86pR78dw12ursc9heP9XMvs6E88w7u9NHzvXNv" +
			"1z9r4N1Fq82Ucc3np4yCJR6JYDx5Sctu6zMisYHhfoJirxVwmRLqbVoCcj6JTEmd" +
			"5EM2M42NGGvKzje5MxPZhyUwtrZAAcC3TfWn1kjUKe2Nd31HTXenzX6oFWRNXfxp" +
			"FAdNNDKiGFnY6h9doTGQm3aF49vGPFGMQviH9qnn6sN57mMcPrtUjhKHGdK8SYCU" +
			"pH7SKmbwVYiRpdF6YKKRL9WRiJm22TMWwLK3RHra799tktc7RELRn7EQWicDYJY5" +
			"yHQfxEoBXw93pujWFNgqmiCAQ3qRhCnow1cEUCpZoCGrPQJ5KdVVBCXJVYF83CFd" +
			"gKjkqQhqibAXQ1udUrp23bCHAedwRBVyT5zXgkN9yqmLLtQTko7Cz8eyN5s5TbwV" +
			"RV6S29URSznwJocaTFAf9X9Eitb4HuVyNRxkQ1YqEpnZ9aSZHi1tDpX5nXyt5BAb" +
			"jHM8h76ua7q73bCc3bQA7CTSzLTiDa1oEh2cf2MvJUyiLqfCJo8GHjbYmezDaxTq" +
			"rVqPcdFkAGYYMjf7Pj5sGfzgaKDApmo7yENcYeKz4VokSp9L3trLoBeGWqBvNgxU" +
			"VnYMATtuYpUsgh486s2KsSWDd7cNNNAoxSN7DuiuEb528WYwhsDB5gY4cA38Tosh" +
			"RUQKLYvHAVSvZoJBNdav5e7ds8fdGjCJmEhYgyGLQnfF6QwD84Te1wLAQFDiiy2R" +
			"k5uos3wUgiHV42dMe71hvv9kQC1vysXiSzrBLDxxZrFTJ8PUEjYcZmPwsiXrKP8L" +
			"JC9CiQtj1hnA6";
		private readonly string sender_address3 =
			"ws1hxzpqyzszsgpqzq8d3cftuf506xq3nz7fcu50g6fkgdznem34xe60ncwdtfnc" +
			"wh2wsxhw4crehxw72mk7ze3vaq76325mngaqq45qrjfxg29g0vj7yhjsx3h9uanw" +
			"dqvky8zs0q5swuyqefh48ag7rp3kt7052m54t78g9jwl5cmtse7duvj6nt6h03l8" +
			"0u5g455rhhztnjzxa6vahw2nkym72z99m3mlk00yjxsj0amt54czx5dd8x22jsza" +
			"6h6788fhrnpqql62lvnktc4xlehq2ukssqzqcezuj48tlrxv2tk5c5dz6gc390ys" +
			"f7d8clp6qv39y8425tdsf5e4n3g8aynvw2thxw3gf62w2xmq99nflf8ct4x3u4cf" +
			"dtfckwpt43ylgxuhe365gtk508ph9fcsncdczt43tl6gxadhfnvcr8y2xc34u36s" +
			"m0pdls28eq2g2an3a5pae7etkx4kkqk986gtkuhme86zxy8gf2vmyw2nddtpwddx" +
			"sat9jgvsxmx7736k0fr9q5un7kjfsmcxks3xy8nn7d0u2edaysyfvwum4d2nzls6" +
			"cxg72d8zrvzlmaj6c6v4s9gkg6kzu990cwrwh7jxk5dnuvtwkqwl6ct9s6pcayv3" +
			"uq0pfvfw7arz2tcz2zttltt2dfarff6vm97auk65je7ut785d8qy7zcdh748vszd" +
			"txzla6mjygcat0l2a20c8c6jgh8ykpa9p3r404pdwjkxqlks3k0n37y4uwzzj09m" +
			"jdkmmlw5mu6s7c36zjufrnjnx7ekaleuwkq6wvtrcj0vgmjpxld0308dlu3thhx8" +
			"9lu48ypqxqgqqynsuknk";
		private readonly string meta_data = "{\"RegisterId\":\"40daacacf4ef407cb5c4b9b7d0e7fe36\",\"TransactionType\":\"Action\",\"BlueprintId\":\"a72e679a89ecf611ee22fdfba7f1242e29545559ba422c81d856b492aadc0d81\",\"InstanceId\":\"8726415b-b443-4631-953e-a391cfbef632\",\"ActionId\":1,\"NextActionId\":1,\"TrackingData\":{}}";
		private readonly string meta_rid = "40daacacf4ef407cb5c4b9b7d0e7fe36";
		private readonly string meta_bid = "a72e679a89ecf611ee22fdfba7f1242e29545559ba422c81d856b492aadc0d81";
		private readonly string meta_iid = "8726415b-b443-4631-953e-a391cfbef632";
		private readonly string prev_txid = "b798420963c36ac370486c1877f392df6805a15daf228b6ed9c0ec88c85507d3";

		/* ToModel() Conversion V1 Tests Raw */
		[Fact]
		public void ToModelNoTransactionV1()
		{
			TransactionModel? model = TransactionFormatter.ToModel(null);
			Assert.Null(model);
		}
		[Fact]
		public void ToModelRawOnePayloadNoRecipientsV1()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal("00", model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelRawMultiPayloadNoRecipientsV1()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelRawOnePayloadOneRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelRawOnePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsV1()
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
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}

/* ToModel() Conversion V1 Tests Signed */
		[Fact]
		public void ToModelSignedOnePayloadNoRecipientsV1()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal("00", model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelSignedMultiPayloadNoRecipientsV1()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelSignedOnePayloadOneRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelSignedOnePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)1, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}

/* ToJSON() Conversion V1 Tests Raw */
		[Fact]
		public void ToJSONNoTransactionV1()
		{
			string json = TransactionFormatter.ToJSON(null);
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONRawOnePayloadNoRecipientsV1()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal((uint)1, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
				Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetString());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.Equal("00", a.Current.GetProperty(JSONKeys.Data).GetString());
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadNoRecipientsV1()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadOneRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsV1()
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
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToJSON() Conversion V1 Tests Signed */
		[Fact]
		public void ToJSONSignedOnePayloadNoRecipientsV1()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal((uint)1, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
				Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetString());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.Equal("00", a.Current.GetProperty(JSONKeys.Data).GetString());
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoRecipientsV1()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadOneRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToModel() Conversion V2 Tests Raw */
		[Fact]
		public void ToModelNoTransactionV2()
		{
			TransactionModel? model = TransactionFormatter.ToModel(null);
			Assert.Null(model);
		}
		[Fact]
		public void ToModelRawOnePayloadNoRecipientsV2()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal("00", model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelRawMultiPayloadNoRecipientsV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelRawOnePayloadOneRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelRawOnePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadEncryptedNoRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadNoEncryptionRecipientsV2()
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
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadEncryptionRecipientsModifiedV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.NotNull(model.Payloads[i].Hash);
				Assert.NotEmpty(model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
		}

/* ToModel() Conversion V2 Tests Signed */
		[Fact]
		public void ToModelSignedOnePayloadNoRecipientsV2()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal("00", model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelSignedMultiPayloadNoRecipientsV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelSignedOnePayloadOneRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelSignedOnePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadEncryptedNoRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadNoEncryptionRecipientsV2()
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
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadEncryptionRecipientsModifiedV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)2, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.NotNull(model.Payloads[i].Data);
				Assert.NotEmpty(model.Payloads[i].Data);
			}
		}

/* ToJSON() Conversion V2 Tests Raw */
		[Fact]
		public void ToJSONNoTransactionV2()
		{
			string json = TransactionFormatter.ToJSON(null);
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONEmptyTransactionV2()
		{
			string json = TransactionFormatter.ToJSON(new Transaction());
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONRawOnePayloadNoRecipientsV2()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal((uint)1, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
				Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetString());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.Equal("00", a.Current.GetProperty(JSONKeys.Data).GetString());
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadNoRecipientsV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadOneRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadEncryptedNoRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsNoEncryptionV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsEncryptionModifiedV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToJSON() Conversion V2 Tests Signed */
		[Fact]
		public void ToJSONSignedOnePayloadNoRecipientsV2()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal((uint)1, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
				Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetString());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.Equal("00", a.Current.GetProperty(JSONKeys.Data).GetString());
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoRecipientsV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadOneRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadEncryptedNoRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoEncryptionRecipientsV2()
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
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.False(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(WalletUtils.ByteArrayToHexString(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadEncryptionRecipientsModifiedV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)2, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToModel() Conversion V3 Tests Raw */
		[Fact]
		public void ToModelNoTransactionV3()
		{
			TransactionModel? model = TransactionFormatter.ToModel(null);
			Assert.Null(model);
		}
		[Fact]
		public void ToModelRawOnePayloadNoRecipientsNoCompNoEncV3()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(Convert.ToBase64String([0x00]), model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelRawMultiPayloadNoRecipientsNoCompNoEncV3()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(Convert.ToBase64String(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelRawOnePayloadOneRecipientNoCompV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet], new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelRawOnePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsCompressionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0003", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsCompNoCryptV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { EType = EncryptionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0001", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsPrIdMetaV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.SetPrevTxHash(prev_txid);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { EType = EncryptionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0001", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.Equal(meta_rid, model.MetaData?.RegisterId);
			Assert.Equal(meta_bid, model.MetaData?.BlueprintId);
			Assert.Equal(meta_iid, model.MetaData?.InstanceId);
		}

/* ToModel() Conversion V3 Tests Signed */
		[Fact]
		public void ToModelSignedOnePayloadNoRecipientsV3()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x0000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(Convert.ToBase64String([0x00]), model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelSignedMultiPayloadNoRecipientsV3()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x0000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(Convert.ToBase64String(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelSignedOnePayloadOneRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet], new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelSignedOnePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x0002", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0002", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsCompressionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0003", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsCompNoCryptV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { EType = EncryptionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0001", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsPrIdMeta()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.SetPrevTxHash(prev_txid);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { EType = EncryptionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)3, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x0001", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
			}
			Assert.Equal(meta_rid, model.MetaData?.RegisterId);
			Assert.Equal(meta_bid, model.MetaData?.BlueprintId);
			Assert.Equal(meta_iid, model.MetaData?.InstanceId);
		}

/* ToJSON() Conversion V3 Tests Raw */
		[Fact]
		public void ToJSONNoTransactionV3()
		{
			string json = TransactionFormatter.ToJSON(null);
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONEmptyTransactionV3()
		{
			string json = TransactionFormatter.ToJSON(new Transaction());
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONRawOnePayloadNoRecipientsV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
				Assert.Equal("0x0001", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadNoRecipientsV3()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal("0x0001", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadOneRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadEncryptedNoRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsNoEncryptionV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal("0x0001", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsEncryptionModifiedV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsNoCompPrevIdMetaV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(meta_rid, d.GetProperty(JSONKeys.MetaData).GetProperty(JSONKeys.Register).GetString());
				Assert.Equal(meta_bid, d.GetProperty(JSONKeys.MetaData).GetProperty("BlueprintId").GetString());
				Assert.Equal(meta_iid, d.GetProperty(JSONKeys.MetaData).GetProperty("InstanceId").GetString());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToJSON() Conversion V3 Tests Signed */
		[Fact]
		public void ToJSONSignedOnePayloadNoRecipientsV3()
		{
			string TestHash = "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal((uint)1, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetString());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.Equal(Convert.ToBase64String([0x00]), a.Current.GetProperty(JSONKeys.Data).GetString());
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoRecipientsV3()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(Convert.ToBase64String(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadOneRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				b.MoveNext();
				Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal("0x0002", a.Current.GetProperty(JSONKeys.Flags).GetString());
				Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetString());
				}
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
				b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
				Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadEncryptedNoRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.Equal("0x0002", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.True(a.Current.TryGetProperty(JSONKeys.Hash, out _));
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetString());
					}
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Challenges).GetArrayLength());
					b = a.Current.GetProperty(JSONKeys.Challenges).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.NotEmpty(b.Current.GetProperty(JSONKeys.Hex).GetString()!);
						Assert.True(b.Current.GetProperty(JSONKeys.Size).GetUInt32() > 1);
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Hex).GetString()!);
					Assert.True(a.Current.GetProperty(JSONKeys.IV).GetProperty(JSONKeys.Size).GetUInt32() > 1);
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoEncryptionRecipientsV3()
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
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(Convert.ToBase64String(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadEncryptionRecipientsModifiedV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], [wallet]).id;
				Assert.True(manager.RemovePayloadWallet(id, wallet).removed);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal("0x0003", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Hash).GetString()!);
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoEncryptionRecipientsNoCompPrevIdMetaV3()
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
			string[] TestHash =
			[
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal((uint)3, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetString());
				}
				Assert.NotEmpty(d.GetProperty(JSONKeys.TxId).GetString()!);
				Assert.NotEmpty(d.GetProperty(JSONKeys.Signature).GetString()!);
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetString());
				Assert.NotEmpty(d.GetProperty(JSONKeys.TimeStamp).GetString()!);
				Assert.Equal(meta_rid, d.GetProperty(JSONKeys.MetaData).GetProperty(JSONKeys.Register).GetString());
				Assert.Equal(meta_bid, d.GetProperty(JSONKeys.MetaData).GetProperty("BlueprintId").GetString());
				Assert.Equal(meta_iid, d.GetProperty(JSONKeys.MetaData).GetProperty("InstanceId").GetString());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal((uint)TestData[i].Length, a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.Flags).GetString());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.Equal(Convert.ToBase64String(TestData[i]), a.Current.GetProperty(JSONKeys.Data).GetString());
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToModel() Conversion V4 Tests Raw */
		[Fact]
		public void ToModelNoTransactionV4()
		{
			TransactionModel? model = TransactionFormatter.ToModel(null);
			Assert.Null(model);
		}
		[Fact]
		public void ToModelRawOnePayloadNoRecipientsNoCompNoEncV4()
		{
			string TestHash = "03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00], null, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x00000cc000020000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(Convert.ToBase64String([0x00]), model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelRawMultiPayloadNoRecipientsNoCompNoEncV4()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray()
			];
			string[] TestHash =
			[
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"1dc01772ee0171f5f614c673e3c7fa1107a8cf727bdf5a6dadb379e93c0d1d00"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], null, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.Equal((ulong)TestData[i].Length, model.Payloads[i].PayloadSize);
				Assert.Equal("0x00000cc000020000", model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Equal(Convert.ToBase64String(TestData[i]), model.Payloads[i].Data);
			}
		}
		[Fact]
		public void ToModelRawOnePayloadOneRecipientNoCompV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet], new TransactionV4.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x00020cc000020000", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelRawOnePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x00020cc000020000", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00020cc000020000", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsCompressionV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00030cc200020000", model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsCompNoCryptV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV4.PayloadOptions { EType = EncryptionType.None });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00010c2200020000", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
		}
		[Fact]
		public void ToModelRawMultiPayloadMultiRecipientsPrIdMetaProtectedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.SetPrevTxHash(prev_txid);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV4.PayloadOptions { PType = true, EType = EncryptionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00210c2200020000", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.Equal(meta_rid, model.MetaData?.RegisterId);
			Assert.Equal(meta_bid, model.MetaData?.BlueprintId);
			Assert.Equal(meta_iid, model.MetaData?.InstanceId);
		}
		[Fact]
		public void ToModelRawCompressionLevelsV4()
		{
			string[] flags = ["0x00010cc100020000", "0x00010cc300020000", "0x00010cc200020000"];
			string hash = "831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Max });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Fast });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Balanced });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(3, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Equal(3, model.Payloads.Length);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			for (int i = 0; i < 3; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash, model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
		}
		[Fact]
		public void ToModelRawProtectedUserTypeV4()
		{
			string[] flags = ["0x00210cc200020000", "0x00210cc200024489"];
			string hash = "831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { PType = true });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { PType = true, UType = 0x4489 });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(2, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Equal(2, model.Payloads.Length);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			for (int i = 0; i < 2; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash, model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
		}
		[Fact]
		public void ToModelRawHashTypesV4()
		{
			string[] flags = [
				"0x000100c200020000",
				"0x000104c200020000",
				"0x000108c200020000",
				"0x00010cc200020000",
				"0x000110c200020000" ];
			string[] hash = [
				"2947ca600a0c8c6832dce7c0b8ea923c1f25a1c8e6953ae832eaf35e515ae1a9",
				"a2bf326def2d01829ad79ec427ad0e7d080fd2df883727257023fc254c5d736b77bb90ec82fde739143e10d69d06aac4",
				"04bd5795b018d9b8c78432ee17c66beadee26494d9a9fe97a34826c35378eba0a9db8b4c83ceb98aef8af78bb608f0ad9b1c932636e6ac0ad40a557563f436e3",
				"831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a",
				"c3354ddd5bb0b9af1a6c8cd414afa48004646ebb91d61ee72520c31bc5c26428f6ca5078f55675878b09d432c1af193ab98b94500f21f1c46cbe4a3ca52fadc7" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA384 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA512 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_512 });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(5, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Equal(5, model.Payloads.Length);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			for (int i = 0; i < 5; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
		}
		[Fact]
		public void ToModelRawEncryptionTypesV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] flags = [
				"0x00030c4200020000",
				"0x00030c6200020000",
				"0x00030c8200020000",
				"0x00030ca200020000",
				"0x00030cc200020000" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_128 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_GCM });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.CHACHA20_POLY1305 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.XCHACHA20_POLY1305 });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(5, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Equal(5, model.Payloads.Length);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			for (int i = 0; i < 5; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.NotEmpty(model.Payloads[i].Hash);
				Assert.NotNull(model.Payloads[i].Challenges);
				Assert.Single(model.Payloads[i].Challenges);
				Assert.NotNull(model.Payloads[i].Challenges[0].hex);
				Assert.True(model.Payloads[i].Challenges[0].size > 0);
				Assert.NotNull(model.Payloads[i].WalletAccess);
				Assert.Single(model.Payloads[i].WalletAccess);
				Assert.NotNull(model.Payloads[i].IV);
				Assert.NotNull(model.Payloads[i].IV.hex);
				Assert.NotEmpty(model.Payloads[i].IV.hex);
				Assert.True(model.Payloads[i].IV.size > 0);
			}
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
		}
		[Fact]
		public void ToModelRawWalletTypesV4()
		{
			string[] wallets = [sender_address1, sender_address2, sender_address3];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], wallets, new TransactionV4.PayloadOptions { TType = PayloadTypeField.Unknown });
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.Empty(model.TxId);
			Assert.Empty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Empty(model.Signature);
			Assert.Empty(model.SenderWallet);
			Assert.Equal("0x00030cc200000000", model.Payloads[0].PayloadFlags);
			Assert.NotEmpty(model.Payloads[0].Hash);
			Assert.NotNull(model.Payloads[0].Challenges);
			Assert.Equal(wallets.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].WalletAccess);
			for (int j = 0; j < wallets.Length; j++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[j].hex);
				Assert.NotEmpty(model.Payloads[0].Challenges[j].hex);
				Assert.True(model.Payloads[0].Challenges[j].size > 0);
				Assert.Equal(wallets[j], model.Payloads[0].WalletAccess[j]);
			}
			Assert.NotNull(model.Payloads[0].IV);
			Assert.NotNull(model.Payloads[0].IV.hex);
			Assert.NotEmpty(model.Payloads[0].IV.hex);
			Assert.True(model.Payloads[0].IV.size > 0);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(DateTime.MinValue, model.TimeStamp);
		}

/* ToModel() Conversion V4 Tests Signed */
		[Fact]
		public void ToModelSignedOnePayloadNoRecipientsV4()
		{
			string TestHash = "03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00], null);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Single(model.Payloads);
			Assert.Equal((ulong)1, model.Payloads[0].PayloadSize);
			Assert.Equal("0x00000cc200020000", model.Payloads[0].PayloadFlags);
			Assert.Equal(TestHash, model.Payloads[0].Hash);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Null(model.Payloads[0].Challenges);
			Assert.Null(model.Payloads[0].WalletAccess);
			Assert.Null(model.Payloads[0].IV);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			Assert.Equal(Convert.ToBase64String([0x00]), model.Payloads[0].Data);
		}
		[Fact]
		public void ToModelSignedMultiPayloadNoRecipientsV4()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				new byte[512],
				new byte[256],
				new byte[1024]
			];
			string[] TestHash =
			[
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"1dc01772ee0171f5f614c673e3c7fa1107a8cf727bdf5a6dadb379e93c0d1d00",
				"1ac7bd6f1bf4d791ffd5171c6a9804f8d59211f16dc6353a731c944a02122a5f",
				"2b69702a889248a4d6620475a105dccd5e0d4230aca8a492aaf6510e55d55b02",
				"831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a"
			];
			string[] TestFlags =
			[
				"0x00000cc000020000",
				"0x00000cc200020000",
				"0x00010cc200020000",
				"0x00000cc000020000",
				"0x00010cc100020000"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], null, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			manager.AddPayload(TestData[1], null);
			manager.AddPayload(TestData[2], null);
			manager.AddPayload(TestData[3], null, new TransactionV4.PayloadOptions { CType = CompressionType.None });
			manager.AddPayload(TestData[3], null, new TransactionV4.PayloadOptions { CType = CompressionType.Max });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.Equal(sender_address2, model.SenderWallet);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal(TestFlags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(TestHash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
		}
		[Fact]
		public void ToModelSignedOnePayloadOneRecipientProtectedV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet], new TransactionV4.PayloadOptions { PType = true });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Single(model.RecipientsWallets);
			Assert.Equal(wallet, model.RecipientsWallets.First());
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address3, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x00220cc200020000", model.Payloads[0].PayloadFlags);
			Assert.Single(model.Payloads[0].Challenges);
			Assert.Single(model.Payloads[0].WalletAccess);
			Assert.Equal(wallet, model.Payloads[0].WalletAccess[0]);
			Assert.NotNull(model.Payloads[0].IV);
		}
		[Fact]
		public void ToModelSignedOnePayloadMultiRecipientsUserTypeV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet, new TransactionV4.PayloadOptions { UType = 0x4489 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Single(model.Payloads);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.NotEqual(DateTime.MinValue, model.TimeStamp);
			Assert.True(model.Payloads[0].PayloadSize > 0);
			Assert.Equal("0x00020cc200024489", model.Payloads[0].PayloadFlags);
			Assert.Equal(wallet.Length, model.Payloads[0].WalletAccess.Length);
			Assert.Equal(wallet.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].IV);
			for (int i = 0; i < wallet.Length; i++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[i]);
				Assert.Equal(wallet[i], model.Payloads[0].WalletAccess[i]);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsHashTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				new byte[256],
				new byte[1024]
			];
			string[] TestFlags = [
				"0x000200c200020000",
				"0x000204c200020000",
				"0x000208c200020000",
				"0x00030cc200020000",
				"0x000310c200020000" ];
			HashType[] types = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet, new TransactionV4.PayloadOptions { HType = HashType.SHA256 });
			manager.AddPayload(TestData[1], wallet, new TransactionV4.PayloadOptions { HType = HashType.SHA384 });
			manager.AddPayload(TestData[2], wallet, new TransactionV4.PayloadOptions { HType = HashType.SHA512 });
			manager.AddPayload(TestData[3], wallet, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_256 });
			manager.AddPayload(TestData[4], wallet, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_512 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address2, model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal(TestFlags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
				Assert.Equal(types[i].GetHashSizeAttribute(), model.Payloads[i].Hash.Length >> 1);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsCompProtectedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			string[] TestFlags = [
				"0x00230cc100020000",
				"0x00230cc300025555",
				"0x002204c000024489" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			manager.AddPayload(TestData[0], wallet, new PayloadOptions() { CType = CompressionType.Max, PType = true });
			manager.AddPayload(TestData[1], wallet, new PayloadOptions() { CType = CompressionType.Fast, PType = true, UType = 0x5555 });
			manager.AddPayload(TestData[2], wallet, new PayloadOptions() { CType = CompressionType.None, PType = true, UType = 0x4489, HType = HashType.SHA384 });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address3, model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal(TestFlags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(wallet.Length, model.Payloads[i].WalletAccess.Length);
				Assert.NotNull(model.Payloads[i].IV);
				for (int j = 0; j < wallet.Length; j++)
				{
					Assert.NotNull(model.Payloads[i].Challenges[j]);
					Assert.Equal(wallet[j], model.Payloads[i].WalletAccess[j]);
				}
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsNoCryptNoCompV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV4.PayloadOptions { EType = EncryptionType.None, CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00000c2000020000", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
		}
		[Fact]
		public void ToModelSignedMultiPayloadMultiRecipientsPrIdMetaProtectedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData =
			[
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.SetPrevTxHash(prev_txid);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV4.PayloadOptions { PType = true, EType = EncryptionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(TestData.Count, (int)model.PayloadCount);
			Assert.Equal(wallet.Length, model.RecipientsWallets.Count());
			Assert.Equal(wallet, model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address2, model.SenderWallet);
			Assert.Equal(TestData.Count, model.Payloads.Length);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			for (int i = 0; i < TestData.Count; i++)
			{
				Assert.True(model.Payloads[i].PayloadSize > 0);
				Assert.Equal("0x00210c2200020000", model.Payloads[i].PayloadFlags);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
				Assert.Null(model.Payloads[i].Challenges);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.Equal(meta_rid, model.MetaData?.RegisterId);
			Assert.Equal(meta_bid, model.MetaData?.BlueprintId);
			Assert.Equal(meta_iid, model.MetaData?.InstanceId);
		}
		[Fact]
		public void ToModelRawCompressionHashLevelsV4()
		{
			string[] flags = [
				"0x000100c100020000",
				"0x000108c300020000",
				"0x000110c200020000" ];
			string[] hash = [
				"2947ca600a0c8c6832dce7c0b8ea923c1f25a1c8e6953ae832eaf35e515ae1a9",
				"04bd5795b018d9b8c78432ee17c66beadee26494d9a9fe97a34826c35378eba0a9db8b4c83ceb98aef8af78bb608f0ad9b1c932636e6ac0ad40a557563f436e3",
				"c3354ddd5bb0b9af1a6c8cd414afa48004646ebb91d61ee72520c31bc5c26428f6ca5078f55675878b09d432c1af193ab98b94500f21f1c46cbe4a3ca52fadc7"];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Max, HType = HashType.SHA256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Fast, HType = HashType.SHA512 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { CType = CompressionType.Balanced, HType = HashType.Blake2b_512 });
			tx.SetPrevTxHash(prev_txid);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(3, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Equal(3, model.Payloads.Length);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address3, model.SenderWallet);
			for (int i = 0; i < 3; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
		}
		[Fact]
		public void ToModelSignedProtectedUserTypeDefaultV4()
		{
			string[] flags = [
				"0x00210cc200025555",
				"0x00010cc200020000" ];
			string hash = "831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { PType = true, UType = 0x5555 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions {});
			tx.SetPrevTxHash(prev_txid);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(2, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Equal(2, model.Payloads.Length);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address1, model.SenderWallet);
			for (int i = 0; i < 2; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash, model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
		}
		[Fact]
		public void ToModelSignedHashTypesV4()
		{
			string[] flags = [
				"0x0021004200020000",
				"0x0001046200020000",
				"0x0021088200020000",
				"0x00010ca200020000",
				"0x002110c200020000" ];
			string[] hash = [
				"2947ca600a0c8c6832dce7c0b8ea923c1f25a1c8e6953ae832eaf35e515ae1a9",
				"a2bf326def2d01829ad79ec427ad0e7d080fd2df883727257023fc254c5d736b77bb90ec82fde739143e10d69d06aac4",
				"04bd5795b018d9b8c78432ee17c66beadee26494d9a9fe97a34826c35378eba0a9db8b4c83ceb98aef8af78bb608f0ad9b1c932636e6ac0ad40a557563f436e3",
				"831fab278ba2820d16f4889ca3dcc4a45939a56c5303237c07ed375c8d02af3a",
				"c3354ddd5bb0b9af1a6c8cd414afa48004646ebb91d61ee72520c31bc5c26428f6ca5078f55675878b09d432c1af193ab98b94500f21f1c46cbe4a3ca52fadc7" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA256, PType = true, EType = EncryptionType.AES_128 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA384, EType = EncryptionType.AES_256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.SHA512, PType = true, EType = EncryptionType.AES_GCM });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_256, EType = EncryptionType.CHACHA20_POLY1305 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], null, new TransactionV4.PayloadOptions { HType = HashType.Blake2b_512, PType = true, EType = EncryptionType.XCHACHA20_POLY1305 });
			tx.SetPrevTxHash(prev_txid);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(5, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Equal(5, model.Payloads.Length);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address2, model.SenderWallet);
			for (int i = 0; i < 5; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.Equal(hash[i], model.Payloads[i].Hash);
				Assert.Null(model.Payloads[i].Challenges);
				Assert.Null(model.Payloads[i].WalletAccess);
				Assert.Null(model.Payloads[i].IV);
			}
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
		}
		[Fact]
		public void ToModelSignedAllTypesV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] flags = [
				"0x00030c4200020000",
				"0x00030c6200020000",
				"0x00030c8200020000",
				"0x00030ca200020000",
				"0x002310c100011234" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_128 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_256 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.AES_GCM });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet], new TransactionV4.PayloadOptions { EType = EncryptionType.CHACHA20_POLY1305 });
			tx.GetTxPayloadManager().AddPayload(new byte[256], [wallet],
				new TransactionV4.PayloadOptions
				{
					EType = EncryptionType.XCHACHA20_POLY1305,
					PType = true,
					UType = 0x1234,
					CType = CompressionType.Max,
					HType = HashType.Blake2b_512,
					TType = PayloadTypeField.Docket
				});
			tx.SetPrevTxHash(prev_txid);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(5, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Equal(5, model.Payloads.Length);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address3, model.SenderWallet);
			for (int i = 0; i < 4; i++)
			{
				Assert.Equal(flags[i], model.Payloads[i].PayloadFlags);
				Assert.NotEmpty(model.Payloads[i].Hash);
				Assert.NotNull(model.Payloads[i].Challenges);
				Assert.Single(model.Payloads[i].Challenges);
				Assert.NotNull(model.Payloads[i].Challenges[0].hex);
				Assert.True(model.Payloads[i].Challenges[0].size > 0);
				Assert.NotNull(model.Payloads[i].WalletAccess);
				Assert.Single(model.Payloads[i].WalletAccess);
				Assert.NotNull(model.Payloads[i].IV);
				Assert.NotNull(model.Payloads[i].IV.hex);
				Assert.NotEmpty(model.Payloads[i].IV.hex);
				Assert.True(model.Payloads[i].IV.size > 0);
			}
			Assert.Equal(flags[4], model.Payloads[4].PayloadFlags);
			Assert.NotNull(model.Payloads[4].Hash);
			Assert.NotEmpty(model.Payloads[4].Hash);
			Assert.NotNull(model.Payloads[4].Challenges[0].hex);
			Assert.NotEmpty(model.Payloads[4].Challenges[0].hex);
			Assert.NotNull(model.Payloads[4].WalletAccess);
			Assert.NotEmpty(model.Payloads[4].WalletAccess);
			Assert.NotNull(model.Payloads[4].IV.hex);
			Assert.NotEmpty(model.Payloads[4].IV.hex);
			Assert.Equal(EncryptionType.XCHACHA20_POLY1305.GetIVSizeAttribute(), (int)model.Payloads[4].IV.size);
			Assert.Equal(prev_txid, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
			Assert.Equal(meta_rid, model.MetaData?.RegisterId);
			Assert.Equal(meta_bid, model.MetaData?.BlueprintId);
			Assert.Equal(meta_iid, model.MetaData?.InstanceId);
		}
		[Fact]
		public void ToModelSignedWalletTypesV4()
		{
			string[] wallets = [sender_address1, sender_address2, sender_address3];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload(new byte[256], wallets, new TransactionV4.PayloadOptions { TType = PayloadTypeField.Unknown });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Assert.Equal((UInt32)4, model.Version);
			Assert.Equal(1, (int)model.PayloadCount);
			Assert.Empty(model.RecipientsWallets);
			Assert.NotEmpty(model.TxId);
			Assert.NotEmpty(model.Id);
			Assert.Single(model.Payloads);
			Assert.NotEmpty(model.Signature);
			Assert.NotEmpty(model.SenderWallet);
			Assert.Equal(sender_address1, model.SenderWallet);
			Assert.Equal("0x00030cc200000000", model.Payloads[0].PayloadFlags);
			Assert.NotEmpty(model.Payloads[0].Hash);
			Assert.NotNull(model.Payloads[0].Challenges);
			Assert.Equal(wallets.Length, model.Payloads[0].Challenges.Length);
			Assert.NotNull(model.Payloads[0].WalletAccess);
			for (int j = 0; j < wallets.Length; j++)
			{
				Assert.NotNull(model.Payloads[0].Challenges[j].hex);
				Assert.NotEmpty(model.Payloads[0].Challenges[j].hex);
				Assert.True(model.Payloads[0].Challenges[j].size > 0);
				Assert.Equal(wallets[j], model.Payloads[0].WalletAccess[j]);
			}
			Assert.NotNull(model.Payloads[0].IV);
			Assert.NotNull(model.Payloads[0].IV.hex);
			Assert.NotEmpty(model.Payloads[0].IV.hex);
			Assert.True(model.Payloads[0].IV.size > 0);
			Assert.Equal(TxDefs.NullTxId, model.PrevTxId);
			Assert.True(model.TimeStamp != DateTime.MinValue);
		}

/* ToJSON() Conversion V4 Tests Raw */
		[Fact]
		public void ToJSONNoTransactionV4()
		{
			string json = TransactionFormatter.ToJSON(null);
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONEmptyTransactionV4()
		{
			string json = TransactionFormatter.ToJSON(new Transaction());
			Assert.Equal("{}", json);
		}
		[Fact]
		public void ToJSONRawOnePayloadNoRecipientsV4()
		{
			string TestHash = "03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00000cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadNoRecipientsV4()
		{
			string[] TestHash = [
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"1dc01772ee0171f5f614c673e3c7fa1107a8cf727bdf5a6dadb379e93c0d1d00" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00000cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadOneRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.True(b.Current.GetProperty("Challenge").GetProperty(JSONKeys.Size).GetUInt64() > 0);
				Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
				Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawOnePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadEncryptionTypesNoRecipientsV4()
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
			EncryptionType[] types = [EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.XCHACHA20_POLY1305];
			string[] flags = [
				"0x00020c62",
				"0x00020c82",
				"0x00020cc2"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions { EType = types[i] });
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(types[i].ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsNoEncryptionHashesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] flags = [
				"0x000000c2",
				"0x000004c2",
				"0x000008c2",
				"0x00000cc2",
				"0x000010c2"
			];
			string[] hashes = [
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"bec021b4f368e3069134e012c2b4307083d3a9bdd206e24e5f0d86e13d6636655933ec2b413465966817a9c208a11717",
				"b8244d028981d693af7b456af8efa4cad63d282e19ff14942c246e50d9351d22704a802a71c3580b6370de4ceb293c324a8423342557d4e5c38438f0e36910ee",
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"2fa3f686df876995167e7c2e5d74c4c7b6e48f8068fe0e44208344d480f7904c36963e44115fe3eb2a3ac8694c28bcb4f5a0f3276f2e79487d8219057a506e4b"
			];
			HashType[] hashtypes = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 5; i++)
				manager.AddPayload([0x00], null, new PayloadOptions() { HType = hashtypes[i] });
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)5, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(5, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < 5; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(hashtypes[i].ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(hashes[i], a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadRecipientsCompressedV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] flags = [
				"0x00030cc1",
				"0x00030cc2",
				"0x00030cc3"
			];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Max, CompressionType.Balanced, CompressionType.Fast];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], [wallet], new PayloadOptions() { CType = types[i] });
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					b.MoveNext();
					Assert.Equal(wallet, b.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsMetaUserProtectedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] ud = [
				"0x5555",
				"0x4489",
				"0x1234" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions { PType = true, UType = Convert.ToUInt16(ud[i], 16) });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(meta_rid, d.GetProperty(JSONKeys.MetaData).GetProperty(JSONKeys.Register).GetString());
				Assert.Equal(meta_bid, d.GetProperty(JSONKeys.MetaData).GetProperty("BlueprintId").GetString());
				Assert.Equal(meta_iid, d.GetProperty(JSONKeys.MetaData).GetProperty("InstanceId").GetString());
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal(ud[i], a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00230cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONRawMultiPayloadMultiRecipientsTxTypeWalletTypeV4()
		{
			string[] wallets = [sender_address1, sender_address2, sender_address3];
			string[] networks = ["0x12", "0x18", "0x17"];
			PayloadTypeField[] tf = [PayloadTypeField.Genesys, PayloadTypeField.Unknown, PayloadTypeField.Blueprint];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallets);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallets, new PayloadOptions { PType = true, TType = tf[i] });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallets.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallets.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallets[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal(networks[i], a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.False(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.False(d.TryGetProperty(JSONKeys.Sender, out _));
				Assert.False(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(tf[i].ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00230cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallets.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallets.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallets[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal(networks[j], b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToJSON() Conversion V4 Tests Signed */
		[Fact]
		public void ToJSONSignedOnePayloadNoRecipientsV4()
		{
			string TestHash = "03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00000cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.Equal(TestHash, a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
				Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadNoRecipientsV4()
		{
			string[] TestHash = [
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"1dc01772ee0171f5f614c673e3c7fa1107a8cf727bdf5a6dadb379e93c0d1d00" ];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address2, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x18", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00000cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(TestHash[i], a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.False(a.Current.TryGetProperty(JSONKeys.Challenges, out _));
					Assert.False(a.Current.TryGetProperty(JSONKeys.IV, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedOnePayloadOneRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address3, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x17", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				b.MoveNext();
				Assert.Equal(wallet, b.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.True(b.Current.GetProperty("Challenge").GetProperty(JSONKeys.Size).GetUInt64() > 0);
				Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
				Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToSignedRawOnePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)1, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(1, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				a.MoveNext();
				Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
				Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
				Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
				Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
				Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
				Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
				Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
				Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
				JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					b.MoveNext();
					Assert.Equal(wallet[i], b.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
				}
				Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address2, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x18", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00020cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(EncryptionType.XCHACHA20_POLY1305.ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadEncryptionTypesNoRecipientsV4()
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
			EncryptionType[] types = [EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.XCHACHA20_POLY1305];
			string[] flags = [
				"0x00020c62",
				"0x00020c82",
				"0x00020cc2"
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions { EType = types[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(0, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address3, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x17", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 1);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(types[i].ToString(), a.Current.GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadRecipientsNoEncryptionHashesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] flags = [
				"0x000000c2",
				"0x000004c2",
				"0x000008c2",
				"0x00000cc2",
				"0x000010c2"
			];
			string[] hashes = [
				"6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
				"bec021b4f368e3069134e012c2b4307083d3a9bdd206e24e5f0d86e13d6636655933ec2b413465966817a9c208a11717",
				"b8244d028981d693af7b456af8efa4cad63d282e19ff14942c246e50d9351d22704a802a71c3580b6370de4ceb293c324a8423342557d4e5c38438f0e36910ee",
				"03170a2e7597b7b7e3d84c05391d139a62b157e78786d8c082f29dcf4c111314",
				"2fa3f686df876995167e7c2e5d74c4c7b6e48f8068fe0e44208344d480f7904c36963e44115fe3eb2a3ac8694c28bcb4f5a0f3276f2e79487d8219057a506e4b"
			];
			HashType[] hashtypes = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < 5; i++)
				manager.AddPayload([0x00], null, new PayloadOptions() { HType = hashtypes[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)5, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(5, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < 5; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(hashtypes[i].ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(hashes[i], a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString());
					Assert.Equal(0, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadRecipientsCompressedV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			string[] flags = [
				"0x00030cc1",
				"0x00030cc2",
				"0x00030cc3"
			];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Max, CompressionType.Balanced, CompressionType.Fast];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], [wallet], new PayloadOptions() { CType = types[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(1, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				Assert.Equal(TxDefs.NullTxId, d.GetProperty(JSONKeys.PrevId).GetString());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				a.MoveNext();
				Assert.Equal(wallet, a.Current.GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address2, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x18", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.False(d.TryGetProperty(JSONKeys.MetaData, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal(flags[i], a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.False(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(1, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					b.MoveNext();
					Assert.Equal(wallet, b.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
					Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsMetaUserProtectedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			string[] ud = [
				"0x5555",
				"0x4489",
				"0x1234" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions { PType = true, UType = Convert.ToUInt16(ud[i], 16) });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallet.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallet.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallet[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal("0x12", a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address3, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x17", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(meta_rid, d.GetProperty(JSONKeys.MetaData).GetProperty(JSONKeys.Register).GetString());
				Assert.Equal(meta_bid, d.GetProperty(JSONKeys.MetaData).GetProperty("BlueprintId").GetString());
				Assert.Equal(meta_iid, d.GetProperty(JSONKeys.MetaData).GetProperty("InstanceId").GetString());
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(PayloadTypeField.Transaction.ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal(ud[i], a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00230cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallet.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallet.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallet[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal("0x12", b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}
		[Fact]
		public void ToJSONSignedMultiPayloadMultiRecipientsTxTypeWalletTypeV4()
		{
			string[] wallets = [sender_address1, sender_address2, sender_address3];
			string[] networks = ["0x12", "0x18", "0x17"];
			PayloadTypeField[] tf = [PayloadTypeField.Genesys, PayloadTypeField.Unknown, PayloadTypeField.Blueprint];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallets);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallets, new PayloadOptions { PType = true, TType = tf[i] });
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(_, Transaction? transport) = tx.GetTxTransport();
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(TransportIdentifier.Transaction.ToString(), d.GetProperty(JSONKeys.Type).GetString());
				Assert.Equal((uint)4, d.GetProperty(JSONKeys.Version).GetUInt32());
				Assert.Equal((uint)TestData.Count, d.GetProperty(JSONKeys.PayloadCount).GetUInt32());
				Assert.Equal(wallets.Length, d.GetProperty(JSONKeys.Recipients).GetArrayLength());
				JsonElement.ArrayEnumerator a = d.GetProperty(JSONKeys.Recipients).EnumerateArray();
				Assert.Equal(prev_txid, d.GetProperty(JSONKeys.PrevId).GetString());
				for (int i = 0; i < wallets.Length; i++)
				{
					a.MoveNext();
					Assert.Equal(wallets[i], a.Current.GetProperty(JSONKeys.Wallet).GetString());
					Assert.Equal(networks[i], a.Current.GetProperty(JSONKeys.Network).GetString());
				}
				Assert.True(d.TryGetProperty(JSONKeys.TxId, out _));
				Assert.True(d.TryGetProperty(JSONKeys.Signature, out _));
				Assert.Equal(sender_address1, d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Wallet).GetString());
				Assert.Equal("0x12", d.GetProperty(JSONKeys.Sender).GetProperty(JSONKeys.Network).GetString());
				Assert.True(d.TryGetProperty(JSONKeys.TimeStamp, out _));
				Assert.Equal(TestData.Count, d.GetProperty(JSONKeys.Payloads).GetArrayLength());
				a = d.GetProperty(JSONKeys.Payloads).EnumerateArray();
				for (int i = 0; i < TestData.Count; i++)
				{
					a.MoveNext();
					Assert.Equal(tf[i].ToString(), a.Current.GetProperty(JSONKeys.TypeField).GetString());
					Assert.Equal("0x0000", a.Current.GetProperty(JSONKeys.UserField).GetString());
					Assert.Equal("0x00230cc2", a.Current.GetProperty(JSONKeys.Options).GetString());
					Assert.True(a.Current.GetProperty(JSONKeys.PayloadSize).GetUInt32() > 0);
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Protected).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Compressed).GetBoolean());
					Assert.True(a.Current.GetProperty(JSONKeys.Flags).GetProperty(JSONKeys.Encrypted).GetBoolean());
					Assert.Equal(a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Hex).GetString()?.Length, HashType.Blake2b_256.GetHashSizeAttribute() << 1);
					Assert.Equal(HashType.Blake2b_256.ToString(), a.Current.GetProperty(JSONKeys.Hash).GetProperty(JSONKeys.Type).GetString());
					Assert.Equal(wallets.Length, a.Current.GetProperty(JSONKeys.Access).GetArrayLength());
					JsonElement.ArrayEnumerator b = a.Current.GetProperty(JSONKeys.Access).EnumerateArray();
					for (int j = 0; j < wallets.Length; j++)
					{
						b.MoveNext();
						Assert.Equal(wallets[j], b.Current.GetProperty(JSONKeys.Wallet).GetString());
						Assert.Equal(networks[j], b.Current.GetProperty(JSONKeys.Network).GetString());
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Hex, out _));
						Assert.True(b.Current.GetProperty("Challenge").TryGetProperty(JSONKeys.Size, out _));
					}
					Assert.NotEmpty(a.Current.GetProperty(JSONKeys.Data).GetString()!);
				}
			}
			catch (Exception) { Assert.Fail(string.Empty); }
		}

/* ToTransaction() Conversion From JSON V1 Tests Raw */
		[Fact]
		public void ToTransactionNoJSONV1()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((string?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyJSONV1()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(string.Empty);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyObjJSONV1()
		{
			Transaction? tx = TransactionFormatter.ToTransaction("{}");
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadV1()
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
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadSingleRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadsMultiRecipientsV1()
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
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}

/* ToTransaction() Conversion From JSON V1 Tests Signed */
		[Fact]
		public void ToTransactionSignedSinglePayloadV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadSingleRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}

/* ToTransaction() Conversion From JSON V2 Tests Raw */
		[Fact]
		public void ToTransactionNoJSONV2()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((string?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyJSONV2()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(string.Empty);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyObjJSONV2()
		{
			Transaction? tx = TransactionFormatter.ToTransaction("{}");
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadSingleRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadsMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedModifiedV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] rmf = [true, true, true];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}

/* ToTransaction() Conversion From JSON V2 Tests Signed */
		[Fact]
		public void ToTransactionSignedSinglePayloadV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadSingleRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsNoEncryptionV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedModifiedV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] rmf = [true, true, true];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}

/* ToTransaction() Conversion From JSON V3 Tests Raw */
		[Fact]
		public void ToTransactionNoJSONV3()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((string?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyJSONV3()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(string.Empty);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyObjJSONV3()
		{
			Transaction? tx = TransactionFormatter.ToTransaction("{}");
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadV3()
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
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadSingleRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadsMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedModifiedV3()
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
			bool[] rmf = [true, true, true];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedNoCompressionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedPrIdMetaV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}

/* ToTransaction() Conversion From JSON V3 Tests Signed */
		[Fact]
		public void ToTransactionSignedSinglePayloadV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadV3()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadSingleRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsNoEncryptionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedModifiedV3()
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
			bool[] rmf = [true, true, true];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedNoCompressionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedPrIdMetaV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}

/* ToTransaction() Conversion From JSON V4 Tests Raw */
		[Fact]
		public void ToTransactionNoJSONV4()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((string?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyJSONV4()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(string.Empty);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionEmptyObjJSONV4()
		{
			Transaction? tx = TransactionFormatter.ToTransaction("{}");
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadV4()
		{
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadSingleRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawSinglePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadsMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedNoCompressionV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptedPrIdMetaV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionRawMultiPayloadEncryptionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			EncryptionType[] types = [EncryptionType.AES_128, EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.CHACHA20_POLY1305, EncryptionType.XCHACHA20_POLY1305];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { EType = types[i] });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void ToTransactionRawMultiPayloadHashUserTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x01, 0x02, 0x03],
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			HashType[] types = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			UInt16[] ut = [0x1111, 0x5555, 0x4489, 0xcccc, 0xaaaa];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { HType = types[i], UType = ut[i] });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadHashType());
				Assert.Equal(ut[i], info[i].GetPayloadUserField());
			}
		}
		[Fact]
		public void ToTransactionRawMultiPayloadCompressionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Fast, CompressionType.Balanced, CompressionType.Max];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { CType = types[i], PType = true });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.True(info[i].GetPayloadProtected());
				Assert.True(info[i].GetPayloadCompressed());
				Assert.Equal(types[i], info[i].GetPayloadCompressionType());
			}
		}
		[Fact]
		public void ToTransactionRawMultiPayloadTTypeWalletsV4()
		{
			string[] wallet = [sender_address1, sender_address2, sender_address3];
			PayloadTypeField[] types = [PayloadTypeField.Unknown, PayloadTypeField.Docket, PayloadTypeField.Rejection];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { TType = types[i] });
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.True(info[i].GetPayloadCompressed());
				Assert.Equal(types[i], info[i].GetPayloadTypeField());
			}
		}

/* ToTransaction() Conversion From JSON V4 Tests Signed */
		[Fact]
		public void ToTransactionSignedSinglePayloadV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadV4()
		{
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadSingleRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedSinglePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsMultiRecipientsNoEncryptionV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedNoCompressionV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadsEncrypedPrIdMetaV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadEncryptionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			EncryptionType[] types = [EncryptionType.AES_128, EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.CHACHA20_POLY1305, EncryptionType.XCHACHA20_POLY1305];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { EType = types[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadEncryptionType());
			}
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadHashUserTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x01, 0x02, 0x03],
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			HashType[] types = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			UInt16[] ut = [0x1111, 0x5555, 0x4489, 0xcccc, 0xaaaa];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { HType = types[i], UType = ut[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadHashType());
				Assert.Equal(ut[i], info[i].GetPayloadUserField());
			}
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadCompressionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Fast, CompressionType.Balanced, CompressionType.Max];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { CType = types[i], PType = true });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.True(info[i].GetPayloadProtected());
				Assert.True(info[i].GetPayloadCompressed());
				Assert.Equal(types[i], info[i].GetPayloadCompressionType());
			}
		}
		[Fact]
		public void ToTransactionSignedMultiPayloadTTypeWalletsV4()
		{
			string[] wallet = [sender_address1, sender_address2, sender_address3];
			PayloadTypeField[] types = [PayloadTypeField.Unknown, PayloadTypeField.Docket, PayloadTypeField.Rejection];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { TType = types[i] });
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			string json = TransactionFormatter.ToJSON(transport);
			Assert.NotEqual("{}", json);
			Transaction? transaction = TransactionFormatter.ToTransaction(json);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.True(info[i].GetPayloadCompressed());
				Assert.Equal(types[i], info[i].GetPayloadTypeField());
			}
		}

/* ToTransaction() Conversion From Model V1 Tests Raw */
		[Fact]
		public void ToTransactionModelNoJSONV1()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((TransactionModel?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelEmptyJSONV1()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(new TransactionModel());
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadV1()
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
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadSingleRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadsMultiRecipientsV1()
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
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}

/* ToTransaction() Conversion From Model V1 Tests Signed */
		[Fact]
		public void ToTransactionModelSignedSinglePayloadV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadSingleRecipientV1()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadMultiRecipientsV1()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsV1()
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
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}

/* ToTransaction() Conversion From Model V2 Tests Raw */
		[Fact]
		public void ToTransactionModelNoJSONV2()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((TransactionModel?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelEmptyJSONV2()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(new TransactionModel());
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadSingleRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadsMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedModifiedV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] rm = [true, true, true];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rm, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}

/* ToTransaction() Conversion From Model V2 Tests Signed */
		[Fact]
		public void ToTransactionModelSignedSinglePayloadV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadV2()
		{
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadSingleRecipientV2()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadMultiRecipientsV2()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsNoEncryptionV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedV2()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedModifiedV2()
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
			bool[] rmf = [true, true, true];

			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}

/* ToTransaction() Conversion From Model V3 Tests Raw */
		[Fact]
		public void ToTransactionModelNoJSONV3()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((TransactionModel?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelEmptyJSONV3()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(new TransactionModel());
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadV3()
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
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadSingleRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadsMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedModifiedV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] rmf = [true, true, true];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedNoCompV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedPrIdV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}

/* ToTransaction() Conversion From Model V3 Tests Signed */
		[Fact]
		public void ToTransactionModelSignedSinglePayloadV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadV3()
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
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadSingleRecipientV3()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadMultiRecipientsV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsNoEncryptionV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedModifiedV3()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			bool[] rmf = [true, true, true];
			List<byte[]> TestData =
			[
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55]
			];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
			{
				UInt32 id = manager.AddPayload(TestData[i], wallet).id;
				Assert.Equal(rmf, manager.RemovePayloadWallets(id, wallet).wallets);
			}
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			IPayloadInfo[]? info = fmt?.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Empty(info[i].GetPayloadWallets());
			}
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedNoCompV3()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsEncrypedTxId()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			Assert.NotNull(transport.Id);
			Assert.NotEmpty(transport.Id);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Equal(transaction.Id, transport.Id);
		}

/* ToTransaction() Conversion From Model V4 Tests Raw */
		[Fact]
		public void ToTransactionModelNoJSONV4()
		{
			Transaction? tx = TransactionFormatter.ToTransaction((TransactionModel?)null);
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelEmptyJSONV4()
		{
			Transaction? tx = TransactionFormatter.ToTransaction(new TransactionModel());
			Assert.Null(tx);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadV4()
		{
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadSingleRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawSinglePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadsMultiRecipientsV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadRecipientsV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedNoCompV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptedPrIdV4()
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
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadHashUserTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x01, 0x02, 0x03],
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			HashType[] types = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			UInt16[] ut = [0x1111, 0x5555, 0x4489, 0xcccc, 0xaaaa];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { HType = types[i], UType = ut[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadHashType());
				Assert.Equal(ut[i], info[i].GetPayloadUserField());
			}
			Assert.Equal(Status.TX_NOT_SIGNED, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadCompressionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Fast, CompressionType.Balanced, CompressionType.Max];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { CType = types[i], PType = true }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadCompressionType());
				Assert.True(info[i].GetPayloadProtected());
			}
			Assert.Equal(Status.TX_NOT_SIGNED, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadEncryptionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			EncryptionType[] types = [EncryptionType.AES_128, EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.CHACHA20_POLY1305, EncryptionType.XCHACHA20_POLY1305];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { EType = types[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadEncryptionType());
			}
			Assert.Equal(Status.TX_NOT_SIGNED, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelRawMultiPayloadTTypeWalletsV4()
		{
			string[] wallet = [sender_address1, sender_address2, sender_address3, sender_address2];
			PayloadTypeField[] types = [PayloadTypeField.Blueprint, PayloadTypeField.Action, PayloadTypeField.Transaction];
			List<byte[]> TestData = [
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { TType = types[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.Null(transaction.Id);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.Equal(types[i], info[i].GetPayloadTypeField());
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(3, (int)info[i].GetPayloadWalletsCount());
			}
			Assert.Equal(Status.TX_NOT_SIGNED, fmt.VerifyTx());
		}

/* ToTransaction() Conversion From Model V4 Tests Signed */
		[Fact]
		public void ToTransactionModelSignedSinglePayloadV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadV4()
		{
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadSingleRecipientV4()
		{
			string wallet = "ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz";
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients([wallet]);
			tx.GetTxPayloadManager().AddPayload([0x00], [wallet]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedSinglePayloadMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			tx.GetTxPayloadManager().AddPayload([0x00], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadsMultiRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadRecipientsV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.SetTxRecipients(wallet);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadEncryptedV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadEncryptedNoCompV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new TransactionV3.PayloadOptions { CType = CompressionType.None });
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadEncryptedPrIdV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SetPrevTxHash(prev_txid));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.NotNull(transaction.RegisterId);
			Assert.Equal(meta_rid, transaction.RegisterId);
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadHashUserTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x01, 0x02, 0x03],
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			HashType[] types = [HashType.SHA256, HashType.SHA384, HashType.SHA512, HashType.Blake2b_256, HashType.Blake2b_512];
			UInt16[] ut = [0x1111, 0x5555, 0x4489, 0xcccc, 0xaaaa];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { HType = types[i], UType = ut[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(transaction.Id, fmt.GetTxHash().hash);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			for (int i = 0; i < info?.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadHashType());
				Assert.Equal(ut[i], info[i].GetPayloadUserField());
			}
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadCompressionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				new byte[256],
				Encoding.ASCII.GetBytes("Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World" +
					"Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World"),
				[
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55,
					0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55 ] ];
			CompressionType[] types = [CompressionType.Fast, CompressionType.Balanced, CompressionType.Max];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { CType = types[i], PType = true }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			Assert.Equal(transaction.Id, fmt.GetTxHash().hash);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadCompressionType());
				Assert.True(info[i].GetPayloadProtected());
			}
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadEncryptionTypesV4()
		{
			string[] wallet = [
				"ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz",
				"ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c",
				"ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t" ];
			List<byte[]> TestData = [
				[0x00],
				"Hello World"u8.ToArray(),
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			EncryptionType[] types = [EncryptionType.AES_128, EncryptionType.AES_256, EncryptionType.AES_GCM, EncryptionType.CHACHA20_POLY1305, EncryptionType.XCHACHA20_POLY1305];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { EType = types[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(types[i], info[i].GetPayloadEncryptionType());
			}
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
		}
		[Fact]
		public void ToTransactionModelSignedMultiPayloadTTypeWalletsV4()
		{
			string[] wallet = [sender_address1, sender_address2, sender_address3, sender_address2];
			PayloadTypeField[] types = [PayloadTypeField.Blueprint, PayloadTypeField.Action, PayloadTypeField.Transaction];
			List<byte[]> TestData = [
				[0xff, 0x55, 0xff, 0x55, 0x44, 0x89, 0x44, 0x89, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55, 0x55, 0x55],
				[0x01, 0x02, 0x03],
				[0xaa, 0xaa, 0xaa, 0xaa, 0x44, 0x89, 0x44, 0x89, 0x55, 0x55, 0x55, 0x55] ];
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			IPayloadManager manager = tx.GetTxPayloadManager();
			for (int i = 0; i < TestData.Count; i++)
				manager.AddPayload(TestData[i], wallet, new PayloadOptions() { TType = types[i] }); ;
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			Assert.Equal(Status.STATUS_OK, tx.VerifyTx());
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			ReadOnlySpan<byte> s1 = transport.Data;
			TransactionModel? model = TransactionFormatter.ToModel(transport);
			Assert.NotNull(model);
			Transaction? transaction = TransactionFormatter.ToTransaction(model);
			Assert.NotNull(transaction);
			Assert.True(s1.SequenceEqual(transaction.Data));
			Assert.NotNull(transaction.Id);
			Assert.NotEmpty(transaction.Id);
			Assert.Equal(meta_rid, transaction.RegisterId);
			(Status status, ITxFormat? fmt) = TransactionBuilder.Build(transaction);
			Assert.Equal(Status.STATUS_OK, status);
			Assert.NotNull(fmt);
			IPayloadInfo[]? info = fmt.GetTxPayloadManager().GetPayloadsInfo().info;
			Assert.NotNull(info);
			for (int i = 0; i < info.Length; i++)
			{
				Assert.Equal(types[i], info[i].GetPayloadTypeField());
				Assert.True(info[i].GetPayloadEncrypted());
				Assert.Equal(3, (int)info[i].GetPayloadWalletsCount());
			}
			Assert.Equal(Status.STATUS_OK, fmt.VerifyTx());
		}

/* ToJSONLayout() Tests */
		[Fact]
		public void ToJSONLayoutNoLayout()
		{
			string tx = TransactionFormatter.ToJSONLayout(null);
			Assert.Equal("{}", tx);
		}
		[Fact]
		public void ToJSONLayoutEmptyTx()
		{
			string tx = TransactionFormatter.ToJSONLayout(new Transaction());
			Assert.Equal("{}", tx);
		}
		[Fact]
		public void ToJSONLayoutRawTxV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutRawTxV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutRawTxV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutRawTxV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutRawTxMetaV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutRawTxMetaV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.TxId).ValueKind);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxV1()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_1);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxV2()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_2);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedEd25519TxV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedNISTTxV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedRSA4096TxV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				Assert.Equal(JsonValueKind.Null, d.GetProperty(JSONKeys.Register).ValueKind);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxMetaV3()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_3);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxMetaEd25519V4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey1));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxMetaNISTV4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey2));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
		[Fact]
		public void ToJSONLayoutSignedTxMetaRSA4096V4()
		{
			ITxFormat tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
			tx.GetTxPayloadManager().AddPayload([0x00]);
			Assert.Equal(Status.STATUS_OK, tx.SetTxMetaData(meta_data));
			Assert.Equal(Status.STATUS_OK, tx.SignTx(sender_pkey3));
			(Status result, Transaction? transport) = tx.GetTxTransport();
			Assert.Equal(Status.STATUS_OK, result);
			Assert.NotNull(transport);
			string json = TransactionFormatter.ToJSONLayout(transport);
			Assert.NotEqual("{}", json);
			try
			{
				JsonElement d = JsonDocument.Parse(json).RootElement;
				string? id = d.GetProperty(JSONKeys.TxId).GetString();
				Assert.NotNull(id);
				Assert.NotEmpty(id);
				string? reg = d.GetProperty(JSONKeys.Register).GetString();
				Assert.NotNull(reg);
				Assert.Equal(meta_rid, reg);
				string? data = d.GetProperty(JSONKeys.Data).GetString();
				Assert.NotNull(data);
				Assert.NotEmpty(data);
			}
			catch (Exception) { Assert.Fail(""); }
		}
	}
}
