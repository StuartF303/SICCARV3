using Azure.Identity;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Abstracts;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Serilog.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Common.Auth;
using Siccar.Common.ServiceClients;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using IdentityModel;
using Microsoft.AspNetCore.HttpOverrides;
using WalletService.Configuration;
using WalletService.Core;
using WalletService.Core.Interfaces;
using WalletService.Core.Repositories;
using WalletService.SQLRepository;
using WalletService.V1.Services;
using static Serilog.Log;
using System.Diagnostics;
using Google.Api;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Asp.Versioning;

namespace WalletService
{
	public class Startup
	{
		private readonly string dataProtectionKeyPath = "/app/data/keys/wallet";
        private string _keyVaultConnectionString;
        private ClientSecretCredential _keyVaultCredential;

        public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public virtual void ConfigureServices(IServiceCollection services)
		{
			Information("Starting WalletService");
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            IdentityModelEventSource.ShowPII = true; //Can remove when all Identity Stuff is working well
			AddIDataProtector(services);
			AddServices(services);
		

            IEdmModel v1Model = WalletModelConfiguration.GetEdmModel();
            services.AddDbContext<WalletContext>();
            //services.AddODataQueryFilter();

            services.AddControllers()
                //.AddOData(opt =>
                //{
                //    opt.AddRouteComponents("api", v1Model);
                //    opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(15);
                //})
                .AddJsonOptions(jc =>
                {
                    jc.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    jc.JsonSerializerOptions.WriteIndented = true;
                    jc.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddDapr(config =>
                {
                    var daprHost = Configuration.GetSection("DaprHost").Value;
                    if (!string.IsNullOrWhiteSpace(daprHost))
                    {
                        config.UseGrpcEndpoint(daprHost);
                    }
                });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier;
                };
            });

            AddAuth(services);
            AddSwaggerGeneration(services);
			services.AddCors();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WalletContext context) //  VersionedODataModelBuilder modelBuilder,
        {
            if (env.IsDevelopment())
            {
                //app.UseODataRouteDebug();
                app.UseExceptionHandler("/error-local-development");
                app.UseSwagger(c => c.SerializeAsV2 = true);
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/wallets/swagger/v1/swagger.json", "Wallet v1"));
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

			app.Use((context, next) =>
			{
				context.Features.Set<IODataFeature>(null);
				return next();
			});

            try
            {
                context.Database.Migrate(); // Ensure the DB is uptodate
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Error(ex.Message);
                throw new Exception("Database Migration Errror");
            }
			app.UseRouting();
			ConfigureAuth(app);
			app.UseCloudEvents();

			app.UseCors(builder =>
			{
				builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapSubscribeHandler();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks("/api/wallets/healthz");
            });
		}

		protected virtual void ConfigureAuth(IApplicationBuilder app)
		{
			app.UseAuthentication();
			app.UseAuthorization();
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
			var walletSharedAccessSignature = localSecretStore[WalletConstants.WalletServiceKeySharedAccessSignature];
            Debug($"DP:  {clientId}, {clientSecret}, {_keyVaultConnectionString}, {tenantId}");
			
            _keyVaultCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var persistKeysToAzureBlobStorageString = Configuration.GetSection("PersistKeysToAzureBlobStorage").Value;
            if (bool.TryParse(persistKeysToAzureBlobStorageString, out var persistKeysToAzureBlobStorage) && persistKeysToAzureBlobStorage)
            {
                if (string.IsNullOrWhiteSpace(walletSharedAccessSignature))
                {
                    throw new ArgumentException(
                        "PersistKeysToAzureBlobStorage has been specified, but no shared wallet access signature is set");
                }

                Debug("Configuring data protection to use azure blob storage");
                services.AddDataProtection()
                    .SetApplicationName("wallet-service")
                    .PersistKeysToAzureBlobStorage(walletSharedAccessSignature, "wallet-service", "keys.xml")
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
                    .SetApplicationName("wallet-service")
                    .PersistKeysToFileSystem(dataProtectorDi)
                    .ProtectKeysWithAzureKeyVault(new Uri(_keyVaultConnectionString), _keyVaultCredential);
            }

        }
		 
        public virtual void AddAuth(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();

			var auth = string.IsNullOrWhiteSpace(Configuration["TenantIssuer"]) ? "https://tenant:8080" : Configuration["TenantIssuer"];
			var aud = string.IsNullOrWhiteSpace(Configuration["SiccarAudience"]) ? "siccar.dev" : Configuration["SiccarAudience"];
			Debug("Using Token Authority : '" + auth + "'");

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                // Add these...
                ValidIssuer = auth,
                ValidAudience = aud,
                RoleClaimType = JwtClaimTypes.Role
            };

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = auth;
					options.Audience = aud;
					options.RequireHttpsMetadata = false; // remove before production
					options.TokenValidationParameters = validations;
					options.MapInboundClaims = false; // just pass them
				});

			services.AddAuthentication(AuthenticationDefaults.DaprAuthenticationScheme)
				.AddScheme<DaprAuthOptions, DaprAuthenticationHandler>(AuthenticationDefaults.DaprAuthenticationScheme,
					options => options.DaprSecret = Configuration.GetValue<string>("DaprSecret"));

			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim("tenant")
					.Build();
				options.AddPolicy("DaprClients", new AuthorizationPolicyBuilder(AuthenticationDefaults.DaprAuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .AddRequirements(new NoTenantClaimRequirement()).Build());
			});
		}

        private static void AddSwaggerGeneration(IServiceCollection services)
        {
            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            //// Workaround: https://github.com/OData/WebApi/issues/1177
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
            //.AddApiExplorer();

			services.AddSwaggerGen(c =>
			{
				c.EnableAnnotations();
				c.OperationFilter<SwaggerDefaultValues>();
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

		private void AddServices(IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				// allow multiple locations to request an api version
				options.ApiVersionReader = ApiVersionReader.Combine(
				//    new QueryStringApiVersionReader(),
					new HeaderApiVersionReader("api-version"));
			});

            services.AddRouting();

            services.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddSingleton<IWalletProtector, WalletProtector>();
			services.AddSingleton<IWalletFactory, WalletFactory>();

		//	services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");

            services.AddHttpContextAccessor();
            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<IRegisterServiceClient, RegisterServiceClient>(
                 _ => new RegisterServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("register-service")), serviceProvider)));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedHost |
                    ForwardedHeaders.XForwardedProto;
            });

            var keyVaultUri = new Uri(_keyVaultConnectionString);
            services.AddHealthChecks()
                .AddMySql(Configuration["ConnectionStrings:WalletDB:ConnectionString"]);
                //.AddAzureKeyVault(new Uri($"{keyVaultUri.Scheme}://{keyVaultUri.Host}"), _keyVaultCredential, opts =>
                //{
                //    opts.AddKey("SiccarV3EncryptionKey");
                //}, failureStatus: HealthStatus.Degraded, timeout: TimeSpan.FromSeconds(5) );
        }
    }
}
