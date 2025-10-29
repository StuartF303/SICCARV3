// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Common.Auth
{
    public class NoTenantClaimRequirement : IAuthorizationRequirement
    {
        public NoTenantClaimRequirement()
        {
        }
    }
}
