// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Common.Auth
{
    public class NoTenantClaimAuthorizationHandler : AuthorizationHandler<NoTenantClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NoTenantClaimRequirement requirement)
        {
            
            if (context.User.HasClaim(claim => claim.Type == "iss") && !context.User.HasClaim(claim => claim.Type == "tenant"))
            {
                context.Fail(new AuthorizationFailureReason(this, "Tokens containing an Issuer must contain a 'tenant' claim."));
                return Task.CompletedTask;
            }

            foreach (var r in context.Requirements
                                     .Where(e => e is NoTenantClaimRequirement ||
                                                 e is ClaimsAuthorizationRequirement cu && cu.ClaimType == "tenant"))
            {
                //mark all tenant requirements as succeeded
                context.Succeed(r);
            }
            return Task.CompletedTask;
        }
    }
}
