// Program Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
#nullable enable

namespace Siccar.Registers.ValidatorService
{
	public class Program
	{
		public readonly static AssemblyName? assembly = Assembly.GetEntryAssembly()?.GetName();

		public static void Main(string[] args)
		{
			var builder = CreateHostBuilder(args);
			var conf = GetConfiguration();
			builder.ConfigureAppConfiguration(c => GetConfiguration());
			Log.Logger = CreateSerilogLogger(conf);
			
			try
			{
				builder.Build().Run();
			}
			catch (Exception er) { Log.Fatal("Fatal Exception : ", er.Message); }
			finally
			{
				Log.Information("[FINISHED] : {name}", assembly?.FullName );
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
			.UseSerilog().ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

		private static ILogger CreateSerilogLogger(IConfiguration configuration)
		{
			var loggerConfiguration = new LoggerConfiguration() .Enrich.WithProperty("AssemblyVersion", assembly?.Version?.ToString()!)
				.ReadFrom.Configuration(configuration);
#if DEBUG
			loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
			return loggerConfiguration.CreateLogger();
		}

		private static IConfiguration GetConfiguration()
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			var confbuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
				return confbuilder.Build(); }
		}
}
