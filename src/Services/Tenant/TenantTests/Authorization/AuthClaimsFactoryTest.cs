using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using FakeItEasy;
using Siccar.Platform;
using Siccar.Platform.Tenants.Authorization;
using Xunit;
using System;

namespace TenantUnitTests.Authorization
{
    public class AuthClaimsFactoryTest
    {
        private readonly AuthClaimsFactory _underTest;

        public AuthClaimsFactoryTest()
        {
            _underTest = new AuthClaimsFactory();
        }

        public class BuildLocalAuthorizationClaims : AuthClaimsFactoryTest
        {
            [Fact]
            public void Should_ReturnClaimsList_With_IdentityProvider()
            {
                var expectedId = Guid.NewGuid().ToString();
                var expected = new Claim("tenant", expectedId);

                var claims = _underTest.BuildLocalAuthorizationClaims(expectedId);

                var result = claims.Find(claim => claim.Value == expectedId);
                Assert.NotNull(result);
                Assert.Equal(expected.Type, result.Type);
                Assert.Equal(expected.Value, result.Value);
            }
        }
    }
}
