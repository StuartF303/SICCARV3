using System.Threading.Tasks;
using FluentValidation;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;

namespace TenantService.Validation
{
    public class CreateClientValidator : AbstractValidator<Client>
    {
        private readonly ITenantRepository _tenantRepository;

        public CreateClientValidator(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;

            RuleFor(client => client)
                .MustAsync(async (client, _) =>
                {
                    var result = await ClientExists(client.ClientId);
                    return result == false;
                })
                .WithMessage(client => $"A client with the client id '{client.ClientId}' already exists.");

            RuleFor(client => client)
                .SetValidator(new ClientValidator());
        }

        private async Task<bool> ClientExists(string clientId)
        {
            var result = await _tenantRepository.Exists<Client>(client => client.ClientId == clientId);
            return result;
        }
    }
}
