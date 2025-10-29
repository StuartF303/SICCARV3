// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Siccar.Common.Adaptors;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SiccarApplicationTests;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#nullable enable

namespace Siccar.Common.ServiceClients.Tests
{
	public class WalletServiceClientTests
	{
		private readonly IHttpContextAccessor _mockHttpAccessor;
		private readonly IHttpClientAdaptor _mockClientAdaptor;
		private readonly SiccarBaseClient _siccarClient;
		private readonly WalletServiceClient _underTest;
		private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
		private readonly string _walletBaseURL = $"{Constants.WalletAPIURL}";
		private readonly string _pendingTxBaseURL = $"{Constants.PendingTransactionsAPIURL}";
		private readonly TestData testData = new();

		public WalletServiceClientTests()
		{
			_mockHttpAccessor = A.Fake<IHttpContextAccessor>();
			_mockClientAdaptor = A.Fake<IHttpClientAdaptor>();
			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
			var services = new ServiceCollection().AddHttpContextAccessor().AddLogging().BuildServiceProvider();
			_siccarClient = new SiccarBaseClient(_mockClientAdaptor, services);
			_underTest = new WalletServiceClient(_siccarClient);
		}

		public class CreateWallet : WalletServiceClientTests
		{
			// TODO: More & Better tests

			string expectedURL;
			readonly HttpResponseMessage response;

			public CreateWallet()
			{
				expectedURL = $"{_walletBaseURL}";
				response = new HttpResponseMessage(System.Net.HttpStatusCode.Created) { Content = new StringContent(testData.simpleWalletStr()) };
			}

