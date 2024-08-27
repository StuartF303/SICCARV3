using System.Net.Http.Json;
using Siccar.EndToEndTests.Common;

namespace Siccar.EndToEndTests.Register
{
    internal class RegisterOperations
    {
        private readonly string _registerUri = "api/registers";

        public async Task<HttpResponseMessage> Create(Models.Register register)
        {
            var httpClient = await HttpClientFactory.Create();
            return await httpClient.PostAsJsonAsync(_registerUri, register);
        }
    }
}
