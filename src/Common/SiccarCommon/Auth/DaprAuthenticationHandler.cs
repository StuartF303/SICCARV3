// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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
