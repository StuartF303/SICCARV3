using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Siccar.Platform.Tenants.Repository;
using System;
using System.Diagnostics;

namespace Siccar.Platform.Tenants.Authentication
{
    /// <summary>
    /// 
    /// </summary>

    public class OpenIdConnectOptionsInitializer : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly TenantSelectionProvider _tenantAuthenticationProvider;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<OpenIdConnectOptionsInitializer> _logger;

        public OpenIdConnectOptionsInitializer(
            IDataProtectionProvider dataProtectionProvider,
            TenantSelectionProvider tenantProvider,
            ITenantRepository tenantRepository,
            ILogger<OpenIdConnectOptionsInitializer> logger)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _tenantAuthenticationProvider = tenantProvider;
            _tenantRepository = tenantRepository;
            _logger = logger;
        }

        /// <summary>
        /// Configure the OIDC Auth Record using a DB lookup.
        /// </summary>
        /// <param name="name">Name of the OIDC Provider</param>
        /// <param name="options"></param>
        public void Configure(string name, OpenIdConnectOptions options)
        {
            if (!string.Equals(name, OpenIdConnectDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return; // in preflight the request is for OpenIdConnect rather than a specific tenant
            }

            var tenantId = _tenantAuthenticationProvider.GetCurrentTenant();

            var tenant = _tenantRepository.Single<Tenant>(t => t.Id == tenantId).Result;

            if (tenant != null)
            {

                // Create a tenant-specific data protection provider to ensure
                // encrypted states can't be read/decrypted by the other tenants.
                options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenant.Id); // not sure this is so relavent in our architecture

                // Other tenant-specific options like options.Authority can be registered here.
                options.SignInScheme = tenant.SignInScheme;
                options.SignOutScheme = tenant.SignOutScheme;
                options.SaveTokens = tenant.SaveTokens;
                options.Authority = tenant.Authority;
                options.ClientId = tenant.ClientId;
                options.CallbackPath = tenant.CallbackPath;
                options.RemoteSignOutPath = tenant.RemoteSignOutPath;
                options.ResponseType = tenant.ResponseType;
                options.TokenValidationParameters = tenant.TokenValidationParameters;
            }
            else
            {
                // lets log a warning and return a default constructor
                // this gets hit when we have no configured Tenants i.e. pre boot
                _logger.LogWarning($"AuthN no configured tenant : {tenantId}");
                options.Authority = "https://localhost";
                options.ClientId = "default_client";
            }
        }

        public void Configure(OpenIdConnectOptions options)
            => Debug.Fail("This infrastructure method shouldn't be called.");
    }
}