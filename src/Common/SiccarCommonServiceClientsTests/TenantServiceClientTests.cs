// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Common.Adaptors;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Net.Http.Json;
using System.Net.Http;
using Siccar.Application;
using Siccar.Platform;

namespace Siccar.Common.ServiceClients.Tests
{
    public class TenantServiceClientTests
    {
        private readonly IHttpClientAdaptor _mockClientAdaptor;
        private readonly ITenantServiceClient _underTest;
        private readonly string _tenantUrl = $"{Constants.TenantAPIURL}";

        public TenantServiceClientTests()
        {
            _mockClientAdaptor = A.Fake<IHttpClientAdaptor>();

            var services = new ServiceCollection()
                .AddHttpContextAccessor()
                .AddLogging()
                .BuildServiceProvider();

            var siccarClient = new SiccarBaseClient(_mockClientAdaptor, services);
            _underTest = new TenantServiceClient(siccarClient);
        }

        public class GetPublishedParticipants : TenantServiceClientTests
        {
            private readonly string _expectedUrl;
            private readonly string _expectedRegisterId;
            readonly HttpResponseMessage _response;

            public GetPublishedParticipants()
            {
                _expectedRegisterId = "test-register";
                _expectedUrl = $"{_tenantUrl}/{_expectedRegisterId}/participants";
                _response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<Participant>())
                };
            }

            [Fact]
            public async Task ShouldCall_GetPublishedParticipants_WithCorrectUrl()
            {
                A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedUrl)).Returns(_response);

                await _underTest.GetPublishedParticipants(_expectedRegisterId);

                A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedUrl)).MustHaveHappenedOnceExactly();
            }
        }

        public class PublishParticipant : TenantServiceClientTests
        {
            private readonly string _expectedUrl;
            private readonly string _expectedRegisterId;
            private readonly string _expectedWalletAddress;
            
            readonly HttpResponseMessage _response;

            public PublishParticipant()
            {
                _expectedRegisterId = "test-register";
                _expectedWalletAddress = "test-wallet";
                _expectedUrl = $"{_tenantUrl}/publishparticipant/{_expectedRegisterId}/{_expectedWalletAddress}";
                _response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new TransactionModel())
                };
            }

            [Fact]
            public async Task ShouldCall_GetPublishedParticipants_WithCorrectUrl()
            { 
                A.CallTo(() => _mockClientAdaptor.PostAsync(A<string>.Ignored, A<string>.Ignored)).Returns(_response);

                await _underTest.PublishParticipant(_expectedRegisterId, _expectedWalletAddress,
                    new Participant
                    {
                        Name = "test"
                    });

                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedUrl, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            }
        }

    }

}
