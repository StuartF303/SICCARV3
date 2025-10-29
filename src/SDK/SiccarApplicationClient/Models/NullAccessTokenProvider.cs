// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors


using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Application.Client.Models
{
    public class NullAccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            return new ValueTask<AccessTokenResult>();
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return new ValueTask<AccessTokenResult>();
        }
    }
}
