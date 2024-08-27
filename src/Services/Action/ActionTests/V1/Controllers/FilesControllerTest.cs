using ActionService.V1.Controllers;
using ActionService.V1.Repositories;
using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionUnitTests.V1.Controllers
{
    public class FilesControllerTest
    {
        private readonly FilesController _underTest;
        private readonly ILogger<FilesController> _fakeLogger;
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly IFileRepository _fakeFileRepository;
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        public FilesControllerTest()
        {
            _fakeLogger = A.Fake<ILogger<FilesController>>();
            _fakeFileRepository = A.Fake<IFileRepository>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _underTest = new FilesController(_fakeLogger, _fakeFileRepository, _fakeRegisterServiceClient, _fakeWalletServiceClient);
        }

        public class GetFileFromTransaction : FilesControllerTest
        {
            [Fact]
            public async Task Should_ThrowWhenThereIsNoFileTypeInTransactionMetaData()
            {
                var walletAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
                var registerId = Guid.NewGuid().ToString("N");
                var transactionId = Guid.NewGuid().ToString();
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { new byte[1] });

                await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.GetFileFromTransaction(walletAddress, registerId, transactionId));

                A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(registerId, transactionId)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallGetTransactionByIdAsync()
            {
                var walletAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
                var registerId = Guid.NewGuid().ToString("N");
                var transactionId = Guid.NewGuid().ToString();
                var expected = new TransactionModel() { MetaData = new() { TransactionType = TransactionTypes.File, TrackingData = new() { { "fileName", "myfile.jpg" }, { "fileType", "image/jpeg" }, { "fileExtension", ".jpg" } } } };
                A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(registerId, transactionId)).Returns(expected);
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { new byte[1] });

                await _underTest.GetFileFromTransaction(walletAddress, registerId, transactionId);

                A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(registerId, transactionId)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallDecryptPayloadsAsync()
            {
                var walletAddress = "ws1jxw6wuttyhyj7q2ntseneaef9qe4k29ctrxl505we75uw06mthdvs645zu2";
                var registerId = Guid.NewGuid().ToString("N");
                var transactionId = Guid.NewGuid().ToString();
                var expected = new TransactionModel() { MetaData = new() { TransactionType = TransactionTypes.File, TrackingData = new() { { "fileName", "myfile.jpg" }, { "fileType", "image/jpeg" }, { "fileExtension", ".jpg" } } } };
                A.CallTo(() => _fakeRegisterServiceClient.GetTransactionById(registerId, transactionId)).Returns(expected);
                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { new byte[1] });

                await _underTest.GetFileFromTransaction(walletAddress, registerId, transactionId);

                A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(expected, walletAddress)).MustHaveHappenedOnceExactly();
            }
        }

    }
}
