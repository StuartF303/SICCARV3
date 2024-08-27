using System;
using FluentValidation;
using Siccar.Platform.Tenants.Core;

namespace TenantService.Validation
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(client => client.Id)
                .NotEmpty();

            RuleFor(client => client.TenantId)
                .NotEmpty();

            RuleFor(client => client.ClientId)
                .NotEmpty();

            RuleFor(client => client.ClientId)
                .MaximumLength(Guid.NewGuid().ToString().Length);

            When(client => client.RequireClientSecret, () =>
            {
                RuleForEach(client => client.ClientSecrets).ChildRules(secrets =>
                {
                    secrets.RuleFor(x => x.Type).NotEmpty();
                    secrets.RuleFor(x => x.Value).NotEmpty();
                });
            });
        }
    }
}
