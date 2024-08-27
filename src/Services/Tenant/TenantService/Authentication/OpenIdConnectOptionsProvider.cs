using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Siccar.Platform.Tenants.Repository;
using System;
using System.Collections.Concurrent;

namespace Siccar.Platform.Tenants.Authentication
{
    public class OpenIdConnectOptionsProvider : IOptionsMonitor<OpenIdConnectOptions>
    {
        private readonly ConcurrentDictionary<(string name, string tenant), Lazy<OpenIdConnectOptions>> _cache;
        private readonly IOptionsFactory<OpenIdConnectOptions> _optionsFactory;
        private readonly TenantSelectionProvider _tenantAuthenticationProvider;

        public OpenIdConnectOptionsProvider(
            IOptionsFactory<OpenIdConnectOptions> optionsFactory,
            TenantSelectionProvider tenantProvider)
        {
            _cache = new ConcurrentDictionary<(string, string), Lazy<OpenIdConnectOptions>>();
            _optionsFactory = optionsFactory;
            _tenantAuthenticationProvider = tenantProvider;
        }

        public OpenIdConnectOptions CurrentValue => Get(Options.DefaultName);

        public OpenIdConnectOptions Get(string name)
        {
            var tenant = _tenantAuthenticationProvider.GetCurrentTenant();

            Lazy<OpenIdConnectOptions> Create() => new Lazy<OpenIdConnectOptions>(() => _optionsFactory.Create(name));
            return _cache.GetOrAdd((name, tenant), _ => Create()).Value;
        }

        public IDisposable OnChange(Action<OpenIdConnectOptions, string> listener) => null;
    }
}
