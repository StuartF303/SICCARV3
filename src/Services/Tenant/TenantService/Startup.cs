using Dapr.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Serilog;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Siccar.Platform.Tenants.Authentication;
using Siccar.Platform.Tenants.Authorization;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Siccar.Common;
using Siccar.Common.Auth;
using TenantService.Authorization;
using TenantService.Configuration;
using Microsoft.AspNetCore.OData;
using System.Linq;
using Azure.Identity;
using System.Threading;
using WalletService.Core;
using static Serilog.Log;
using Microsoft.Extensions.Logging;
using Google.Api;
using System.Diagnostics;

namespace Siccar.Platform.Tenants
{
    public class Startup
    {
        public readonly Microsoft.Extensions.Logging.ILogger<IdentityServer4.Services.DefaultCorsPolicyService> logger;
        private readonly string dataProtectionKeyPath = "/app/data/keys/tenant";
        private string _keyVaultConnectionString;
        private ClientSecretCredential _keyVaultCredential;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddServiceProfiler();

            AddIDataProtector(services);

            // enable Odata Models and Controllet Views etc
            IEdmModel v1Model = TenantModelConfiguration.GetEdmModel();

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                .AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(1000)
                                    .AddRouteComponents("odata", TenantModelConfiguration.GetEdmModel())
                                    .AddRouteComponents("odata/participants({ParameterValue})", ParticipantModelConfiguration.GetEdmModel())
                         )
                .AddDapr();


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                   ForwardedHeaders.XForwardedFor |
                   ForwardedHeaders.XForwardedHost |
                   ForwardedHeaders.XForwardedProto;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier;
                };
            });

            ConfigureIdentityDB(services);
            ConfigureIdentityServer(services);
            AddServices(services);
            services.AddOidcStateDataFormatterCache();
            AddAuth(services);
            AddSwaggerGeneration(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // to parse and use the proxy info for building the discovery document
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                app.UseODataRouteDebug();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/tenants/swagger/v1/swagger.json", "Tenant v1"));

            // app.UseODataQueryRequest();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)); // Allow any origin 

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCloudEvents();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapSubscribeHandler();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks("/api/tenants/healthz");
            });

            MongoConfig.ConfigureMongoDriver2IgnoreExtraElements();
            var seeds = new SeedTenantServiceData(app, env, Log.Logger);
        }

        private void ConfigureIdentityDB(IServiceCollection services)
        {
            // Setup Persistent Storage
            var mongoDbServer = String.IsNullOrEmpty(Configuration["TenantRepository:MongoDBServer"]) ? "mongodb://127.0.0.1:27017" : Configuration["TenantRepository:MongoDBServer"];
            var mongoDBDatabase = String.IsNullOrEmpty(Configuration["TenantRepository:DatabaseName"]) ? "TenantService" : Configuration["TenantRepository:DatabaseName"];
            Log.Information("Storage Configured to use Server / Table : {0} / {1}", mongoDbServer, mongoDBDatabase);

            // To Enable Funbuckle Multitenancy pipeline processing enable the service below. 
            //services.AddMultiTenant<Tenant>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(op =>
            {
                op.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            })
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbServer, mongoDBDatabase
                );
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            // Identity Server SSL Configuration
            string issuer = Configuration["IdentityServer:IssuerName"] ?? "http://tenant";

            string defaultCertPath = string.IsNullOrEmpty(Configuration["Kestrel:Certificates:Default:Path"]) ? "default.pfx" : Configuration["Kestrel:Certificates:Default:Path"];
            string defaultCertPass = string.IsNullOrEmpty(Configuration["Kestrel:Certificates:Default:Password"]) ? "5iccar" : Configuration["Kestrel:Certificates:Default:Password"];

            X509Certificate2 x509;
            try
            {
                Log.Information("IdP Signing Cert : {0}", defaultCertPath);
                x509 = new X509Certificate2(File.ReadAllBytes(defaultCertPath), defaultCertPass);
                Log.Information("IdentityServer Siging Certificate Loaded");
            }
            catch (Exception)
            {
                throw new FileNotFoundException("Cannot load Identity Server Key Material : " + defaultCertPath);
            }


            var builder = services.AddIdentityServer(opt =>
            {
                opt.IssuerUri = issuer;

            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddMongoRepository()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddClients()
                .AddProfileService<TenantProfileService>()
                .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
                .AddSigningCredential(x509)
                .AddValidationKey(x509);
        }

        private void AddIDataProtector(IServiceCollection services)
        {
            //TODO: Once Github https://github.com/dapr/dapr/issues/3749 is resolved, implement dapr secret configuration
            // https://docs.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/secrets-management

            IDaprClientAdaptor daprClient;
            Debug("Getting Local Secret Store...");

            //using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            //{
            //    // Review the FormMain Singleton.
            //    daprClient = serviceProvider.GetRequiredService<IDaprClientAdaptor>();
            //}

            var daprHost = Configuration.GetSection("DaprHost").Value;
            var daprClientBuilder = new DaprClientBuilder();
            if (!string.IsNullOrWhiteSpace(daprHost))
                daprClientBuilder.UseGrpcEndpoint(daprHost);

            Thread.Sleep(500);
            daprClient = new DaprClientAdaptor(daprClientBuilder.Build(), new SerilogLoggerFactory(Logger).CreateLogger<DaprClientAdaptor>());

            var localSecretStore = daprClient.GetSecretsAsync(WalletConstants.SecretStore).GetAwaiter().GetResult();
            Debug("Configuring Data Protector : ");
            var clientId = localSecretStore[WalletConstants.siccarClientId];
            var clientSecret = localSecretStore[WalletConstants.siccarClientSecret];
            var tenantId = localSecretStore[WalletConstants.siccarTenant];
            _keyVaultConnectionString = localSecretStore[WalletConstants.AzureKeyVaultStringKey];
            var tenantSharedAccessSignature = localSecretStore[WalletConstants.TenantServiceKeySharedAccessSignature];
            Debug($"DP:  {clientId}, {clientSecret}, {_keyVaultConnectionString}, {tenantId}");

            _keyVaultCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var persistKeysToAzureBlobStorageString = Configuration.GetSection("PersistKeysToAzureBlobStorage").Value;
            if (bool.TryParse(persistKeysToAzureBlobStorageString, out var persistKeysToAzureBlobStorage) && persistKeysToAzureBlobStorage)
            {
                if (string.IsNullOrWhiteSpace(tenantSharedAccessSignature))
                {
                    throw new ArgumentException(
                        "PersistKeysToAzureBlobStorage has been specified, but no shared wallet access signature is set");
                }

                Debug("Configuring data protection to use azure blob storage");
                services.AddDataProtection()
                    .SetApplicationName("tenant-service")
                    .PersistKeysToAzureBlobStorage(tenantSharedAccessSignature, "tenant-service", "keys.xml")
                    .ProtectKeysWithAzureKeyVault(new Uri(_keyVaultConnectionString), _keyVaultCredential);
            }
            else
            {
                // Setup defaults for DataProtectorKeyStorage
                DirectoryInfo dataProtectorDi = new(dataProtectionKeyPath);
                if (!dataProtectorDi.Exists)
                {
                    dataProtectorDi.Create();
                    Information("DataProtector Key Directory Created {0}.", dataProtectorDi.FullName);
                }

                Debug("Configuring data protection to use local disk storage");
                services.AddDataProtection()
                    .SetApplicationName("tenant-service")
                    .PersistKeysToFileSystem(dataProtectorDi)
                    .ProtectKeysWithAzureKeyVault(new Uri(_keyVaultConnectionString), _keyVaultCredential);
            }

        }

        private void AddServices(IServiceCollection services)
        {
            services.AddSingleton<ITenantRepository, TenantMongoRepository>();
            services.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
            services.AddSingleton<IAuthClaimsFactory, AuthClaimsFactory>();

            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<IWalletServiceClient, WalletServiceClient>(
                 _ => new WalletServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("wallet-service")), serviceProvider)));

            services.AddSingleton<IRegisterServiceClient, RegisterServiceClient>(
                _ => new RegisterServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("register-service")), serviceProvider)));
            services.AddHealthChecks();
        // temp removing this check as is not working prolly needs auth        .AddMongoDb(Configuration["TenantRepository:MongoDBServer"]);
        }

        public virtual void AddAuth(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();

            // as long as the AddService was correctly added first...
            services.AddAuthentication()
                .AddLocalApi(o =>
                {
                    o.ExpectedScope = null;
                })
                .AddOpenIdConnect();

            services.AddAuthentication(AuthenticationDefaults.DaprAuthenticationScheme)
                .AddScheme<DaprAuthOptions, DaprAuthenticationHandler>(AuthenticationDefaults.DaprAuthenticationScheme
                    , options => options.DaprSecret = Configuration.GetValue<string>("DaprSecret"));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy("tenants-management", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "tenant.admin");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DaprClients",
                    new AuthorizationPolicyBuilder(AuthenticationDefaults.DaprAuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new NoTenantClaimRequirement())
                        .Build());
            });

            // this handles Tenant resolution, chaching and configuring OpenIDConnect parameters
            services.AddSingleton<TenantSelectionProvider>();
            services.AddSingleton<IOptionsMonitor<OpenIdConnectOptions>, OpenIdConnectOptionsProvider>();
            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsInitializer>();
        }

        private static void AddSwaggerGeneration(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.EnableAnnotations();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlFile);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJSU...'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
            });

            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        }
    }
}
