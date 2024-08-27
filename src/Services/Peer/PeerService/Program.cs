using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog; // Using serilog as the Base ILogger
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Siccar.Network.PeerService
{
    public class Program
    {
        public readonly static Version AssemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var conf = GetConfiguration();
            builder.ConfigureAppConfiguration(c => GetConfiguration());
            Log.Logger = CreateSerilogLogger(conf);
            builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithProperty("AssemblyVersion", AssemblyVersion.ToString())
                .ReadFrom.Configuration(configuration);

#if DEBUG
            // Used to filter out potentially bad data due to debugging.
            // Very useful when doing Seq dashboards and want to remove logs under debugging session.
            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif

            return loggerConfiguration.CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var confbuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return confbuilder.Build();
        }
    }
}
