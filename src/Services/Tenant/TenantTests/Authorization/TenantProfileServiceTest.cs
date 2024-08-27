using AspNetCore.Identity.MongoDbCore.Models;
using FakeItEasy;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using Siccar.Platform.Tenants.Authorization;
using Xunit;
using System.Linq;
using SiccarApplicationTests;
using Microsoft.Extensions.Configuration;
using Client = Siccar.Platform.Tenants.Core.Client;
using System.Linq.Expressions;

namespace TenantUnitTests.Authorization
{
    public class TenantProfileServiceTest
    {
        private ILogger<TenantProfileService> _fakeLogger;
        private readonly MongoClaim _userTenantClaim;
        private UserManager<ApplicationUser> _usersStore;
        private SignInManager<ApplicationUser> _signInManager;
        private IUserStore<ApplicationUser> _store;
        private IConfiguration _mockConfig;
        private ApplicationUser _user;
        private ApplicationUser _userWithoutClaim;
        private TenantProfileService _underTest;
        private IHttpContextAccessor _fakeHttpContext;
        private IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private ITenantRepository _mockTenantRepo;
        private TestData testData = new TestData();

        public TenantProfileServiceTest()
        {
            _fakeLogger = A.Fake<ILogger<TenantProfileService>>();
            _fakeHttpContext = A.Fake<IHttpContextAccessor>();
            _userTenantClaim = new MongoClaim { Type = "tenant", Value = testData.tenantId };
            _user = new ApplicationUser { Id = Guid.Parse("1469e0b3-345e-4922-a4e8-8b9afaaed6f2"), Claims = new List<MongoClaim> { _userTenantClaim } };
            _userWithoutClaim = new ApplicationUser { Id = Guid.Parse("f7cb5161-d307-4f20-a1d3-119eda81fde1"), Claims = new List<MongoClaim>() };
            _store = A.Fake<IUserRoleStore<ApplicationUser>>();
            _mockConfig = A.Fake<IConfiguration>();
            _claimsFactory = A.Fake<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockTenantRepo = A.Fake<ITenantRepository>();
            A.CallTo(() => _store.FindByIdAsync(_userWithoutClaim.Id.ToString(), A<CancellationToken>.Ignored)).Returns(_userWithoutClaim);
            A.CallTo(() => _store.FindByIdAsync(_user.Id.ToString(), A<CancellationToken>.Ignored)).Returns(_user);

            _usersStore = TestUserManager(_store);
            _signInManager = new SignInManager<ApplicationUser>(_usersStore, _fakeHttpContext, _claimsFactory, null, null, null, null);
            _underTest = new TenantProfileService(_fakeLogger, _usersStore, _signInManager, _mockTenantRepo, _mockConfig);

        }

        public class GetProfileDataAsync : TenantProfileServiceTest
        {
            [Fact]
            public async Task Should_Add_Tenant_Claim_From_UserStore()
            {
                var profileData = BuildProfileDataForSuccess(_user.Id.ToString());

                await _underTest.GetProfileDataAsync(profileData);

                var result = profileData.IssuedClaims.Find(claim => claim.Value == _userTenantClaim.Value);
                Assert.Equal(_userTenantClaim.Value, result.Value);
            }

            [Fact]
            public async Task Should_ReturnEmptyClaims_WhenUserDoesNotTenantClaim()
            {
                var profileData = BuildProfileDataForSuccess(_userWithoutClaim.Id.ToString());

                await _underTest.GetProfileDataAsync(profileData);

                Assert.Empty(profileData.IssuedClaims);
            }

            [Fact]
            public async Task Should_Add_RegistersClaim()
            {
                var expected = testData.registerId;
                var profileData = BuildProfileDataForSuccess(_user.Id.ToString());
                var retList = new List<Tenant> { new Tenant { Id = testData.tenantId, Name = _userTenantClaim.Value, Registers = new List<string> { expected } } };
                A.CallTo(() => _mockTenantRepo.Single(A<Expression<Func<Tenant, bool>>>.Ignored)).Returns(retList[0]);

                await _underTest.GetProfileDataAsync(profileData);

                var result = profileData.IssuedClaims.Find(claim => claim.Value == expected);
                Assert.Equal(expected, result.Value);
            }
        }

        private ProfileDataRequestContext BuildProfileDataForSuccess(string userId)
        {
            var retList = new List<Tenant> { new Tenant { Name = _userTenantClaim.Value, Registers = new List<string>() } };
            A.CallTo(() => _mockTenantRepo.All<Tenant>()).Returns(retList.AsQueryable());

            var subClaim = new Claim("sub", userId);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { subClaim }));

            return new ProfileDataRequestContext() { Subject = principal };
        }

        private IsActiveContext BuildActiveContextForSuccess(string userId)
        {
            var subClaim = new Claim("sub", userId);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { subClaim }));

            return new IsActiveContext(principal, new Client(), "test");
        }

        private static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? A.Fake<IUserRoleStore<TUser>>();
            
            var options = A.Fake<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            A.CallTo(() => options.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = A.Fake<IUserValidator<TUser>>();
            userValidators.Add(validator);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());
            var userManager = new UserManager<TUser>(store, options, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                A.Fake<ILogger<UserManager<TUser>>>());

            A.CallTo(() => validator.ValidateAsync(userManager, A<TUser>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Success));

            return userManager;
        }
    }
}
