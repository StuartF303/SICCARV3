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
