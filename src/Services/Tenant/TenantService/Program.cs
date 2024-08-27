// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Siccar.Platform.Tenants
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.

            await Task.Run(() =>
            {
                var config = GetConfiguration();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(GetConfiguration())
                    .CreateLogger();
                Log.Information($"Starting up, with process id: {Environment.ProcessId}");
            });

            try
            {
                var webhost = CreateHostBuilder(args).ConfigureAppConfiguration(_ => GetConfiguration()).Build();
                webhost.Run();
                Log.Information("Stopped cleanly");
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
                return 1;
            }
            finally
            {
                Log.Information("Service has shutdown.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Release"}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
