using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
#nullable enable

namespace WalletService.SQLRepository
{
    public class WalletContextFactory : IDesignTimeDbContextFactory<WalletContext>
    {
        private readonly IConfiguration Configuration;

        public WalletContextFactory()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (Configuration == null)
            {
                Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.migration.json")
                .AddEnvironmentVariables()
                .Build();
            }
            else {
                Console.WriteLine("Please set some configuration.");
            }
        }

        public WalletContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WalletContext>();
            var _connStr = Configuration["ConnectionStrings:WalletDB:ConnectionString"];

            Console.WriteLine("Connection String : {0}", _connStr);

            var _serverVersion = ServerVersion.AutoDetect(_connStr);
            builder.UseMySql(_connStr, _serverVersion);
            return new WalletContext(builder.Options, Configuration);
        }
    }
}
