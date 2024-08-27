using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Registers.Core;
using Siccar.Registers.Core.MongoDBStorage;
using Swashbuckle.AspNetCore.Annotations;
using static Google.Rpc.Context.AttributeContext.Types;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Siccar.Registers.RegisterService.V1.Services
{
    public class RegisterResolver : IRegisterResolver
    {

        private readonly IRegisterRepository _registerRepository;
        private readonly ITenantServiceClient _tenantServiceClient;

        public RegisterResolver(
            IRegisterRepository registerRepository,
            ITenantServiceClient tenantServiceClient
            )
        {
            _registerRepository = registerRepository;
            _tenantServiceClient = tenantServiceClient;
        }

        public async Task<IEnumerable<Register>> ResolveRegistersForUser(IEnumerable<Claim> userClaims)
        {
            var hasInstallationRole = userClaims.Any(claim => claim.Type == "role" && claim.Value.Contains("installation"));
            if (hasInstallationRole)
                return await _registerRepository.GetRegistersAsync();

            var tenantClaim = userClaims.ToList().Find(claim => claim.Type.ToLower() == "tenant").Value;
            var tenant = await _tenantServiceClient.GetTenantById(tenantClaim);
            if (tenant == null)
                throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, $"Tenant with id: {tenantClaim} could not be found.");
            return await _registerRepository.QueryRegisters(reg => tenant.Registers.Contains(reg.Id));
        }

        public async Task ThrowIfUserNotAuthorizedForRegister(IEnumerable<Claim> userClaims, string registerId)
        {
            var hasInstallationRole = userClaims.Any(claim => claim.Type == "role" && claim.Value.Contains("installation"));
            if (hasInstallationRole)
                return;

            var tenantClaim = userClaims.ToList().Find(claim => claim.Type.ToLower() == "tenant").Value;
            var tenant = await _tenantServiceClient.GetTenantById(tenantClaim);
            if (tenant == null)
                throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, $"Tenant with id: {tenantClaim} could not be found.");
            if(!tenant.Registers.Contains(registerId))
                throw new HttpStatusException(System.Net.HttpStatusCode.Forbidden, $"Tenant is not authorized to access register: {registerId}");
            return;
        }
    }
}
