using Siccar.Platform;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Siccar.Registers.RegisterService.V1.Services
{
    public interface IRegisterResolver
    {
        public Task<IEnumerable<Register>> ResolveRegistersForUser(IEnumerable<Claim> userClaims);
        public Task ThrowIfUserNotAuthorizedForRegister(IEnumerable<Claim> userClaims, string registerId);
    }
}
