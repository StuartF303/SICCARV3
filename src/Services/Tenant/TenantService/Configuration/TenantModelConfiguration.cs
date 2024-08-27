using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;
using Siccar.Application;
using System.Net.Sockets;

namespace TenantService.Configuration
{
    /// <summary>
    /// Multi Version Configuration of the OData Model builder for Wallet
    /// </summary>
    public static class TenantModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder.
        /// </summary>
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            //var tenant = builder.EntitySet<Tenant>("Tenants")
            //        .EntityType.HasKey(k => k.Id);

            return builder.GetEdmModel();
        }
    }
}
