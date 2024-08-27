using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Siccar.Application.Client.Extensions;
using siccarcmd.Tenant;
using siccarcmd.User;

namespace siccarcmd
{
    /// <summary>
    /// A CLI Tool to work with Siccar Services.
    /// 
    /// First pass of bringing various other tools together,
    /// will optimise over time
    /// </summary>
    class Program
    {
        private static IConfiguration configuration;

        static async Task<int> Main(string[] args)
        {

            // get config - but for the moment just use default CLI Providers
            configuration = SetupConfiguration();

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            var serilog = new LoggerConfiguration()
                .MinimumLevel.Error()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddHttpClient("siccarClient", s =>
                       s.BaseAddress = new Uri(configuration["SiccarService"]))
                        .AddPolicyHandler(retryPolicy);
                    services.AddSiccarSDKStateManagement(configuration); // gets added last

                })
                .UseSerilog()
                .UseConsoleLifetime();

            var host = builder.Build();
            var services = host.Services.CreateScope().ServiceProvider;
            var cli = BuildCommandLine(host);

            // Invoke 
            return await cli.InvokeAsync(args);
        }

        private static RootCommand BuildCommandLine(IHost host)
        {

            var services = host.Services;

            // Create a Root Command (which only runs if there are no subcommands) and add SubCommands
            var rootCommand = new RootCommand("siccarcmd", description: "Siccar Command Line Interface");
            rootCommand.AddGlobalOption(new Option<bool>(new string[] { "--debug", "-d" }, getDefaultValue: () => false, description: "Verbose output and logging"));

            // making heirarchical command structure
            rootCommand.AddCommand(new Command("auth", "information and actions for user authentication")
            {
                new Auth.LoginCommand("login", services),
                new Auth.InfoCommand("info", services),
                new Auth.LogoutCommand("logout", services),
                new Auth.DeviceAuthCommand("device", services),
                new Auth.CredentialsAuthCommand("credentials", services)
            });

            rootCommand.AddCommand(new Command("action", "interact with the user action service")
            {
                new Action.ActionEventCommand("event", services)
            });

            rootCommand.AddCommand(new Command("register", "interact with the installation register service")
            {
                new register.CreateCommand("create", services),
                new register.DeleteCommand("delete", services),
                new register.InfoCommand("info", services),
                //new register.TrafficCommand("traffic", services),
                new register.TransactionCommand("transaction", services),
            });

            rootCommand.AddCommand(new Command("wallet", "interact with the authenticated wallet")
            {
                new register.CreateCommand("create", services)
            });

            rootCommand.AddCommand(new Command("blueprint", "utilities for enacting blueprints")
            {
                new Blueprint.SchemaCommand("schema", services),
                new Blueprint.ValidateCommand("validate", services)
            });

            rootCommand.AddCommand(new Command("peer", "interact with the installation peer service")
            {
                new peer.InfoCommands("info", services),
                new peer.AddPeerCommand("add", services),
                new peer.ListCommand("list", services),
                new peer.HostRegisterCommand("host", services)
            });

            rootCommand.AddCommand(new Command("config", "check what your doing")
            {
                new Config.ListCommand("list", services),
                new Config.SetCommand("set", services)
            });

            rootCommand.AddCommand(new Command("init", "help setup the system")
            {
                new init.DAPRJWTCommand("encode", services)
            });

            rootCommand.AddCommand(new Command("user", "manage users")
            {
                new DeleteUserCommand("delete", services),
                new GetUserCommand("get", services),
                new UpdateRoleCommand("role", services)
            });

            rootCommand.AddCommand(new Command("participant", "Manage participants")
            {
                new PublishParticipantCommand("publish", services)
            });

            return rootCommand;
        }

        private static IConfiguration SetupConfiguration()
        {
            string homePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".siccar");
            string homeConfig = Path.Combine(homePath, "appsettings.json");
            var defaultConfig = new Dictionary<string, string>
            {
                {"SiccarService", "https://localhost:8443/"},
                {"clientId", "siccar-admin-ui-client"},
                {"clientSecret", "secret"},
                {"Scope", "installation.admin tenant.admin wallet.admin register.admin"}
            };

            // get config - but for the moment just use default CLI Providers
            configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(defaultConfig)
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile(homeConfig, optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

            return configuration;
        }
    }
}
