using FakeItEasy;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletService.V1.Services;
using Xunit;

namespace WalletUnitTests.V1.Services
{
	public class WalletFactoryTest
	{
		private readonly IRegisterServiceClient _mockServiceClient;
		private readonly IWalletFactory _underTest;

		public WalletFactoryTest()
		{
			_mockServiceClient = A.Fake<IRegisterServiceClient>();
			_underTest = new WalletFactory(_mockServiceClient);
		}

		public class BuildWallet : WalletFactoryTest
		{
			[Fact]
			public async void Should_Have_PrivateKeySet()
			{
				var (result, _) = await _underTest.BuildWallet(null, null, null, null, null, true);
				Assert.NotNull(result.PrivateKey);
			}
			[Fact]
			public async void Should_Have_OwnerSet()
			{
				var expected = Guid.NewGuid().ToString();
				var (result, _) = await _underTest.BuildWallet(null, null, null, null, expected, true);
				Assert.Equal(expected, result.Owner);
			}
			[Fact]
			public async void Should_Have_TenantSet()
			{
				var expected = Guid.NewGuid().ToString();
				var (result, _) = await _underTest.BuildWallet(null, null, null, expected, null, true);
				Assert.Equal(expected, result.Tenant);
			}
			[Fact]
			public async void Should_Have_NameSet()
			{
				var expected = "WalletName";
				var (result, _) = await _underTest.BuildWallet(expected, null, null, null, null, true);
				Assert.Equal(expected, result.Name);
			}
			[Fact]
			public async void Should_Have_EmptyTransactionsList_WhenCreatingWallet()
			{
				var (result, _) = await _underTest.BuildWallet(null, null, null, null, null, true);
				Assert.Empty(result.Transactions);
			}
			[Fact]
			public async Task Should_Set_WalletAddress()
			{
				var (result, _) = await _underTest.BuildWallet(null, null, null, null, null, true);
				Assert.NotEmpty(result.Addresses);
				Assert.NotNull(result.Addresses.First().WalletId);
				Assert.NotNull(result.Addresses.First().Address);
			}
			[Fact]
			public async Task Should_Set_Delegates()
			{
				var tenant = Guid.NewGuid().ToString();
				var sub = Guid.NewGuid().ToString();
				_ = new WalletAddress
				{
					Address = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh",
					WalletId = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh"
				};
				var (result, _) = await _underTest.BuildWallet(null, null, null, tenant, sub, true);
				Assert.NotEmpty(result.Delegates);
				Assert.Equal(AccessTypes.owner, result.Delegates.First().AccessType);
				Assert.Equal(tenant, result.Delegates.First().Tenant);
				Assert.Equal(sub, result.Delegates.First().Subject);
			}

