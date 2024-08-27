using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Siccar.Common;
using Siccar.Common.ServiceClients;
using Siccar.Common.Adaptors;
using Dapr.Client;
using ActionService.V1.Services;
using FluentValidation.AspNetCore;
using Serilog;
using ActionService.Hubs;
using System.Net.Http;
using Siccar.Application.Validation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Siccar.Common.Auth;
using System.Threading.Tasks;
using ActionService.V1.Adaptors;
using IdentityModel;
using FluentValidation;
using ActionService.V1.Repositories;
using System.Diagnostics;
#nullable enable

namespace ActionService
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddServiceProfiler();

            services.AddControllers().AddDapr();
			services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
			services.AddValidatorsFromAssemblyContaining<BlueprintValidator>();
			services.AddCors();
			services.AddProblemDetails(options =>
			{
				options.CustomizeProblemDetails = context => { context.ProblemDetails.Extensions["traceId"] = Activity.Current?.RootId ?? context.HttpContext.TraceIdentifier; };
			});
			AddServices(services);
			AddAuth(services);
			AddSwaggerGeneration(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler(env.IsDevelopment() ? "/error-local-development" : "/error");
			app.UseSwagger(c => c.SerializeAsV2 = true);
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/actions/swagger/v1/swagger.json", "ActionService v1"));
			app.UseCloudEvents();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseCors(x => x.AllowAnyMethod() .AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true)); // Allow any origin 
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapSubscribeHandler();
				endpoints.MapHub<ActionsHub>("actionshub");
				endpoints.MapControllers();
                endpoints.MapGet("/", (Func<string>)(() => "ok")).AllowAnonymous();
                endpoints.MapHealthChecks("/api/actions/healthz");
			});
		}

		private void AddServices(IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			services.AddTransient<ICalculationService, CalculationService>();
			services.AddTransient<IPayloadResolver, PayloadResolver>();
			services.AddTransient<IActionResolver, ActionResolver>();
			services.AddSingleton<IHubContextAdaptor, HubContextAdaptor>();
			services.AddSingleton<ISchemaDataValidator, SchemaDataValidator>();
			services.AddSingleton<IDaprClientAdaptor, DaprClientAdaptor>();
			services.AddSingleton<IFileRepository, FileRepository>();
			services.AddSingleton<ITransactionRequestBuilder, TransactionRequestBuilder>();
			var serviceProvider = services.BuildServiceProvider();
			services.AddSingleton<ISustainabilityHttpServiceClient, SustainabilityHttpServiceClient>(
			_ => new SustainabilityHttpServiceClient(Configuration, new HttpClientAdaptor(new HttpClient())));
			services.AddSingleton<IWalletServiceClient, WalletServiceClient>(
			_ => new WalletServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("wallet-service")), serviceProvider)));
			services.AddSingleton<IRegisterServiceClient, RegisterServiceClient>(
			_ => new RegisterServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("register-service")), serviceProvider)));
			services.AddSingleton<ITenantServiceClient, TenantServiceClient>(
			_ => new TenantServiceClient(new SiccarBaseClient(new HttpClientAdaptor(DaprClient.CreateInvokeHttpClient("tenant-service")), serviceProvider)));
			services.AddSingleton<IActionService, V1.Services.ActionService>();
			services.AddSignalR(c => { c.EnableDetailedErrors = true; });
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
			});
			services.AddHealthChecks();
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
				ValidateAudience = false,
				ValidIssuer = auth,
				ValidAudience = aud,
				RoleClaimType = JwtClaimTypes.Role
			};

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.Authority = auth;
				options.Audience = aud;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = validations;
				options.MapInboundClaims = false;

				//Register OnMessageRecieved to pass the access token from the signalRMessage to the authentication middleware.
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];
						var path = context.HttpContext.Request.Path;
						if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/actionshub")))
							context.Token = accessToken;
						return Task.CompletedTask;
					}
				};
			});
			services.AddAuthentication(AuthenticationDefaults.DaprAuthenticationScheme)
				.AddScheme<DaprAuthOptions, DaprAuthenticationHandler>(AuthenticationDefaults.DaprAuthenticationScheme,
			options => options.DaprSecret = Configuration.GetValue<string>("DaprSecret"));
			services.AddScoped<IAuthorizationHandler, NoTenantClaimAuthorizationHandler>();
			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder("Bearer").RequireAuthenticatedUser().RequireClaim("tenant").Build();
				options.AddPolicy("DaprClients", new AuthorizationPolicyBuilder(AuthenticationDefaults.DaprAuthenticationScheme).RequireAuthenticatedUser()
					.AddRequirements(new NoTenantClaimRequirement()).Build());
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
							Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,
						}, new List<string>()
					}
				});
			});
		 // services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
		}
	}
}
