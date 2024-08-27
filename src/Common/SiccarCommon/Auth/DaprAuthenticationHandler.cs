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

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Siccar.Common.Auth
{
	public class DaprAuthenticationHandler(IOptionsMonitor<DaprAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder,
		ILogger<DaprAuthenticationHandler> authLogger) : AuthenticationHandler<DaprAuthOptions>(options, logger, encoder)
	{
		private readonly ILogger<DaprAuthenticationHandler> _authLogger = authLogger;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			if (!Request.Headers.ContainsKey("dapr-api-token"))
			{
				_authLogger.LogDebug("dapr-api-token was not present in the headers.");
				return AuthenticateResult.Fail("Unauthorized");
			}

			string authorizationHeader = Request.Headers["dapr-api-token"];
			_authLogger.LogDebug("dapr-api-token {header}", authorizationHeader);

			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadToken(authorizationHeader) as JwtSecurityToken;
			var key = token.Claims.First(claim => claim.Type == "SecretKey").Value;

			var secret = Options.DaprSecret;
			if (key == secret)
			{
				var claims = new List<Claim>
				{
					new(ClaimTypes.Role, "DaprService"),
					new(ClaimTypes.Role, "register.maintainer")
				};

				var identity = new ClaimsIdentity(claims, Scheme.Name);
				var principal = new ClaimsPrincipal(identity);
				var ticket = new AuthenticationTicket(principal, Scheme.Name);
				return AuthenticateResult.Success(ticket);
			};

			return AuthenticateResult.Fail("Unauthorized");
		}
	}

	public class DaprAuthOptions : AuthenticationSchemeOptions
	{
		public string DaprSecret { get; set; }
	}
}