			[Fact]
			public async Task Should_Return_Mnemonic()
			{
				var (_, mnemonic) = await _underTest.BuildWallet(null, null, null, null, null, true);
				Assert.NotNull(mnemonic);
			}
			[Fact]
			public async Task Should_GetAllTransactionsByRecipientAddress_WhenIsCreateIsFalse()
			{
				var registerId = Guid.NewGuid().ToString();
				var (result, _) = await _underTest.BuildWallet(null, null, registerId, null, null, false);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, result.Address)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_GetAllTransactionsBySenderAddress_WhenIsCreateIsFalse()
			{
				var registerId = Guid.NewGuid().ToString();
				var (result, _) = await _underTest.BuildWallet(null, null, registerId, null, null, false);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, result.Address)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_AddSentTransactions_ToWallet()
			{
				var mnemonic = "banner oak circle bonus tennis witness way there car chair own neutral cherry pool clown crunch degree vibrant layer filter title point onion away force";
				var address = "ws1jcu683p6ca5zdvpcm2hclaplwdstjewhm86as6srdyzph7vtcetgq2g3tvw";
				var registerId = Guid.NewGuid().ToString();
				var expectedTxId = Guid.NewGuid().ToString();
				var expected = new List<TransactionModel> { new() { TxId = expectedTxId, SenderWallet = address, RecipientsWallets = new List<string> { Guid.NewGuid().ToString() } } };
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, A<string>.Ignored)).Returns(expected);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);
				Assert.Single(result.Transactions);
				Assert.Single(result.Transactions);
				var tx = result.Transactions.First();
				Assert.Equal(tx.TransactionId, expectedTxId);
				Assert.Equal(tx.Sender, expected[0].SenderWallet);
				Assert.True(tx.isSendingWallet);
				Assert.Equal(tx.PreviousId, expected[0].PrevTxId);
				Assert.Equal(tx.Timestamp, expected[0].TimeStamp);
				Assert.Equal(tx.MetaData, expected[0].MetaData);
				Assert.Equal(tx.WalletId, address);
				Assert.Equal(tx.TransactionId, expectedTxId);
				Assert.True(tx.isSpent);
				Assert.True(tx.isConfirmed);
			}
			[Fact]
			public async Task Should_AddRecievedTransactions_WhereTransactionIds_AreNotInSentTransactions()
			{
				var mnemonic = "banana poem area slice execute snap vanish gorilla spin turn uphold notable cinnamon umbrella never domain industry ordinary engage exhibit artwork twice worry family credit";
				var address = "ws1j693yppfe6v38ek4yd7tesqr3gt49aspp82ja37dfaj2r2t0j6dxq5qlqem";
				var registerId = Guid.NewGuid().ToString();
				var expectedTxId = Guid.NewGuid().ToString();
				var expected = new List<TransactionModel> { new() { TxId = expectedTxId, SenderWallet = Guid.NewGuid().ToString(), RecipientsWallets = new List<string> { address } } };
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, A<string>.Ignored)).Returns(expected);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);
				Assert.Single(result.Transactions);
				var tx = result.Transactions.First();
				Assert.Equal(tx.TransactionId, expectedTxId);
				Assert.Equal(tx.Sender, expected[0].SenderWallet);
				Assert.False(tx.isSendingWallet);
				Assert.Equal(tx.PreviousId, expected[0].PrevTxId);
				Assert.Equal(tx.Timestamp, expected[0].TimeStamp);
				Assert.Equal(tx.MetaData, expected[0].MetaData);
				Assert.Equal(tx.WalletId, address);
				Assert.Equal(tx.TransactionId, expectedTxId);
				Assert.False(tx.isSpent);
				Assert.True(tx.isConfirmed);
			}
			[Fact]
			public async Task Should_SetSentTransactionIsSpentToFalse_WhenWalletHasSentTransactionToSelf()
			{
				var mnemonic = "banana web shop tobacco place suit ketchup certain notice recipe green gaze trim fun manage response begin season jump dash raccoon letter sudden ostrich mention";
				var address = "ws1jcfmtfp78zghnqn4duww34s4adxshtd4kj30a9dhl0ry973pz899sqk0j8a";
				var registerId = Guid.NewGuid().ToString();
				var expectedTxId = Guid.NewGuid().ToString();
				var prevTxId = Guid.NewGuid().ToString();
				var latestTx = new TransactionModel { TxId = expectedTxId, SenderWallet = address, PrevTxId = prevTxId };
				var recieved = new List<TransactionModel> {
					new() { TxId = prevTxId, SenderWallet = address, RecipientsWallets = new List<string> { Guid.NewGuid().ToString() } },
					latestTx
				};
				var sent = new List<TransactionModel> { latestTx };
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, A<string>.Ignored)).Returns(recieved);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, A<string>.Ignored)).Returns(sent);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);
				Assert.Equal(2, result.Transactions.Count);
				var sentWalletTx = result.Transactions.ToList().Find(tx => tx.TransactionId == latestTx.Id);
				var recievedWalletTx = result.Transactions.ToList().Find(tx => tx.TransactionId == recieved[0].TxId);
				Assert.NotNull(sentWalletTx);
				Assert.NotNull(recievedWalletTx);
				Assert.False(sentWalletTx.isSpent);
				Assert.True(recievedWalletTx.isSpent);
			}
			[Fact]
			public async Task Should_SetSentTransactionIsSpentToTrue_WhenWalletRespondsToATransaction_ThatWasSentToSelf()
			{
				// Given a single wallet
				var mnemonic = "banana matter rent night artefact network tone head famous narrow strategy region margin leave excess apple purchase citizen merit thumb priority behind flag tank canal";
				var address = "ws1jne2skjsf4upqus50nxc6elyhps34zteyrck29g92cnde4yskpzls0zl5nv";
				var registerId = Guid.NewGuid().ToString();

				// Given a wallet sends a transaction to it's self
				var prevTxId = Guid.NewGuid().ToString();
				var prevTx = new TransactionModel { TxId = prevTxId, SenderWallet = address, RecipientsWallets = new List<string> { Guid.NewGuid().ToString() } };
				var sentTransactions = new List<TransactionModel> { prevTx };
				var recievedTransactions = new List<TransactionModel> { prevTx };

				// And the wallet sends a transaction in response to the previous transaction
				var expectedTxId = Guid.NewGuid().ToString();
				var latestTx = new TransactionModel { TxId = expectedTxId, SenderWallet = address, PrevTxId = prevTxId };
				sentTransactions.Add(latestTx);

				// When we rebuild the wallet
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, A<string>.Ignored)).Returns(recievedTransactions);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, A<string>.Ignored)).Returns(sentTransactions);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);

				// Then the previous sent transaction is set to isSpent equals True
				Assert.Equal(2, result.Transactions.Count);
				var previousSentTx = result.Transactions.ToList().Find(tx => tx.TransactionId == prevTx.Id);

				Assert.NotNull(previousSentTx);
				Assert.True(previousSentTx.isSpent);
			}
			[Fact]
			public async Task Should_Return_SentTransactions_And_RecievedTransactions()
			{
				var mnemonic = "bar industry slogan keen will diagram camera flash slight enrich nose loyal argue coconut fortune oyster service throw course hazard author gloom crater purity fringe";
				var address = "ws1jkcqmht6c6t6j62r7227t6808cdsc5e5kk6jjxakht8f2h42plp2qkn0ywx";
				var registerId = Guid.NewGuid().ToString();
				var sentTxId = Guid.NewGuid().ToString();
				var recievedTxId = Guid.NewGuid().ToString();
				var recieved = new List<TransactionModel> { new() { TxId = recievedTxId, RecipientsWallets = new List<string> { Guid.NewGuid().ToString() } } };
				var sent = new List<TransactionModel> { new() { TxId = sentTxId, SenderWallet = address } };
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, A<string>.Ignored)).Returns(recieved);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, A<string>.Ignored)).Returns(sent);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);
				Assert.Equal(2, result.Transactions.Count);
				var sentWalletTx = result.Transactions.ToList().Find(tx => tx.TransactionId == sentTxId);
				var recievedWalletTx = result.Transactions.ToList().Find(tx => tx.TransactionId == recievedTxId);
				Assert.NotNull(sentWalletTx);
				Assert.True(sentWalletTx.isSpent);
				Assert.True(sentWalletTx.isConfirmed);
				Assert.NotNull(recievedWalletTx);
				Assert.False(recievedWalletTx.isSpent);
				Assert.True(recievedWalletTx.isConfirmed);
			}
			[Fact]
			public async Task Should_UpdateSentTransactionisSpentProperty_WhenRecievedTransactionIsAlsoInSentTransactions()
			{
				var mnemonic = "barrel ribbon brand essay sport assume price spell spice stable awake fortune pony sick treat wealth heavy turn pull champion bike robot sleep tornado pool";
				var address = "ws1jvn66vgcmy9v7pn2kaesj0dqgjayvn7ud4sh202gjp7nxc0ny6lus9wa9gk";
				var registerId = Guid.NewGuid().ToString();
				var transactionId = Guid.NewGuid().ToString();
				var transactions1 = new List<TransactionModel> { new() { TxId = transactionId, SenderWallet = address, RecipientsWallets = new List<string> { address } } };
				var transactions2 = new List<TransactionModel> { new() { TxId = transactionId, SenderWallet = address, RecipientsWallets = new List<string> { address } } };
				A.CallTo(() => _mockServiceClient.GetAllTransactionsByRecipientAddress(registerId, A<string>.Ignored)).Returns(transactions1);
				A.CallTo(() => _mockServiceClient.GetAllTransactionsBySenderAddress(registerId, A<string>.Ignored)).Returns(transactions2);
				var (result, _) = await _underTest.BuildWallet(null, mnemonic, registerId, null, null, false);
				Assert.Single(result.Transactions);
				Assert.False(result.Transactions.First().isSpent);
			}
		}
	}
}
