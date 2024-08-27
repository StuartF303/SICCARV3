using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Siccar.Network.Peers.Core;
using Siccar.Network.Router;
using Siccar.Network.Router.ConnectionHandlers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Siccar.Common.Auth;
using System.Diagnostics;

namespace Siccar.Network.PeerService
{
    public class Startup
    {
        private static CancellationTokenSource source;
       

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // Define the cancellation token.
            source = new CancellationTokenSource();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            var jopts = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            services.AddControllers()
                .AddJsonOptions( options => {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                .AddDapr();
            
            AddAuth(services);
            
            services.AddHttpClient();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Siccar.Network.PeerService",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Siccar",
                        Email = "info@siccar.net",
                        Url = new Uri("https://www.siccar.net/"),
                    },
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlFile);
            });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier;
                };
            });


            services.AddSingleton<IPeerRouter, DefaultRouter>(); // the router maintains the connection table
            services.AddFactory<ITransportConnectionHandler, WebAPIConnectionHandler>(); // Create a factory for handlers into the di pipline

            // And SignalR for P2P  
            bool messageLogging = bool.Parse(Configuration["Transport:Logging"] ?? "false");

            services.AddSignalR(c => { c.EnableDetailedErrors = messageLogging; })
               .AddHubOptions<Router.PeerHub>(cf => { cf.EnableDetailedErrors = messageLogging; });    // For comms with other peers

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            }); // for SignalR Binarymode
            services.AddHealthChecks();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env, IPeerRouter peerrouter)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseCors(builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            }

            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/peer/swagger/v1/swagger.json", "PeerService v1"));

            peerrouter.InitialisePeer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Network.Router.PeerHub>(PeerConstants.PeerHubUri);
              //  endpoints.MapHub<Hubs.AdminHub>("AdminHub");
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks("/api/peer/healthz");
            });

            applicationLifetime.ApplicationStopping.Register(() => OnShutdown(peerrouter));
        }

        public virtual void AddAuth(IServiceCollection services)
        {
            var auth = string.IsNullOrWhiteSpace(Configuration["TenantIssuer"]) ? "https://tenant:8080" : Configuration["TenantIssuer"];
            var aud = string.IsNullOrWhiteSpace(Configuration["SiccarAudience"]) ? "siccar.dev" : Configuration["SiccarAudience"];
            Log.Debug("Using Token Authority : '" + auth + "'");

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false, // we dont use aud
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

                    //Register OnMessageRecieved to pass the access token from the signalRMessage to the authentication middleware.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/actionshub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            
            services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim("tenant")
                    .Build();

            });
        }

        public void OnShutdown(IPeerRouter peerrouter)
        {
            peerrouter.Shutdown();
            source.Cancel();

            Thread.Sleep(250);
            Log.Information("Shutting down... ");
            Log.CloseAndFlush();
        }
    }
}
