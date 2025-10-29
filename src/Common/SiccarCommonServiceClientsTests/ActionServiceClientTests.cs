// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SiccarApplicationTests;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Siccar.Common.ServiceClients.Tests
{
    public class ActionServiceClientTests
    {
        private readonly IHttpContextAccessor _mockHttpAccessor;
        private readonly string _actionBaseURL = $"{Constants.ActionAPIURL}";
        private readonly TestData testData = new();

        public ActionServiceClientTests()
        {
            _mockHttpAccessor = A.Fake<IHttpContextAccessor>();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var services = new ServiceCollection()
                .AddHttpContextAccessor()
                .AddLogging()   
                .BuildServiceProvider();
        }

        public class GetActions : ActionServiceClientTests
        {
            readonly string txId = "";
            readonly TransactionModel bpTx = new();

            public GetActions()
            {
                SetupTestForSuccess();
                txId = testData.blueprintTxId;
                bpTx = new TransactionModel() { TxId = txId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
                _ = $"{_actionBaseURL}/{testData.walletAddress1}/transactions";
                _ = new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Content = new StringContent(JsonSerializer.Serialize(bpTx))
                };
            }

            //[Fact]
            //public async Task Should_Call_PostAsyncWithCorrectParameters()
            //{

            //    A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);
            //    // should a post not return a 'created'

            //    var result = await _underTest.CreateAndSendTransaction(null, testData.walletAddress1);

            //    A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            //}


            //[Fact]
            //public async Task Should_Call_PostAsyncWithPayloadData()
            //{
            //    var expected = new CreateTransactionRequest { TransactionMetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
            //    var contentString = JsonSerializer.Serialize(expected, _options);

            //    A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

            //    var result = await _underTest.CreateAndSendTransaction(expected, testData.walletAddress1);

            //    A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, contentString)).MustHaveHappenedOnceExactly();
            //}

            //[Fact]
            //public async Task Should_Return_Transaction_OnSuccess()
            //{

            //    A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

            //    var result = await _underTest.CreateAndSendTransaction(new CreateTransactionRequest(), testData.walletAddress1);

            //    Assert.Equal(txId, result.TxId);
            //}


        }

        //public class DecryptTransaction : ActionServiceClientTests
        //{
        //    string regId = "";
        //    string txId = "";
        //    string txQuery = "";
        //    string expectedURL = "";
        //    string instanceId = "";
        //    Transaction bpTx = new Transaction();
        //    HttpContent content = new StringContent("{}");
        //    HttpResponseMessage response;

        //    public DecryptTransaction()
        //    {
        //        SetupTestForSuccess();
        //        txId = testData.blueprintTxId;
        //        bpTx = new Transaction() { TxId = txId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
        //        expectedURL = $"{_actionBaseURL}/{testData.walletAddress1}/transactions/decrypt";
        //        response = new HttpResponseMessage
        //        {
        //            StatusCode = System.Net.HttpStatusCode.OK,
        //            Content = new StringContent("[\"SGVsbG8K\"]")
        //        };
        //    }

        //    [Fact]
        //    public async Task Should_Call_PostAsyncWithCorrectParameters()
        //    {
        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

        //        var result = await _underTest.DecryptTransaction(null, testData.walletAddress1);

        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        //    }


        //    [Fact]
        //    public async Task Should_Call_PostAsyncWithPayloadData()
        //    {
        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

        //        var result = await _underTest.DecryptTransaction(bpTx, testData.walletAddress1);

        //        var contentString = JsonSerializer.Serialize(bpTx, _options);
        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, contentString)).MustHaveHappenedOnceExactly();
        //    }

        //    [Fact]
        //    public async Task Should_Return_ByteArray_OnSuccess()
        //    {
        //        var expected = new byte[][] { Convert.FromBase64String("SGVsbG8K") };
        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(expected)) };
                
        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

        //        var result = await _underTest.DecryptTransaction(new Transaction(), testData.walletAddress1);
        //        Assert.Equal(expected, result);
        //    }

        //    [Fact]
        //    public async Task Should_Throw_WhenResponseNotSuccess()
        //    {
        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Wallet not found" };
        //        A.CallTo(() => _mockClientAdaptor.PostAsync(expectedURL, A<string>.Ignored)).Returns(response);

        //        await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.DecryptTransaction(null, testData.walletAddress1));
        //    }
        //}

        //public class GetWalletTransactions : ActionServiceClientTests
        //{
        //    string regId = "";
        //    string txId = "";
        //    string txQuery = "";
        //    string expectedURL = "";
        //    string instanceId = "";
        //    Transaction bpTx = new Transaction();
        //    HttpContent content = new StringContent("{}");
        //    HttpResponseMessage response;

        //    public GetWalletTransactions()
        //    {
        //        SetupTestForSuccess();
        //        txId = testData.blueprintTxId;
        //        bpTx = new Transaction() { TxId = txId, MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Blueprint } };
        //        expectedURL = $"{_pendingTxBaseURL}/{testData.walletAddress1}";
        //        response = new HttpResponseMessage
        //        {
        //            StatusCode = System.Net.HttpStatusCode.OK,
        //            Content = new StringContent("[\"SGVsbG8K\"]")
        //        };
        //    }

        //    [Fact]
        //    public async Task Should_Call_PostAsyncWithCorrectURL()
        //    {
    
        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") };
        //        A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);


        //        var result = await _underTest.GetWalletTransactions(testData.walletAddress1);

        //        A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
        //    }

        //    [Fact]
        //    public async Task Should_Call_GetAsyncWithCorrectWalletAddress()
        //    {

        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") };
        //        A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

        //        var result = await _underTest.GetWalletTransactions(testData.walletAddress1);

        //        A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).MustHaveHappenedOnceExactly();
        //    }

        //    [Fact]
        //    public async Task Should_Return_PendingTXs_OnSuccess()
        //    {

        //        var expected = new List<PendingTransaction> { new PendingTransaction() };
        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(expected)) };
        //        A.CallTo(() => _mockClientAdaptor.GetAsync(expectedURL)).Returns(response);

        //        var result = await _underTest.GetWalletTransactions(testData.walletAddress1);
        //        Assert.Equal(expected, result);
        //    }

        //    [Fact]
        //    public async Task Should_Throw_WhenResponseNotSuccess()
        //    {

        //        var response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, ReasonPhrase = "Wallet not found" };
        //        A.CallTo(() => _mockClientAdaptor.GetAsync(A<string>.Ignored)).Returns(response);

        //        await Assert.ThrowsAsync<HttpStatusException>(async () => await _underTest.GetWalletTransactions("someaddress"));
        //    }
        //}

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

