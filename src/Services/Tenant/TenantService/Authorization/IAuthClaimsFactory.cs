using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Siccar.Platform.Tenants.Authorization
{
    public interface IAuthClaimsFactory
    {
        List<Claim> BuildLocalAuthorizationClaims(string tenant);
    }
}
