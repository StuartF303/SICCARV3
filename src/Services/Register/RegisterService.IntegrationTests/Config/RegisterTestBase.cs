using System.Reflection;
using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Registers.Core;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RegisterService.IntegrationTests.Config
{
    [Collection("Register")]
    public class RegisterTestBase
    {
        public readonly RegisterOperations RegisterOperations;
        private readonly RegisterWebApplicationFactory<WebHostStartup> _factory;
        protected static ITenantServiceClient? _fakeTenantServiceClient;
        protected static IRegisterRepository? _fakeRegisterRepository;
        protected static IDaprClientAdaptor? _fakeDaprClientAdaptor;

        public RegisterTestBase(RegisterWebApplicationFactory<WebHostStartup> factory)
        {
            _factory = factory;
            RegisterOperations = new(CreateClient());
        }

        private HttpClient CreateClient()
        {
            var path = Assembly.GetAssembly(typeof(WebHostStartup))!.Location;

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Mocks
                    _fakeRegisterRepository = A.Fake<IRegisterRepository>();
                    _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
                    _fakeDaprClientAdaptor = A.Fake<IDaprClientAdaptor>();

                    A.CallTo(() => _fakeRegisterRepository.IsLocalRegisterAsync(A<string>.Ignored)).Returns(true);
                    A.CallTo(() => _fakeRegisterRepository.GetDocketAsync(A<string>.Ignored, A<ulong>.Ignored)).Returns(new Docket());
                    A.CallTo(() => _fakeTenantServiceClient.GetTenantById(A<string>.Ignored)).Returns(new Tenant());
                    services.Replace(ServiceDescriptor.Singleton(_fakeRegisterRepository));
                    services.Replace(ServiceDescriptor.Singleton(_fakeTenantServiceClient));
                    services.Replace(ServiceDescriptor.Singleton(_fakeDaprClientAdaptor));
                    services.AddSignalR();
 
                    // Fake auth
                    services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                });

                builder.UseStartup<WebHostStartup>();
                builder.UseContentRoot(Path.GetDirectoryName(path)!);
                builder.ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.Development.json", optional: false)
                        .AddEnvironmentVariables();
                });
            })
            .CreateClient();

            return client;
        }
    }
}
