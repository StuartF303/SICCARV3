using BlueprintService.V1.Repositories;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Siccar.Application.Validation;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Common.Auth;
using Siccar.Common.ServiceClients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IdentityModel;
using FluentValidation;
using Siccar.Application;
using System.Diagnostics;
#nullable enable

namespace BlueprintService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            //IdentityModelEventSource.ShowPII = true; //Can remove when all Identity Stuff is working well
            services.AddControllers().AddDapr(config =>
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

            AddServices(services);
            AddAuth(services);
            AddSwaggerGeneration(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseExceptionHandler("/error-local-development");
            else
                app.UseExceptionHandler("/error");
            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/blueprints/swagger/v1/swagger.json", "BlueprintService v1");
            });
            app.UseCors();
            app.UseCloudEvents();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks($"{Constants.BlueprintAPIURL}/healthz");
            });
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
            services.AddSingleton<IBlueprintRepository, DaprBlueprintRepository>();
            services.AddValidatorsFromAssemblyContaining<BlueprintValidator>();
            services.AddScoped<IValidator<Blueprint>, BlueprintValidator>();
            services.AddHttpContextAccessor();
            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<IWalletServiceClient, WalletServiceClient>(
                 _ => new WalletServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("wallet-service")), serviceProvider)));
            services.AddSingleton<IRegisterServiceClient, RegisterServiceClient>(
                 _ => new RegisterServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("register-service")), serviceProvider)));
            services.AddSingleton<ITenantServiceClient, TenantServiceClient>(
                _ => new TenantServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("tenant-service")), serviceProvider)));
            services.AddHealthChecks();
        }

        public virtual void AddAuth(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();

            var auth = string.IsNullOrWhiteSpace(Configuration["TenantIssuer"]) ? "http://tenant:8080" : Configuration["TenantIssuer"];
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

        private static void AddSwaggerGeneration(IServiceCollection services)
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

            // services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        }
    }
}
