using System;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Siccar.Platform;

namespace WalletService.Configuration
{
    /// <summary>
    /// Multi Version Configuration of the OData Model builder for Wallet
    /// </summary>
    public class WalletModelConfiguration
    {
        /// <summary>
        /// NOT CURRENTLY in use 
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        /// <param name="routePrefix">Used to apply a given route prefix</param>
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder), "OData builder is Null");

            //builder.Namespace = "WalletService";

            var wallet = GetEdmModel();
        }

        /// <summary>
        /// Applies model configurations using the provided builder.
        /// </summary>
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Wallet>("Wallets")
                .EntityType.HasKey(w => w.Address)
                .HasMany<WalletTransaction>(wt => wt.Transactions)
                .Filter()
                .OrderBy()
                .Expand(5)
                .Page(250, 50)
                .Select();

            builder.EntitySet<Wallet>("Wallets")
                .EntityType.Property(e => e.Name).IsRequired();

            builder.EntitySet<Wallet>("Wallets")
                .HasManyBinding(wa => wa.Addresses, "Addresses");

            builder.EntitySet<Wallet>("Wallets")
                .HasManyBinding(wd => wd.Delegates, "Delegates");

            builder.EntitySet<Wallet>("Wallets")
                .HasManyBinding(wd => wd.Transactions, "Transactions");

            builder.EntitySet<WalletTransaction>("Transactions");

            builder.EntitySet<WalletAddress>("Addresses");

            builder.EntitySet<WalletAccess>("Delegates");

            return builder.GetEdmModel();
        }
    }
}
