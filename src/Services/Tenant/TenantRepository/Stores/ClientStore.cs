using IdentityServer4.Models;
using Siccar.Platform.Tenants.Repository;

namespace Siccar.Platform.Tenants.Repository.Stores
{
    public class ClientStore : IdentityServer4.Stores.IClientStore
    {
        protected ITenantRepository _dbRepository;

        public ClientStore(ITenantRepository repository)
        {
            _dbRepository = repository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return client;
        }
    }
}
