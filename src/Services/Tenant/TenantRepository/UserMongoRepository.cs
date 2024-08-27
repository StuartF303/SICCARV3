using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;

namespace TenantRepository
{
    public class UserMongoRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserMongoRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User?> Get(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user == null
                ? null
                : await MapUser(user);
        }

        public async Task<User> Update(User user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            var existingRoles = await _userManager.GetRolesAsync(existingUser!);

            var rolesToAdd = user.Roles?.Except(existingRoles).ToList();
            var rolesToRemove = existingRoles.Except(user.Roles ?? new List<string>()).ToList();

            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(existingUser!, rolesToRemove);
            }

            if (rolesToAdd?.Any() ?? false)
            {
                await _userManager.AddToRolesAsync(existingUser!, rolesToAdd);
            }
            return user;
        }

        public async Task Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<List<User>> ListByTenant(string tenantId)
        {
            var users = _userManager.Users.Where(
                user => user.Claims.Any(c => c.Type == "tenant" && c.Value == tenantId)).ToList();

            var userTasks = users.Select(MapUser);

            var result = (await Task.WhenAll(userTasks)).ToList();
            return result;
        }

        private async Task<User> MapUser(ApplicationUser user)
        {
            return new User
            {
                Id = user.Id,
                Tenant = user.Claims.FirstOrDefault(u => u.Type == "tenant")?.Value,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                UserName = user.Claims.FirstOrDefault(u => u.Type == JwtClaimTypes.Name)?.Value
            };
        }
    }
}
