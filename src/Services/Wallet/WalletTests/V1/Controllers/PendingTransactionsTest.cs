using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletService.V1.Controllers;
using Xunit;
using WalletService.Core.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using WalletService.Core;
using Microsoft.Extensions.Logging;

namespace WalletService.UnitTests.V1.Controllers
{
	public class PendingTransactionsTest
	{
		private readonly PendingTransactionsController _underTest;
		private readonly IWalletRepository _walletRepositoryMock;
		private readonly ILogger<PendingTransactionsController> _loggerMock;
		private readonly string expectedOwner = Guid.NewGuid().ToString();

		public PendingTransactionsTest()
		{
			_walletRepositoryMock = A.Fake<IWalletRepository>();
			_loggerMock = A.Fake<ILogger<PendingTransactionsController>>();
			_underTest = new PendingTransactionsController(_walletRepositoryMock, _loggerMock)
			{
				ControllerContext = new ControllerContext()
				{
					// so going to try to add a user sub, wtf IFeaturesCollection
					HttpContext = new DefaultHttpContext(),
				}
			};
		}


		public class StoreTransactionId : PendingTransactionsTest
		{
			// these tests check that a transaction is added aspending and then completed on PendingTransactions Controller

			[Fact]
			public async Task Should_CallUpddateTransactionsForSender()
			{
				var transationId = Guid.NewGuid().ToString();
				var expected = new PendingTransaction { Id = transationId };
				var expected2 = new WalletTransaction { Id = transationId };
				var senderAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var recipientAddress = "ws1ydyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shyt";
				var transactionConfirmedPayload = new TransactionConfirmed 
				{ 
					ToWallets = [recipientAddress], 
					TransactionId = transationId, Sender = senderAddress,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action
					}
				};

