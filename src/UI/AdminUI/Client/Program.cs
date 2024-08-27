using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using System.Threading.Tasks;
using IdentityModel;
using Siccar.Application.Client.Extensions;
using Siccar.UI.Admin.Adaptors;
using FluentValidation;
using Siccar.Application.Validation;
using Siccar.Application;
using Siccar.UI.Admin.Services;
using Blazored.LocalStorage;

namespace Siccar.UI.Admin
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZiWH1ecXZXRmBVVkJ+XQ==");

			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			var config = builder.Configuration;
			builder.Services.AddSingleton<IConfiguration, WebAssemblyHostConfiguration>();
			builder.Services.AddScoped<TransactionAdaptorOData>();
			builder.Services.AddScoped<ParticipantAdaptorOData>();
			builder.Services.AddSingleton<PageHistoryState>();
			builder.RootComponents.Add<App>("#app");
			builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
			builder.Services.AddSyncfusionBlazor();
			builder.Services.AddAuthorizationCore();
			builder.Services.AddScoped<IValidator<Blueprint>, BlueprintValidator>();

			string siccarService = builder.Configuration["SiccarService"];

			builder.Services.AddOidcAuthentication(opt =>
			{
				opt.ProviderOptions.Authority = siccarService;
				opt.ProviderOptions.ClientId = "siccar-admin-ui-client";
				opt.ProviderOptions.DefaultScopes.Clear();
				opt.ProviderOptions.DefaultScopes.Add("installation.admin");
				opt.ProviderOptions.ResponseType = "code";
				opt.ProviderOptions.RedirectUri = "authentication/login-callback";
				opt.ProviderOptions.PostLogoutRedirectUri = "authentication/logout-callback";
				opt.ProviderOptions.DefaultScopes.Add("openid");
				opt.ProviderOptions.DefaultScopes.Add("profile");
				opt.UserOptions.RoleClaim = JwtClaimTypes.Role;
			}).AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>(); 

			builder.Services.AddHttpContextAccessor();
			builder.Services.AddBlazoredLocalStorage();
			builder.Services.AddSiccarSDKStateManagement(config);
			await builder.Build().RunAsync();
		}
	}
}