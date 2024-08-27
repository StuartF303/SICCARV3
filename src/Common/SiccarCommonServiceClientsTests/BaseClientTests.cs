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

using FakeItEasy;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.Exceptions;
using Xunit;

namespace SiccarCommonServiceClientsTests
{
    public class BaseClientTests
    {
        private readonly IHttpClientAdaptor _mockClientAdaptor;
        private readonly SiccarBaseClient _siccarClient;

        public BaseClientTests()
        {
            _mockClientAdaptor = A.Fake<IHttpClientAdaptor>();

            var services = new ServiceCollection()
                .AddHttpContextAccessor()
                .AddLogging()
                .BuildServiceProvider();

            _siccarClient = new SiccarBaseClient(_mockClientAdaptor, services);
        }

        [Fact]
        public async Task EmptyResponseString()
        {
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("")
            });

            await Assert.ThrowsAsync<HttpStatusException>(async () => await _siccarClient.GetJsonAsync("test"));
        }

        [Fact]
        public async Task NullResponseString()
        {
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound));

            await Assert.ThrowsAsync<HttpStatusException>(async () => await _siccarClient.GetJsonAsync("test"));
        }

        [Fact]
        public async Task NonJsonResponseString()
        {
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("This is not json")
            });

            await Assert.ThrowsAsync<HttpStatusException>(async () => await _siccarClient.GetJsonAsync("test"));
        }

        [Fact]
        public async Task EmptyJsonObjectResponseString()
        {
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}")
            });

            try
            {
                await _siccarClient.GetJsonAsync("test");
            }
            catch (Exception e)
            {
                Assert.IsType<HttpStatusException>(e);
                var httpStatusException = e as HttpStatusException;
                Assert.Null(httpStatusException!.TraceId);
            }
        }

        [Fact]
        public async Task ValidProblemResponseJson()
        {
            var responseJson = """
                {
                    "traceId": "test-trace-id",
                    "detail": "Error detail"
                }
                """;
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(responseJson)
            });

            try
            {
                await _siccarClient.GetJsonAsync("test");
            }
            catch (Exception e)
            {
                Assert.IsType<HttpStatusException>(e);
                var httpStatusException = e as HttpStatusException;
                Assert.Equal("test-trace-id", httpStatusException!.TraceId);
                Assert.Equal("Not Found: Error detail", httpStatusException.Message);
            }
        }

        [Fact]
        public async Task JsonResponseNoTraceId()
        {
            var responseJson = """
                {                   
                    "detail": "Error detail"
                }
                """;
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(responseJson)
            });

            try
            {
                await _siccarClient.GetJsonAsync("test");
            }
            catch (Exception e)
            {
                Assert.IsType<HttpStatusException>(e);
                var httpStatusException = e as HttpStatusException;
                Assert.Null(httpStatusException!.TraceId);
                Assert.Equal("Not Found: Error detail", httpStatusException.Message);
            }
        }

        [Fact]
        public async Task JsonResponseNoDetail()
        {
            var responseJson = """
                {
                    "traceId": "test-trace-id"
                }
                """;
            A.CallTo(() => _mockClientAdaptor.GetAsync("/api/test")).Returns(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(responseJson)
            });

            try
            {
                await _siccarClient.GetJsonAsync("test");
            }
            catch (Exception e)
            {
                Assert.IsType<HttpStatusException>(e);
                var httpStatusException = e as HttpStatusException;
                Assert.Equal("test-trace-id", httpStatusException!.TraceId);
                Assert.Equal("Not Found: ", httpStatusException.Message);
            }
        }
    }
}
