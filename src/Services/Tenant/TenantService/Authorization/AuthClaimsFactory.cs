using Siccar.Platform.Tenants.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Siccar.Platform.Tenants.Authorization
{
	public class AuthClaimsFactory : IAuthClaimsFactory
	{
		public List<Claim> BuildLocalAuthorizationClaims(string tenantId)
		{
			//ToDo: Lookup tenant information when dynamic providers has been implemented
			//This only brings in the identity provider posing as the tenant.
			var additionalLocalClaims = new List<Claim> { new Claim("tenant", tenantId)  };
			return additionalLocalClaims;
		}
	}
}
