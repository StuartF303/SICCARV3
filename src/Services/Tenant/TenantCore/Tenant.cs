using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Finbuckle.MultiTenant;
using Siccar.Platform.Tenants.Core;
using System.Text.Json.Serialization;

namespace Siccar.Platform
{
    public class Tenant :  ITenantInfo, IEquatable<Tenant>
    {
        /// <summary>
        /// A Tenant is a hosted organisation locally on this installation
        /// </summary>

        [Required]
        [Key]
        [RegularExpression(@"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$", ErrorMessage = "Tenant id must be a guid")]
        public string? Id { get; set; }

        public string? Identifier { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tenant name must be populated.")]
        public string? Name { get; set; }
        public string? ConnectionString { get; set; }

        public string? Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string SignInScheme { get; set; }
        public string SignOutScheme { get; set; }

        public string CallbackPath { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string RemoteSignOutPath { get; set; }
        public string ResponseType { get; set; }

        public string AdditionalScopes { get; set; }
        public bool GetClaimsFromUserInfoEndpoint { get; set; }

        public TokenValidationParameters TokenValidationParameters { get; set; }

        public bool SaveTokens { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tenant admin email must be populated.")]
        public string AdminEmail { get; set; }

        [EmailAddress]
        public string BillingEmail { get; set; }


        public List<string> Scope { get; set; }
        public List<string> Admins { get; set; }
        public List<string> Registers { get; set; }
        public List<Client> Clients { get; set; }
        public int AccountsCount { get; set; }

        public Tenant()
        {
            Id = Guid.NewGuid().ToString();
            Identifier = string.Empty;
            SaveTokens = true;
            Name = string.Empty;
            Authority = string.Empty;
            ConnectionString = string.Empty;
            ClientId = string.Empty;
            ClientSecret = string.Empty;
            SignOutScheme = "idsrv";
            SignInScheme = "idsrv.external";
            CallbackPath = string.Empty;
            SignedOutCallbackPath = string.Empty;
            RemoteSignOutPath = string.Empty;
            AdditionalScopes = string.Empty;
			TokenValidationParameters = new TokenValidationParameters { AuthenticationType = "oidc" };
			AdminEmail = string.Empty;
            BillingEmail = string.Empty;
            ResponseType = string.Empty;
            Scope = new List<string>();
            Admins = new List<string>();
            Registers = new List<string>();
            Clients = new List<Client>();
        }

        public bool Equals(Tenant? other)
        {
            return other != null &&
            Id == other.Id &&
            Identifier == other.Identifier &&
            Name == other.Name &&
            ConnectionString == other.ConnectionString &&
            Authority == other.Authority &&
            ClientId == other.ClientId &&
            ClientSecret == other.ClientSecret &&
            SignInScheme == other.SignInScheme &&
            SignOutScheme == other.SignOutScheme &&
            SaveTokens == other.SaveTokens &&
            AdminEmail == other.AdminEmail &&
            BillingEmail == other.BillingEmail &&
             (
                Scope == other.Scope ||
                Scope != null &&
                Scope.SequenceEqual(other.Scope)
            ) &&
            (
                Admins == other.Admins ||
                Admins != null &&
                Admins.SequenceEqual(other.Admins)
            ) &&
            (
                Registers == other.Registers ||
                Registers != null &&
                Registers.SequenceEqual(other.Registers)
            ) &&
            (
                Clients == other.Clients ||
                Clients != null &&
                Clients.SequenceEqual(other.Clients)
            );
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Tenant);
        }

        public override int GetHashCode()
        {
            var combinedHash = HashCode.Combine(
                Id,
                Name,
                ConnectionString,
                Authority,
                ClientId,
                ClientSecret,
                SignInScheme,
                Clients
                );

            return HashCode.Combine(
                combinedHash,
                SignOutScheme,
                SaveTokens,
                AdminEmail,
                BillingEmail,
                Scope,
                Admins,
                Registers
                );
        }
    }
}
