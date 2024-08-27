using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Siccar.Platform;
#nullable enable

namespace Siccar.Registers.RegisterService.Configuration
{
    /// <summary>
    /// Multi Version Configuration of the OData Model builder for Wallet
    /// </summary>
    public static class OdataModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder.
        /// </summary>
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var dockets = builder.EntitySet<Docket>("Dockets")
                    .EntityType.HasKey(d => d.Id)
                    .Filter()
                    .OrderBy()
                    .Expand(5)
                    .Page(250, 50)
                    .Select();

            dockets.HasMany<TransactionModel>(t => t.Transactions)
                .IsNavigable();

            var register = builder.EntitySet<Register>("Registers")
                    .EntityType.HasKey(r => r.Id)
                    .Filter()
                    .OrderBy()
                    .Expand(5)
                    .Page(250, 50)
                    .Select();
            register.HasMany<Docket>(d => d.Dockets)
                .IsNavigable();
            register.HasMany<TransactionModel>(t => t.Transactions)
                .IsNavigable();

            var transactions = builder.EntitySet<TransactionModel>("Transactions")
                .EntityType.HasKey(t => t.TxId)
                .Filter()
                .OrderBy()
                .Expand(5)
                .Page(250, 1000)
                .Select();

            return builder.GetEdmModel();
        }
    }
}
