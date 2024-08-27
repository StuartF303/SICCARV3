using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Siccar.Platform.Tenants.Repository;
using Siccar.Platform.Tenants.Core;
using System.Linq;
using System.Web;

namespace Siccar.Platform.Tenants.Authentication
{
    public class TenantSelectionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TenantSelectionProvider> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly string _defaultTenantId;

        public TenantSelectionProvider(IHttpContextAccessor httpContextAccessor, ILogger<TenantSelectionProvider> logger,
             ITenantRepository tenantRepository, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tenantRepository = tenantRepository;
            _defaultTenantId = string.IsNullOrWhiteSpace(configuration["DefaultTenant"]) ? Guid.Empty.ToString() : configuration["DefaultTenant"];
            _logger.LogInformation($"Default Tenant : {_defaultTenantId}");
        }


        /// <summary>
        /// Uses context to associate the incoming request with a Tenant
        /// </summary>
        /// <returns></returns>
        public string GetCurrentTenant()
        {
            var found = false;
            var tenantId = _defaultTenantId; // will return the default tenant id in worst case

            // So our strategy is a few fold, hopefully its just been passed to us
            // lets check the path and try that - this will be fast and efficient but 
            // requires changes to IDServer to understand the new auth endpoints
            if (_httpContextAccessor.HttpContext.Request.Path.ToString().Length > 19)
            {
                var pathparts = _httpContextAccessor.HttpContext.Request.Path.ToString().Split("/");
                if (pathparts.Length > 3) // should be 4 parts ''/'tenantid'/'connect'/'token'
                {
                    tenantId = pathparts[1];
                    found = true;
                }
            }

            // We can also do a lookup by Client ID - this might be more of a hit but 
            // it can be cached in momory by the repository provider

            var queryCtx = _httpContextAccessor.HttpContext.Request.Query;
            if (queryCtx.ContainsKey("client_id") && !found) // an inbound request
            {
                string clientId = queryCtx.Single(c => c.Key == "client_id").Value.ToString();

                var client = _tenantRepository.Single<Client>(c => c.ClientId == clientId).Result;
                if (client is null)
                {
                    _logger.LogWarning($"ClientID not found : {clientId}");
                    return null;
                }
                tenantId = client.TenantId;
                found = true;
            }
            if (queryCtx.ContainsKey("ReturnUrl") && !found) // a response redirect, we need to pluck it out of the querystring
            {
                var queryString = queryCtx.Single(c => c.Key.ToLower() == "returnurl").Value.ToString();
                var queryStringParts = queryString.Split("?");
                if (queryStringParts.Length > 1)
                {
                    queryString = queryStringParts[1];
                    var clientId = HttpUtility.ParseQueryString(queryString).Get("client_id");
                    var client = _tenantRepository.Single<Client>(c => c.ClientId == clientId).Result;
                    tenantId = client.TenantId;
                    found = true;
                }
            }

            _logger.LogDebug($"AuthN resolved tenant : {tenantId}");

            return tenantId;
        }
    }
}
