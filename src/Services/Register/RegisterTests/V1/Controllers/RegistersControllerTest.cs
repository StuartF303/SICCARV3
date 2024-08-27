using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using Siccar.Platform.Registers.Core.Models;
using Siccar.Registers.Core;
using Siccar.Registers.RegisterService.V1;
using Siccar.Registers.RegisterService.V1.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RegisterTests.V1.Controllers
{
    public class RegistersControllerTest
    {
        private readonly RegistersController _underTest;
        private readonly IRegisterResolver _registerResolver;
        private readonly IRegisterRepository _registerRepository;
        private IDaprClientAdaptor _fakeDaprClient;
        private const string TenantId = "a5f4ca33-833b-4bcc-899c-23641c1f41b3";
        public RegistersControllerTest()
        {
            var loggerMock = A.Fake<ILogger<RegistersController>>();
            _registerResolver = A.Fake<IRegisterResolver>();
            _registerRepository = A.Fake<IRegisterRepository>();
            _fakeDaprClient = A.Fake<IDaprClientAdaptor>();
            _underTest = new RegistersController(loggerMock, _registerRepository, _registerResolver, _fakeDaprClient)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                }
            };
            var claim = new Claim("tenant", TenantId);
            var claims = new ClaimsIdentity(new List<Claim> { claim });
            _underTest.HttpContext.User = new ClaimsPrincipal(claims);
        }

        public class GetAllRegisters : RegistersControllerTest
        {
          
            [Fact]
            public async Task Should_ReturnRegisters()
            {
                var expected = new List<Register> { new Register() };

                A.CallTo(() => _registerResolver.ResolveRegistersForUser(A<IEnumerable<Claim>>._)).Returns(expected);

                var result = await _underTest.GetAllRegisters();
                var resultObject = result as OkObjectResult;

                Assert.Equal(expected, resultObject!.Value);
            }
        }

        public class DeleteRegister : RegistersControllerTest
        {
            [Fact]
            public async Task Should_PublishPubSubEvent()
            {
                var expectedRegisterId = Guid.NewGuid().ToString();

                await _underTest.DeleteRegister(expectedRegisterId);

                A.CallTo(() => _fakeDaprClient.PublishEventAsync("pubsub", Topics.RegisterDeletedTopicName,
                    A<RegisterDeleted>.That.Matches(registerDeleted =>
                        registerDeleted.Id == expectedRegisterId &&
                        registerDeleted.TenantId == TenantId))).MustHaveHappenedOnceExactly();
            }
        }

        public class CreateRegister : RegistersControllerTest
        {
            [Fact]
            public async Task Should_PublishPubSubEvent()
            {
                var expected = new Register
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "test-register-name"
                };

                A.CallTo(() => _registerRepository.InsertRegisterAsync(A<Register>.Ignored)).Returns(expected);

                await _underTest.PostRegister(expected);

                A.CallTo(() => _fakeDaprClient.PublishEventAsync("pubsub", Topics.RegisterCreatedTopicName, 
                    A<RegisterCreated>.That.Matches(registerCreated => 
                        registerCreated.Id == expected.Id && 
                        registerCreated.Name == expected.Name &&
                        registerCreated.TenantId == TenantId))).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task MaximumNumberOfRegistersReached_ShouldReturnBadRequest()
            {
                var testRegister = new Register
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "test-register-name"
                };

                A.CallTo(() => _registerRepository.CountRegisters()).Returns(25);
              
                var response = await _underTest.PostRegister(testRegister);

                Assert.True(response is BadRequestObjectResult);
            }
        }
    }
}