				await _underTest.UpdatePendingTxsForWallets(transactionConfirmedPayload);

				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(
					senderAddress, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_CallUpddateTransactionsForEachRecipient()
			{
				var transationId = Guid.NewGuid().ToString();
				var expected = new PendingTransaction { Id = transationId };
				var expected2 = new WalletTransaction { Id = transationId };
				var senderAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var recipientAddress = "ws1ydyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shyt";
				var recipientAddress2 = "ws1jeyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shgi";
				var transactionConfirmedPayload = new TransactionConfirmed
				{
					ToWallets = [recipientAddress, recipientAddress2], 
					TransactionId = transationId, Sender = senderAddress,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action
					}
				};

				await _underTest.UpdatePendingTxsForWallets(transactionConfirmedPayload);

				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress2, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_CallUpddateTransactionsForEachRecipientMesh()
			{
				var transationId = "3fce4c590d19bbae56faac106179c3cedd5de3400514bb41cbebf2385ec0fccb";
				var expected = new PendingTransaction { Id = transationId };
				var expected2 = new WalletTransaction { Id = transationId };
				var senderAddress = "ws1jjh7rqdfx6cexrpv2ler6mmq8y9r7dh43k645umrsgl2kvpz46wuq2fvnne";
				var recipientAddress = "ws1ydyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shyt";
				var recipientAddress2 = "ws1jqsxwdc00vyjhu7shx9lsu7kqe2q7qgvuwm423mgqvy9vgp6r85gqr2apfc";
				var recipientAddress3 = "ws1jpeg97g5p6ucumqmst5r9v5qekm2frj6zf7zj7yssxx7uyysnlprsvmx5sl";
				var recipientAddress4 = "ws1jxyv89p35arjld9mm2a9tn5tk4ytkcuzcv2jkkk9748xvnjvxu95qh2uvcd";
				var recipientAddress5 = "ws1jwhklehh69v99v76jlx56zv0nt0qlymqgxx2jds287d938ukjx9uqr9ejgc";
				var recipientAddress6 = "ws1j2hsnc32swjy6eg5kk7a0pvesk9ycr3sl6366cg9sgnvndcvqgn3s9ed7z4";

				List<string> toWallets = [senderAddress, recipientAddress, recipientAddress2, recipientAddress3, recipientAddress4, recipientAddress5, recipientAddress6];
				var transactionConfirmedPayload = new TransactionConfirmed
				{
					ToWallets = toWallets,
					TransactionId = transationId,
					Sender = senderAddress,
					MetaData = new TransactionMetaData
					{
						TransactionType = TransactionTypes.Action
					}
				};

				await _underTest.UpdatePendingTxsForWallets(transactionConfirmedPayload);

				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(senderAddress, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress2, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress3, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress4, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress5, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				A.CallTo(() => _walletRepositoryMock.WalletUpdateTransactions(recipientAddress6, transationId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
		}

		public class GetTransactions : PendingTransactionsTest
		{
			public GetTransactions()
			{
				var claim = new Claim("sub", expectedOwner);
				var claims = new ClaimsIdentity(new List<Claim> { claim });
				_underTest.HttpContext.User = new ClaimsPrincipal(claims);
			}

			[Fact]
			public async Task Should_Return_RecentTransactions()
			{ 
				var transationId = Guid.NewGuid().ToString();
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var walletTx = new WalletTransaction
				{
					Id = transationId,
					TransactionId = transationId,
					WalletId = walletAddress,
					MetaData = new TransactionMetaData { BlueprintId = Guid.NewGuid().ToString() },
					isConfirmed = true,
					isSendingWallet = false,
					isSpent = false
				};
				var expected = new PendingTransaction
				{
					Id = transationId,
					MetaData = new TransactionMetaData { BlueprintId = walletTx.MetaData.BlueprintId }
				};

				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(new Wallet { Address = walletAddress, Transactions = new List<WalletTransaction> { walletTx } });
				

				var response = await _underTest.GetTransactions(walletAddress);

				var result = response.Result as OkObjectResult;
				var resultObject = result.Value as List<PendingTransaction>;
				Assert.Single(resultObject);
				Assert.Equal(expected.Id, resultObject[0].Id);
				Assert.Equal(JsonSerializer.Serialize(expected.MetaData), JsonSerializer.Serialize(resultObject[0].MetaData));
			}

			[Fact]
			public async Task Should_Not_ReturnTransactionsWhere_TxIsNotConfirmed()
			{
				var transactionId = Guid.NewGuid().ToString();
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var walletTx = new WalletTransaction
				{
					Id = transactionId,
					TransactionId = transactionId,
					WalletId = walletAddress,
					MetaData = new TransactionMetaData { BlueprintId = Guid.NewGuid().ToString() },
					isSpent = false,
					isConfirmed = false,
					isSendingWallet = false
				};
				var expected = new PendingTransaction
				{
					Id = transactionId,
					MetaData = new TransactionMetaData { BlueprintId = walletTx.MetaData.BlueprintId }
				};

				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(new Wallet { Address = walletAddress, Transactions = new List<WalletTransaction> { walletTx } });

				var response = await _underTest.GetTransactions(walletAddress);

				var result = response.Result as OkObjectResult;
				var resultObject = result.Value as List<PendingTransaction>;
				Assert.Empty(resultObject);
			}

			[Fact]
			public async Task Should_Not_ReturnTransactionsWhere_TxIsSpent()
			{
				var transactionId = Guid.NewGuid().ToString();
				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				var walletTx = new WalletTransaction
				{
					Id = transactionId,
					TransactionId = transactionId,
					WalletId = walletAddress,
					MetaData = new TransactionMetaData { BlueprintId = Guid.NewGuid().ToString() },
					isSpent = true,
					isConfirmed = false,
					isSendingWallet = false
				};
				var expected = new PendingTransaction
				{
					Id = transactionId,
					MetaData = new TransactionMetaData { BlueprintId = walletTx.MetaData.BlueprintId }
				};

				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(new Wallet { Address = walletAddress, Transactions = new List<WalletTransaction> { walletTx } });

				var response = await _underTest.GetTransactions(walletAddress);
				var result = response.Result as OkObjectResult;
				var resultObject = result.Value as List<PendingTransaction>;
				Assert.Empty(resultObject);
			}

			[Fact]
			public async Task Should_Throw_WhenWalletIsNull()
			{
				var expectedOwner = Guid.NewGuid().ToString();
				var claim = new Claim("sub", expectedOwner);
				var claims = new ClaimsIdentity(new List<Claim> { claim });
				await Task.Run(() => _underTest.HttpContext.User = new ClaimsPrincipal(claims));

				var walletAddress = "ws1jnyured8rvkmcw79ds2afcd7zpskgpddst0uxrlzjyyx92mclfh5qs3shxh";
				A.CallTo(() => _walletRepositoryMock.GetWallet(walletAddress, expectedOwner)).Returns(null as Wallet);
				await (_ = Assert.ThrowsAsync<HttpStatusException>(() => _underTest.GetTransactions(walletAddress)));
			}
		}
	}
}
