using FakeItEasy;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Common;
using Siccar.Platform;
using Siccar.Registers.Core;
using Siccar.Registers.RegisterService.V1.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace RegisterTests.V1.Services
{
    public class RegisterResolverTest
    {
        private readonly RegisterResolver _underTest;
        private readonly ITenantServiceClient _tenantServiceClient;
        private readonly IRegisterRepository _registerRepository;
        private const string _tenantId = "a5f4ca33-833b-4bcc-899c-23641c1f41b3";

        public RegisterResolverTest()
        {
            _tenantServiceClient = A.Fake<ITenantServiceClient>();
            _registerRepository = A.Fake<IRegisterRepository>();
            _underTest = new RegisterResolver(_registerRepository, _tenantServiceClient);
        }

        public class ResolveRegistersForUser : RegisterResolverTest
        {
            [Fact]
            public async Task Should_CallTenantService_WithUserTenantClaim()
            {
                var claims = new List<Claim> { new Claim("tenant", _tenantId) };
                await _underTest.ResolveRegistersForUser(claims);
                A.CallTo(() => _tenantServiceClient.GetTenantById(_tenantId)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallRegisterRepository()
            {
                var claims = new List<Claim> { new Claim("tenant", _tenantId) };
                var expected = new Tenant
                {
                    Registers = new List<string> { Guid.NewGuid().ToString() }
                };
                A.CallTo(() => _tenantServiceClient.GetTenantById(_tenantId)).Returns(expected);

                await _underTest.ResolveRegistersForUser(claims);
                A.CallTo(() => _registerRepository.QueryRegisters(A<Func<Register, bool>>.Ignored)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnRegisters()
            {
                var claims = new List<Claim> { new Claim("tenant", _tenantId) };
                var expected = new List<Register> { new Register() };

                A.CallTo(() => _registerRepository.QueryRegisters(A<Func<Register, bool>>.Ignored)).Returns(expected);
                var result = await _underTest.ResolveRegistersForUser(claims);
                Assert.Equal(expected, result);
            }

            [Fact]
            public async Task Should_ThrowWhenGetTenantById_ReturnsNull()
            {
                var claims = new List<Claim> { new Claim("tenant", _tenantId) };
                A.CallTo(() => _tenantServiceClient.GetTenantById(_tenantId)).Returns(Task.FromResult<Tenant>(null!));
                var result = await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.ResolveRegistersForUser(claims));
                Assert.Equal(System.Net.HttpStatusCode.NotFound, result.Status);
            }

            [Fact]
            public async Task Should_CallGetAllRegisters_WhenUserHasInstallationRoles()
            {
                var claims = new List<Claim> { new Claim("role", Constants.InstallationAdminRole) };
                await _underTest.ResolveRegistersForUser(claims);
                A.CallTo(() => _registerRepository.GetRegistersAsync()).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnRegistersFromGetAllAsynce()
            {
                var claims = new List<Claim> { new Claim("role", Constants.InstallationAdminRole) };
                var expected = new List<Register> { new Register() };
                A.CallTo(() => _registerRepository.GetRegistersAsync()).Returns(expected);
                var result = await _underTest.ResolveRegistersForUser(claims);
                Assert.Equal(expected, result);
                A.CallTo(() => _registerRepository.QueryRegisters(A<Func<Register, bool>>.Ignored)).MustNotHaveHappened();
            }
        }

        public class ThrowIfUserNotAuthorizedForRegister : RegisterResolverTest
        {
            [Fact]
            public async Task ShouldThrowIfTenantDoesNot_HaveAccessToRegister()
            {
                var expectedRegisterId = Guid.NewGuid().ToString();
                var expectedTenant = Guid.NewGuid().ToString();
                var claims = new List<Claim> { new Claim("tenant", expectedTenant) };
                A.CallTo(() => _tenantServiceClient.GetTenantById(A<string>._))
                    .Returns(new Tenant { Registers = new List<string>() });

                await Assert.ThrowsAsync<HttpStatusException>(
                    () => _underTest.ThrowIfUserNotAuthorizedForRegister(claims, expectedRegisterId));

                A.CallTo(() => _tenantServiceClient.GetTenantById(expectedTenant)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task ShouldReturn_WhenTenantIsAuthorized()
            {
                var expectedRegisterId = Guid.NewGuid().ToString();
                var expectedTenant = Guid.NewGuid().ToString();
                var claims = new List<Claim> { new Claim("tenant", expectedTenant) };
                A.CallTo(() => _tenantServiceClient.GetTenantById(A<string>._))
                    .Returns(new Tenant { Registers = new List<string>() { expectedRegisterId } });

                await _underTest.ThrowIfUserNotAuthorizedForRegister(claims, expectedRegisterId);

                A.CallTo(() => _tenantServiceClient.GetTenantById(expectedTenant)).MustHaveHappenedOnceExactly();
            }
        }
    }
}
