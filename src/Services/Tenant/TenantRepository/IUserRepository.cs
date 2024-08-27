using Siccar.Platform;

namespace TenantRepository
{
    public interface IUserRepository
    {
        Task<User?> Get(Guid id);
        Task<User> Update(User user);
        Task Delete(Guid id);
        Task<List<User>> ListByTenant(string tenantId);
    }
}
