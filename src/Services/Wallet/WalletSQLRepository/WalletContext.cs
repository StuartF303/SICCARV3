using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siccar.Platform;
#nullable enable

namespace WalletService.SQLRepository
{
	public class WalletContext : DbContext
	{
		public DbSet<Wallet>? Wallets { get; set; }
		public DbSet<WalletAddress>? Addresses { get; set; }
		public DbSet<WalletAccess>? Delegates { get; set; }
		public DbSet<WalletTransaction>? Transactions { get; set; }
		public DbSet<TransactionMetaData>? TransactionMetaData { get; set; }

		private readonly ILogger<WalletContext>? _logger;
		private readonly IConfiguration? _configuration;
		private readonly string connectionStr = "";
		private readonly string connectionType = "";
		private readonly ServerVersion? _serverVersion;
		private readonly string? cosmosEndpoint = "";
		private readonly string? cosmosAccountKey = "";
		private readonly string dbName = "wallets";

		public WalletContext(DbContextOptions<WalletContext> options) : base(options) {}

		/// <summary>
		/// The main SQL Repository entry point
		/// </summary>
		/// <param name="options"></param>
		/// <param name="configuration"></param>
		/// <param name="log"></param>
		public WalletContext(DbContextOptions<WalletContext> options, IConfiguration configuration, ILogger<WalletContext>? log = null) : base(options)
		{
			_logger = log;
			_configuration = configuration;
			connectionStr = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:WalletDB:ConnectionString"])
				? "server=mysql;database=wallets;user=root;password=5iccar-dev-secret"
				: _configuration["ConnectionStrings:WalletDB:ConnectionString"]!;
			connectionType = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:WalletDB:ProviderType"])
				? "mysql" : _configuration["ConnectionStrings:WalletDB:ProviderType"]!.ToLower();

			switch (connectionType)
			{
				case "mysql":
					_serverVersion = ServerVersion.AutoDetect(connectionStr);
					_logger?.LogDebug("DB Type : {type} ; Connection : {connect}", connectionType, connectionStr);
					break;
				case "cosmossql":
					var keyValues = connectionStr.Split(';').Select(s => s.Split('=', 2, StringSplitOptions.None)).ToLookup(kv => kv[0], kv => kv.Last());
					cosmosEndpoint = keyValues["AccountEndpoint"]?.FirstOrDefault();
					cosmosAccountKey = keyValues["AccountKey"]?.FirstOrDefault();
					if (string.IsNullOrEmpty(cosmosEndpoint) || string.IsNullOrEmpty(cosmosAccountKey))
						throw new ArgumentException("Cosmos Configuration Incorrect!");
					break;
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// to support multiple providers

			switch (connectionType)
			{
				case "mysql":
					optionsBuilder.UseMySql(connectionStr, _serverVersion);
					break;
				case "cosmossql":
					optionsBuilder.UseCosmos(cosmosEndpoint!, cosmosAccountKey!, dbName);
					break;
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// would rather specify these on the model than explicitly but this is the SQL Code First
			modelBuilder.Entity<Wallet>(entity =>
			{
				entity.HasKey(e => e.Address);
				entity.Property(e => e.Name).IsRequired();
				entity.HasIndex(e => e.Owner);
			});

			modelBuilder.Entity<WalletAddress>(entity =>
			{
				entity.HasKey(e => e.Address);
				entity.Property(e => e.DerivationPath).IsRequired();
				entity.HasOne<Wallet>().WithMany(e => e.Addresses).HasForeignKey(wa => wa.WalletId).OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<WalletTransaction>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasOne<Wallet>().WithMany(e => e.Transactions).HasForeignKey(wa => wa.WalletId).OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<TransactionMetaData>(entity =>
			{
				entity.Ignore("TrackingData");
				entity.Ignore("_trackingData");
				entity.Property<string>("_trackingDataJson");
			});

			modelBuilder.Entity<WalletAccess>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Subject);
				entity.Property(e => e.AssignedTime).IsETagConcurrency();
				entity.HasOne<Wallet>().WithMany(e => e.Delegates).HasForeignKey(wa => wa.WalletId).OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
