// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System.ComponentModel.DataAnnotations;
using IdentityServer4.Models;
#nullable enable

namespace Siccar.Common.ServiceClients.Models.Tenant
{
	public class Client
	{
		public Guid Id { get; set; }
		[Required(ErrorMessage = "Please enter a Tenant ID")]
		public string TenantId { get; set; } = string.Empty;
		public bool Enabled { get; set; } = true;
		[Required(ErrorMessage = "Please enter a Client ID")]
		public string? ClientId { get; set; }
		public string ProtocolType { get; set; } = "oidc";
		public List<Secret> ClientSecrets { get; set; } = new List<Secret>();
		public bool RequireClientSecret { get; set; } = true;
		[Required(ErrorMessage = "Please enter a Client Name")]
		public string? ClientName { get; set; }
		public string? Description { get; set; }
		public string? ClientUri { get; set; }
		public string? LogoUri { get; set; }
		public bool RequireConsent { get; set; } = false;
		public bool AllowRememberConsent { get; set; } = true;
		public List<string> AllowedGrantTypes { get; set; } = new();
		public bool RequirePkce { get; set; } = true;
		public bool AllowPlainTextPkce { get; set; } = false;
		public bool RequireRequestObject { get; set; } = false;
		public bool AllowAccessTokensViaBrowser { get; set; } = false;
		public List<string> RedirectUris { get; set; } = new List<string>();
		public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
		public string? FrontChannelLogoutUri { get; set; }
		public bool FrontChannelLogoutSessionRequired { get; set; } = true;
		public string? BackChannelLogoutUri { get; set; }
		public bool BackChannelLogoutSessionRequired { get; set; } = true;
		public bool AllowOfflineAccess { get; set; } = false;
		public List<string> AllowedScopes { get; set; } = new List<string>();
		public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;
		public int IdentityTokenLifetime { get; set; } = 300;
		public ICollection<string> AllowedIdentityTokenSigningAlgorithms { get; set; } = new HashSet<string>();
		public int AccessTokenLifetime { get; set; } = 3600;
		public int AuthorizationCodeLifetime { get; set; } = 300;
		public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
		public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
		public int? ConsentLifetime { get; set; } = null;
		public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;
		public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = false;
		public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;
		public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;
		public bool EnableLocalLogin { get; set; } = true;
		public ICollection<string> IdentityProviderRestrictions { get; set; } = new HashSet<string>();
		public bool IncludeJwtId { get; set; } = true;
		public ICollection<ClientClaim> Claims { get; set; } = new HashSet<ClientClaim>();
		public bool AlwaysSendClientClaims { get; set; } = false;
		public string ClientClaimsPrefix { get; set; } = "client_";
		public string? PairWiseSubjectSalt { get; set; }
		public int? UserSsoLifetime { get; set; }
		public string? UserCodeType { get; set; }
		public int DeviceCodeLifetime { get; set; } = 300;
		public List<string> AllowedCorsOrigins { get; set; } = new List<string>();
		public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
	}
}
