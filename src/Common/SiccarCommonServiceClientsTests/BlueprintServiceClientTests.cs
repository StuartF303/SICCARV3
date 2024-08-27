using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using SiccarApplicationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Siccar.Common.ServiceClients.Tests
{
    public class BlueprintServiceClientTests 
    {
        private readonly IHttpContextAccessor _mockHttpAccessor;
        private readonly IHttpClientAdaptor _mockClientAdaptor;
        private readonly IServiceProvider _mockServices;
        private readonly SiccarBaseClient _siccarClient;
        private readonly IBlueprintServiceClient _underTest;
        private string _blueprintUrl = $"{Constants.BlueprintAPIURL.ToLower()}";
        private TestData _testData = new TestData();

        public BlueprintServiceClientTests()
        {
            _mockHttpAccessor = A.Fake<IHttpContextAccessor>();
            _mockClientAdaptor = A.Fake<IHttpClientAdaptor>();
            _mockServices = A.Fake<IServiceProvider>();

            var services = new ServiceCollection()
                .AddHttpContextAccessor()
                .AddLogging()
                .BuildServiceProvider();

            _siccarClient = new SiccarBaseClient(_mockClientAdaptor, services);
            _underTest = new BlueprintServiceClient(_siccarClient);
        }

        public class GetDraftBlueprint : BlueprintServiceClientTests
        {
            private string _expectedURL;
            readonly HttpResponseMessage response;
            private Blueprint _testBlueprint;

            public GetDraftBlueprint()
            {
                _testBlueprint = _testData.blueprint1();
                _expectedURL = $"{_blueprintUrl}/{_testBlueprint.Id}";
                response = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(_testBlueprint)
                };
            }

            [Fact]
            public async Task Should_CallPostAsync_WithExpectedURL()
            {
                A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedURL)).Returns(response);

                var result = await _underTest.GetBlueprintDraft(_testBlueprint.Id);

                A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedURL)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnBlueprint()
            {
                A.CallTo(() => _mockClientAdaptor.GetAsync(_expectedURL)).Returns(response);

                var result = await _underTest.GetBlueprintDraft(_testBlueprint.Id);

                Assert.Equal(_testBlueprint.Id, result.Id);
            }
        }
        public class StoreDraftBlueprint : BlueprintServiceClientTests
        {
            private string _expectedURL;
            readonly HttpResponseMessage response;
            private Blueprint _testBlueprint;

            public StoreDraftBlueprint()
            {
                _expectedURL = $"{_blueprintUrl}";
                _testBlueprint = _testData.blueprint1();
                response = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(_testBlueprint)
                };
            }

            [Fact]
            public async Task Should_CallPostAsync_WithExpectedURL()
            {
                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedURL, A<string>.Ignored)).Returns(response);

                var result = await _underTest.CreateBlueprintDraft(_testBlueprint);

                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnBlueprint()
            {
                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedURL, A<string>.Ignored)).Returns(response);

                var result = await _underTest.CreateBlueprintDraft(_testBlueprint);

                Assert.Equal(_testBlueprint.Id, result.Id);
            }
        }

        public class EditDraftBlueprint : BlueprintServiceClientTests
        {
            private string _expectedURL;
            readonly HttpResponseMessage response;
            private Blueprint _testBlueprint;

            public EditDraftBlueprint()
            {
                _testBlueprint = _testData.blueprint1();
                _expectedURL = $"{_blueprintUrl}/{_testBlueprint.Id}";
                response = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(_testBlueprint)
                };
            }

            [Fact]
            public async Task Should_CallPostAsync_WithExpectedURL()
            {
                A.CallTo(() => _mockClientAdaptor.PutAsync(_expectedURL, A<string>.Ignored)).Returns(response);

                var result = await _underTest.UpdateBlueprintDraft(_testBlueprint);

                A.CallTo(() => _mockClientAdaptor.PutAsync(_expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnBlueprint()
            {
                A.CallTo(() => _mockClientAdaptor.PutAsync(_expectedURL, A<string>.Ignored)).Returns(response);

                var result = await _underTest.UpdateBlueprintDraft(_testBlueprint);

                Assert.Equal(_testBlueprint.Id, result.Id);
            }
        }

        public class PublishBlueprint : BlueprintServiceClientTests
        {
            private string _expectedURL;
            private string _registerId;
            private string _walletaddress;
            private Blueprint _testBlueprint;
            readonly HttpResponseMessage response;

            public PublishBlueprint()
            {
                _registerId = Guid.NewGuid().ToString();
                _walletaddress = "ws1jksgpmhe8vcxhfx5ej4fhwyz7e4zzp4zw6er2russ4kwakzmt72eskqp38c";
                _expectedURL = $"{_blueprintUrl}/{_walletaddress}/{_registerId}/publish";
                _testBlueprint = _testData.blueprint1();
                response = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(_testBlueprint)
                };
            }

            [Fact]
            public async Task Should_CallPostAsync_WithExpectedURL()
            {
                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedURL, A<string>.Ignored)).Returns(response);

                var result = await _underTest.PublishBlueprint(_walletaddress, _registerId, _testBlueprint);

                A.CallTo(() => _mockClientAdaptor.PostAsync(_expectedURL, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            }
        }


    }
}
