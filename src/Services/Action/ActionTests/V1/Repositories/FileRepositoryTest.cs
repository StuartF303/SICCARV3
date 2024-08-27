using ActionService.ActionConstants;
using ActionService.V1.Repositories;
using ActionUnitTests.V1.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Siccar.Common.Adaptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Net.WebRequestMethods;

namespace ActionUnitTests.V1.Repositories
{
    public class FileRepositoryTest
    {
        private readonly ILogger<FileRepository> _fakeLogger;
        private readonly IDaprClientAdaptor _fakeClient;
        private readonly IFileRepository _underTest;
        public FileRepositoryTest()
        {
            _fakeClient = A.Fake<IDaprClientAdaptor>();
            _fakeLogger = A.Fake<ILogger<FileRepository>>();
            _underTest = new FileRepository(_fakeClient, _fakeLogger);
        }
        public class StoreFile : FileRepositoryTest
        {
            [Fact]
            public async Task ShouldCallDaprClientWithExpectedPayload()
            {
                var expected = new byte[1];
                var stream = new MemoryStream(expected);
                var file = new FormFile(stream, 0, 1, "testFile", "myFile.txt");
                file.Headers = new HeaderDictionary(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "contentType", "text/string" } });

                var result = await _underTest.StoreFile(file, 1000000);

                A.CallTo(() => _fakeClient
                .InvokeBindingAsync(StringConstants.FileBindingActionName, StringConstants.FileCreateActionName, A<byte[]>.That.IsSameSequenceAs(expected), A<Dictionary<string, string>>._))
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task ShouldCallDaprClientWithExpectedMetaData()
            {
                var bytes = new byte[1];
                var fileName = "myFile.txt";
                var stream = new MemoryStream(bytes);
                var file = new FormFile(stream, 0, 1, "testFile", fileName);
                file.Headers = new HeaderDictionary(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "contentType", "text/string" } });

                var expected = new Dictionary<string, string> {
                                { "fileName", fileName },
                                { "blobName", fileName },
                                { "key", fileName },
                                { "contentType", file.ContentType }
                            };

                var result = await _underTest.StoreFile(file, 1000000);

                A.CallTo(() => _fakeClient
                .InvokeBindingAsync(StringConstants.FileBindingActionName, StringConstants.FileCreateActionName, A<byte[]>._,
                A<Dictionary<string, string>>.That.Matches(dict =>
                dict.Keys.Contains("fileName") &&
                dict.Keys.Contains("blobName") &&
                dict.Keys.Contains("key") &&
                dict.Keys.Contains("contentType") &&
                dict["contentType"] == expected["contentType"]
                )))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
