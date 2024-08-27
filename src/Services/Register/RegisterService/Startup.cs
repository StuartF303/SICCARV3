using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Serilog;
using Siccar.Common;
using Siccar.Common.Auth;
using Siccar.Registers.Core;
using Siccar.Registers.Core.MongoDBStorage;
using Siccar.Registers.RegisterService.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OpenApi.Models;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Dapr.Client;
using Siccar.Registers.RegisterService.V1.Services;
using System.Threading.Tasks;
using Siccar.Registers.RegisterService.Hubs;
using Siccar.Registers.RegisterService.V1.Adapters;

namespace Siccar.Registers.RegisterService
{
    public class Startup
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Local Configuration Provider
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            IdentityModelEventSource.ShowPII = false; //Can remove when all Idenity Stuff is working well      

            AddServices(services);

            IEdmModel v1Model = OdataModelConfiguration.GetEdmModel();

            services.AddControllers()
                .AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(1000)
                .AddRouteComponents("odata/registers({registerId})", v1Model)
                )
                .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
               })
                .AddDapr();

            services.AddCors();

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier;
                };
            });

            AddAuth(services);
            AddSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => c.SerializeAsV2 = true);
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/registers/swagger/v1/swagger.json", "Siccar.Registers.RegisterService v1");
                });
                app.UseCors(builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
                app.UseODataRouteDebug();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();

            ConfigureAuth(app);

            //app.UseAuthentication(); TODO: Alan restore ??
            //app.UseAuthorization();

            app.UseCloudEvents();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.EnableDependencyInjection();  // Work-around for #1175
                //endpoints.Select().Expand().OrderBy().Count().Filter().MaxTop(null);
                //endpoints.MapVersionedODataRoute("odata", "odata/registers", modelBuilder.GetEdmModels());
                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();
                endpoints.MapHub<RegistersHub>("registershub");
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks($"{Constants.RegisterAPIURL}/healthz");
            });
            applicationLifetime.ApplicationStopping.Register(() => OnShutdown());
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IRegisterRepository>(rr =>
                new RegisterRepository(rr.GetService<ILogger<RegisterRepository>>(),
                            Configuration));
            services.AddSingleton<IHubContextAdapter, RegisterServiceHubContextAdapter>();
            services.AddHttpContextAccessor();
            var serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
            services.AddSingleton<IRegisterResolver, RegisterResolver>();
            services.AddSingleton<ITenantServiceClient, TenantServiceClient>(
                 _ => new TenantServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("tenant-service")), serviceProvider)));
            services.AddHealthChecks();
            // move to check the context >>>    .AddMongoDb(Configuration["RegisterRepository:MongoDBServer"], timeout: TimeSpan.FromSeconds(5));
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public void OnShutdown()
        {

            Thread.Sleep(250);
            Log.Information("Shutting down... ");
            Log.CloseAndFlush();

        }

        public virtual void AddAuth(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();

            var auth = string.IsNullOrWhiteSpace(Configuration["TenantIssuer"]) ? "https://tenant:8080" : Configuration["TenantIssuer"];
            var aud = string.IsNullOrWhiteSpace(Configuration["SiccarAudience"]) ? "siccar.dev" : Configuration["SiccarAudience"];
            Log.Debug("Using Token Authority : '" + auth + "'");

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

                    //Register OnMessageReceived to pass the access token from the signalRMessage to the authentication middleware.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/registershub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthentication(AuthenticationDefaults.DaprAuthenticationScheme)
                .AddScheme<DaprAuthOptions, DaprAuthenticationHandler>(AuthenticationDefaults.DaprAuthenticationScheme
                , options => options.DaprSecret = Configuration.GetValue<string>("DaprSecret"));

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim("tenant")
                    .Build();

                options.AddPolicy("DaprClients",
                    new AuthorizationPolicyBuilder(AuthenticationDefaults.DaprAuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new NoTenantClaimRequirement())
                        .Build());
            });

            services.AddSignalR(c => { c.EnableDetailedErrors = true; });
        }

        private static void AddSwagger(IServiceCollection services)
        {
            /*
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            */
            services.AddSwaggerGen(c =>
            {
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
