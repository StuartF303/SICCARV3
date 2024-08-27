// Statup Service Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Serilog;
using Siccar.Common;
using Siccar.Common.ServiceClients;
using Siccar.Common.Adaptors;
using Siccar.Registers.ValidatorCore;
using Siccar.Registers.ValidationEngine;
using System.Diagnostics;
using Dapr.Client;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Siccar.Common.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using System;
#nullable enable

namespace Siccar.Registers.ValidatorService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) { Configuration = configuration; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            AddServices(services);

            AddAuth(services);
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier;
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
            app.UseRouting();
            ConfigureAuth(app);
            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks($"{Constants.ValidatorAPIURL}/healthz");
            });

            applicationLifetime.ApplicationStopping.Register(() => OnShutdown());
        }

        private static void AddServices(IServiceCollection collection)
        {
            var serviceProvider = collection.BuildServiceProvider();
            collection.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
            collection.AddControllers().AddDapr();
            collection.AddHealthChecks();
            collection.AddSingleton<IRegisterServiceClient, RegisterServiceClient>(
                _ => new RegisterServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("register-service")), serviceProvider)));
            collection.AddSingleton<ISiccarValidator, RulesBasedValidator>();
            collection.AddSingleton<IMemPool, MemPool>();
            collection.AddTransient<DocketBuilder>();
            collection.AddTransient<Genesys>();
            collection.AddHostedService<ISiccarValidator>(sp => sp.GetService<ISiccarValidator>()!);
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
    }
}
