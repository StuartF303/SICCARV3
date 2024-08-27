using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Serilog;
using Siccar.Platform.Tenants.Core;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Client = Siccar.Platform.Tenants.Core.Client;

namespace Siccar.Platform.Tenants.Repository
{
	public class SeedTenantServiceData : IDisposable
	{
		private readonly JsonSerializerOptions jopts = new(JsonSerializerDefaults.Web);

		private readonly ITenantRepository _tenantRepository;
		private IEnumerable<Tenant> _tenants = new List<Tenant>();
		private IEnumerable<Client> _clients = new List<Client>();
		private IEnumerable<string> _roles = new List<string>();
		private IEnumerable<IdentityResource> _resources = new List<IdentityResource>();
		private IEnumerable<ApiScope> _scopes = new List<ApiScope>();
		private IEnumerable<ApiResource> _apiresources = new List<ApiResource>();
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ILogger _log;
		private readonly bool _development;

		public SeedTenantServiceData(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
		{
			_log = logger;
			_development = env.IsDevelopment();

			_roleManager = (RoleManager<ApplicationRole>)app.ApplicationServices.CreateScope().ServiceProvider
				.GetService(typeof(RoleManager<ApplicationRole>))!;

			// Make sure we have all our basic storage online upfront
			_log.Information("Checking Tenant Repository.");

			_tenantRepository = (ITenantRepository)app.ApplicationServices.GetService(typeof(ITenantRepository))!;
			if (_tenantRepository == null)
				throw new Exception("Tenant Repository Not Configured");

			// Set the default lists
			var seedList = new List<string>() { "seed.json" };
			if (_development)
				seedList.Add("seed.development.json");

			foreach (var seedfileName in seedList)
			{
				try
				{
					using StreamReader r = new("deploy/" + seedfileName);
					_log.Information($"Importing configuration from : {seedfileName}");
					string json = r.ReadToEnd();
					var items = JsonDocument.Parse(json);
					ProcessItems(items);
				}
				catch (Exception)
				{
					Log.Error($"Seed file {seedfileName} NOT Found.");
				}
			}
			InitializeDatabase();
		}

		private void ProcessItems(JsonDocument items)
		{
			// each of these will throw Key Not Found if they dont exist 
			try
			{
				var propApi = items.RootElement.GetProperty("apiresources");
				_apiresources = JsonSerializer.Deserialize<List<ApiResource>>(propApi, jopts) ?? new List<ApiResource>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug($"No APIResources found.");
			}
			catch (Exception err) { _log.Error($"Error processing APIResources : {err.Message}"); }

			try
			{
				var propId = items.RootElement.GetProperty("identityresources");
				_resources = JsonSerializer.Deserialize<List<IdentityResource>>(propId, jopts) ?? new List<IdentityResource>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug($"No IdentityResources found.");
			}
			catch (Exception err) { _log.Information($"Info processing IdentityResources : {err.Message}"); }

			try
			{
				var propScp = items.RootElement.GetProperty("scopes");
				_scopes = JsonSerializer.Deserialize<List<ApiScope>>(propScp, jopts) ?? new List<ApiScope>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug($"No Scopes found.");
			}
			catch (Exception err) { _log.Information($"Info processing Scopes : {err.Message}"); }

			try
			{
				var propTnt = items.RootElement.GetProperty("tenants").GetRawText();
				_tenants = JsonSerializer.Deserialize<List<Tenant>>(propTnt, jopts) ?? new List<Tenant>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug($"No Tenants found.");
			}
			catch (Exception err)
			{
				_log.Error($"Error processing Tenants : {err.Message}");
			}

			try
			{
				var propCl = items.RootElement.GetProperty("clients");
				_clients = JsonSerializer.Deserialize<List<Client>>(propCl, jopts) ?? new List<Client>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug($"No Clients found.");
			}
			catch (Exception err)
			{
				_log.Error($"Error processing Clients : {err.Message}");
			}

			try
			{
				var roles = items.RootElement.GetProperty("roles");
				_roles = JsonSerializer.Deserialize<List<string>>(roles, jopts) ?? new List<string>();
			}
			catch (KeyNotFoundException)
			{
				_log.Debug("No roles found.");
			}
			catch (Exception err)
			{
				_log.Error($"Error processing roles : {err.Message}");
			}
		}

		private void InitializeDatabase()
		{
			int createdNewRepository = 0;

			//  --Tenant
			if (!_tenantRepository.CollectionExists<Tenant>())
			{
				foreach (var tenant in _tenants)
				{
					_tenantRepository.Add<Tenant>(tenant);
				}
				_log.Information($"Added {_tenants.Count()} Tenants");
				createdNewRepository++;
			}

			//  --Scopes
			if (!_tenantRepository.CollectionExists<ApiScope>())
			{
				foreach (var scope in _scopes)
				{
					_tenantRepository.Add<ApiScope>(scope);
				}
				_log.Information($"Added {_scopes.Count()} APIScopes");
				createdNewRepository++;
			}

			//  --Client
			if (!_tenantRepository.CollectionExists<Client>())
			{
				foreach (var client in _clients)
				{
					_tenantRepository.Add<Client>(client);
				}
				_log.Information($"Added {_clients.Count()} Clients");
				createdNewRepository++;
			}

			//  --IdentityResource
			if (!_tenantRepository.CollectionExists<IdentityResource>())
			{
				foreach (var res in _resources)
				{
					_tenantRepository.Add<IdentityResource>(res);
				}
				_log.Information($"Added {_resources.Count()} Resources");
				createdNewRepository++;
			}

			//  --ApiResource
			if (!_tenantRepository.CollectionExists<ApiResource>())
			{
				foreach (var api in _apiresources)
				{
					_tenantRepository.Add<ApiResource>(api);
				}
				_log.Information($"Added {_apiresources.Count()} APIResources");
				createdNewRepository++;
			}

			foreach (var role in _roles)
			{
				if (_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult() == false)
				{
					_roleManager.CreateAsync(new ApplicationRole { Name = role }).GetAwaiter().GetResult();
				}
			}

			// If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
			if (createdNewRepository > 0)
			{
				_ = $"Tenant Repository created/populated! You should restart you website, so Mongo driver will be configured to ignore Extra Elements - e.g. IdentityServer \"_id\" ";
			}
		}

		/// <summary>
		/// Lets tidy up.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			_roleManager.Dispose();
		}
	}
}
