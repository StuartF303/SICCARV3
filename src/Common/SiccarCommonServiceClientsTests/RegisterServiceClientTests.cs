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

namespace Siccar.Common.ServiceClients.Tests
{
	public class RegisterServiceClientTests
	{
		private readonly IHttpContextAccessor _mockHttpAccessor;
		private readonly IHttpClientAdaptor _mockClientAdaptor;
		private readonly SiccarBaseClient _siccarClient;
		private readonly IRegisterServiceClient _underTest;
		private readonly string _registerBaseURL = $"{Constants.RegisterAPIURL}";
		private readonly TestData testData = new();

		public RegisterServiceClientTests()
		{
			_mockHttpAccessor = A.Fake<IHttpContextAccessor>();
			_mockClientAdaptor = A.Fake<IHttpClientAdaptor>();
			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

			var services = new ServiceCollection()
				.AddHttpContextAccessor()
				.AddLogging()
				.BuildServiceProvider();

			_siccarClient = new SiccarBaseClient(_mockClientAdaptor, services);
			_underTest = new RegisterServiceClient(_siccarClient);
		}

		public class GetTransactionById : RegisterServiceClientTests
		{
			readonly string regId = "";
			readonly string expectedURL = "";
			readonly TransactionModel bpTx = new();
			readonly HttpResponseMessage response;

			public GetTransactionById()
			{
				SetupTestForSuccess();
				regId = testData.registerId;
				expectedURL = $"{_registerBaseURL}/{regId}/transactions/{testData.blueprintTxId}";
				bpTx =  new TransactionModel { TxId = testData.blueprintTxId } ;
			 
				response = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(bpTx))
				};
			}

			[Fact]
			public async Task Should_Call_GetAsyncWithCorrectURL_Return_Transaction_OnSuccess()
			{
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);
				var result = await _underTest.GetTransactionById(regId, testData.blueprintTxId  );
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
				Assert.Equal(bpTx.TxId, result.TxId);
			}
			[Fact]
			public async Task Should_Throw_WhenResponseNotSuccess()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Could not find register" };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);
				await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.GetTransactionById(regId, testData.blueprintTxId));
			}
		}

		public class GetBlueprintTransactions : RegisterServiceClientTests
		{
			readonly string regId = "";
			readonly string txQuery = "";
			readonly string expectedURL = "";
			readonly List<TransactionModel> bpTxs = [];
			readonly HttpContent content = new StringContent("{}");
			readonly HttpResponseMessage response;

			public GetBlueprintTransactions()
			{
				SetupTestForSuccess();
				regId = testData.registerId;
				txQuery = "?$filter=MetaData/transactionType eq 'Blueprint'";
				expectedURL = $"{_registerBaseURL}/{regId}/transactions{txQuery}";
				bpTxs = [new TransactionModel { TxId = testData.blueprintTxId }];
				var expectedString = JsonSerializer.Serialize(bpTxs);
				response = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(expectedString)
				};
			}

			[Fact]
			public async Task Should_Call_WithCorrectURL_Returns_TransactionList_OnSuccess()
			{

				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

				var result = await _underTest.GetBlueprintTransactions(regId);

				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
				Assert.Single(result);
				Assert.Equal(bpTxs[0].TxId, result[0].TxId);
			}

			[Fact]
			public async Task Should_Throw_WhenResponseNotSuccess()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Could not find register" };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

				await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.GetBlueprintTransactions(regId));
			}
		}
		
		public class GetTransactionsByInstanceId : RegisterServiceClientTests
		{
			readonly string regId = "";
			readonly string txQuery = "";
			readonly string expectedURL = "";
			readonly string instanceId = "";
			readonly List<TransactionModel> bpTxs = [];
			readonly HttpContent content = new StringContent("{}");
			readonly HttpResponseMessage response;

			public GetTransactionsByInstanceId()
			{
				SetupTestForSuccess();
				regId = testData.registerId;
				instanceId = Guid.NewGuid().ToString();
				txQuery = $"?$filter=MetaData/instanceId eq '{instanceId}'";
				expectedURL = $"{_registerBaseURL}/{regId}/transactions{txQuery}";
				bpTxs = [new TransactionModel { TxId = testData.blueprintTxId }];

				response = new HttpResponseMessage
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(bpTxs))
				};
			}


			[Fact]
			public async Task Should_Call_GetAsyncWithCorrectURL_Retunrs_Transaction_OnSuccess()
			{

				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(bpTxs)) };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

				var result = await _underTest.GetTransactionsByInstanceId(regId, instanceId);

				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
				Assert.Single(result);
				Assert.Equal(bpTxs[0].TxId, result[0].TxId);
			}



			[Fact]
			public async Task Should_Throw_WhenResponseNotSuccess()
			{
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Could not find register" };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

				await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.GetTransactionsByInstanceId(regId, instanceId));
			}
		}

		public class GetPublishedParticipantTransactions : RegisterServiceClientTests
		{
			readonly List<TransactionModel> _transactions = [];
			private readonly string _registerId = "test-register";

			[Fact]
			public async Task Should_Call_GetAsyncWithCorrectURL()
			{
				_transactions.Add(new TransactionModel { TxId = testData.blueprintTxId });
				var participantQuery = $"?$filter=MetaData/transactionType eq 'Participant' and MetaData/registerId eq '{_registerId}'";
				var expectedUrl = $"{_registerBaseURL}/{_registerId}/transactions{participantQuery}";
				var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(_transactions)) };
				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedUrl)).Returns(response);

				var result = await _underTest.GetParticipantTransactions(_registerId);

				A.CallTo(() => _mockClientAdaptor.GetAsync(expectedUrl)).MustHaveHappenedOnceExactly();
				Assert.Single(result!);
				Assert.Equal(_transactions[0].TxId, result![0].TxId);
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