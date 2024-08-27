using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Siccar.Platform.Tenants.Repository.Stores
{
    public class ResourceStore : IResourceStore
    {
        protected ITenantRepository _dbRepository;

        public ResourceStore(ITenantRepository repository)
        {
            _dbRepository = repository;
        }

        private async Task<IEnumerable<ApiScope>> GetAllApiScopes()
        {
            return await _dbRepository.All<ApiScope>();
        }

        private async Task<IEnumerable<ApiResource>> GetAllApiResources()
        {
            return await _dbRepository.All<ApiResource>();
        }

        private async Task<IEnumerable<IdentityResource>> GetAllIdentityResources()
        {
            return await _dbRepository.All<IdentityResource>();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return _dbRepository.Single<ApiResource>(a => a.Name == name);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(a.Name)));

            return list.AsEnumerable();
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));

            return list.AsEnumerable();
        }

        public IdentityServer4.Models.Resources GetAllResources()
        {
            var result = new IdentityServer4.Models.Resources(GetAllIdentityResources().Result, GetAllApiResources().Result, GetAllApiScopes().Result);
            return result;
        }

        private Func<IdentityResource, bool> BuildPredicate(Func<IdentityResource, bool> predicate)
        {
            return predicate;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(await GetAllIdentityResources(), await GetAllApiResources(), await GetAllApiScopes());
            return result;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));

            return list.AsEnumerable();
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<ApiScope>(s => scopeNames.Contains(s.Name));

            return list.AsEnumerable();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<ApiResource>(a => scopeNames.Contains(a.Name));

            return list.AsEnumerable();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return await Task.Run(() => Array.Empty<ApiResource>());
        }
    }
}