			[Fact]
			public async Task Should_Create_WalletBasic()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.CreateWallet("A Random Test Wallet");
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Create_WalletWithDescription()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.CreateWallet(testData.walletName, testData.walletDescription);
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
				//Assert.True(result.Address == testData.walletAddress);
			}
			[Fact]
			public async Task Should_Create_WalletWithMnemonic()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.CreateWallet(testData.walletName, testData.walletDescription, testData.walletMnemonic);
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Delete_WalletByAddress()
			{
				expectedURL = $"/api/Wallets/{testData.walletAddress}";
				var deleteResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(String.Empty) };
				A.CallTo(() => _mockClientAdaptor.DeleteAsync(expectedURL)).Returns(deleteResponse);
				var result = await _underTest.DeleteWallet(testData.walletAddress);
				A.CallTo(() => _mockClientAdaptor.DeleteAsync(expectedURL)).MustHaveHappenedOnceExactly();
			}
		}

		public class DecryptTransaction : WalletServiceClientTests
		{
			readonly string txId = "";
			readonly string expectedURL = "";
			readonly TransactionModel bpTx = new();
			readonly HttpResponseMessage? response;
			public DecryptTransaction()
			{
				SetupTestForSuccess();
				txId = testData.blueprintTxId;
				bpTx = new TransactionModel() { TxId = txId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
				expectedURL = $"{_walletBaseURL}/{testData.walletAddress1}/transactions/decrypt";
				response = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent("[\"SGVsbG8K\"]")
				};
			}

			[Fact]
			public async Task Should_Call_PostAsyncWithCorrectParameters()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.DecryptTransaction(null, testData.walletAddress1);
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Call_PostAsyncWithPayloadData()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.DecryptTransaction(bpTx, testData.walletAddress1);
				var contentString = JsonSerializer.Serialize(bpTx, _options);
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, contentString)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Return_ByteArray_OnSuccess()
			{
				var expected = new byte[][] { Convert.FromBase64String("SGVsbG8K") };
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(expected)) };
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				var result = await _underTest.DecryptTransaction(new TransactionModel(), testData.walletAddress1);
				Assert.Equal(expected, result);
			}
			[Fact]
			public async Task Should_Throw_WhenResponseNotSuccess()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Wallet not found" };
				A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
				await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.DecryptTransaction(null, testData.walletAddress1));
			}
		}

		public class DecryptUnencryptedTransactions : WalletServiceClientTests
		{
			readonly string _expectedUrl;
			readonly TransactionModel _participantTransaction;
			readonly HttpResponseMessage? _response;
			public DecryptUnencryptedTransactions()
			{
				SetupTestForSuccess();
				var transactionId = "test-participant-transaction-id";
				_participantTransaction = new TransactionModel() { TxId = transactionId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Participant } };
				_expectedUrl = $"{_walletBaseURL}/transactions/decrypt";
				_response = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent("[\"SGVsbG8K\"]")
				};
			}

			[Fact]
			public async Task Should_Call_Decrypt_WithCorrectParameters()
			{
				A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedUrl, A<string>.Ignored)).Returns(_response);
				await _underTest.GetAccessiblePayloads(_participantTransaction);
				A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedUrl, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
		}

		public class GetWalletTransactions : WalletServiceClientTests
		{
			readonly string txId = "";
			readonly string expectedURL = "";
			public GetWalletTransactions()
			{
				SetupTestForSuccess();
				txId = testData.blueprintTxId;
				_ = new TransactionModel() { TxId = txId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
				expectedURL = $"{_pendingTxBaseURL}/{testData.walletAddress1}";
				_ = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent("[\"SGVsbG8K\"]")
				};
			}

			[Fact]
			public async Task Should_Call_PostAsyncWithCorrectURL()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);
				var result = await _underTest.GetWalletTransactions(testData.walletAddress1);
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Call_GetAsyncWithCorrectWalletAddress()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);
				var result = await _underTest.GetWalletTransactions(testData.walletAddress1);
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_Return_PendingTXs_OnSuccess()
			{
				var expected = new List<PendingTransaction> { new PendingTransaction() };
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(expected)) };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);
				var result = await _underTest.GetWalletTransactions(testData.walletAddress1);
				Assert.Equal(expected, result);
			}
			[Fact]
			public async Task Should_Throw_WhenResponseNotSuccess()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Wallet not found" };
				A.CallTo(() => _mockClientAdaptor.GetAsync(A<string>.Ignored)).Returns(response);
				await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.GetWalletTransactions("someaddress"));
			}
		}

		public class GetAllTransactions : WalletServiceClientTests
		{
			readonly string _expectedUrl;
			public GetAllTransactions()
			{
				SetupTestForSuccess();
				_expectedUrl = $"{_walletBaseURL}/{testData.walletAddress1}/transactions";
				_ = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent("[]")
				};
			}

			[Fact]
			public async Task Should_Call_GetAsyncWithCorrectURL()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") };
				A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedUrl)).Returns(response);
				await _underTest.GetAllTransactions(testData.walletAddress1);
				A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedUrl)).MustHaveHappenedOnceExactly();
			}
		}

		public class AddDelegate : WalletServiceClientTests
		{
			readonly string _expectedUrl;
			public AddDelegate()
			{
				SetupTestForSuccess();
				_expectedUrl = $"{_walletBaseURL}/{testData.walletAddress1}/delegates";
				_ = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
			}

			[Fact]
			public async Task Should_Call_PostAsyncWithCorrectURL()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("{}") };
				A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedUrl, A<string>.Ignored)).Returns(response);
				await _underTest.AddDelegate(testData.walletAddress1, new WalletAccess());
				A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedUrl, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
		}
		public class DeleteDelegate : WalletServiceClientTests
		{
			readonly string _expectedSubject;
			readonly string _expectedUrl;
						
			public DeleteDelegate()
			{
				SetupTestForSuccess();
				_expectedSubject = "test-subject";
				_expectedUrl = $"{_walletBaseURL}/{testData.walletAddress1}/{_expectedSubject}/delegates";
				_ = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
			}

			[Fact]
			public async Task Should_Call_DeleteAsyncWithCorrectURL()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("{}") };
				A.CallTo(() => _mockClientAdaptor.DeleteAsync(_expectedUrl)).Returns(response);
				await _underTest.DeleteDelegate(testData.walletAddress1, _expectedSubject);
				A.CallTo(() => _mockClientAdaptor.DeleteAsync(_expectedUrl)).MustHaveHappenedOnceExactly();
			}
		}

		private void SetupTestForSuccess()
		{
			var headers = new HeaderDictionary
			{
				["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
			};
			A.CallTo(() => _mockHttpAccessor.HttpContext!.Request.Headers).Returns(headers);
		}
	}
}
