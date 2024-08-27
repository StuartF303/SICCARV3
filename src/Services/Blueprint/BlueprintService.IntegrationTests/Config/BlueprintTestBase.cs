using System.Reflection;
using BlueprintService.V1.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siccar.Application;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using WebMotions.Fake.Authentication.JwtBearer;

namespace BlueprintService.IntegrationTests.Config
{
    [Collection("Blueprint")]
    public class BlueprintTestBase 
    {
        public readonly BlueprintOperations? BlueprintOperations;
        private readonly BlueprintWebApplicationFactory<WebHostStartup> _factory;
        private static HttpClient? _client;
        protected static ITenantServiceClient? _fakeTenantServiceClient;
        protected static IWalletServiceClient? _fakeWalletServiceClient;
        protected static IRegisterServiceClient? _fakeRegisterServiceClient;
        protected static IBlueprintRepository? _fakeBlueprintRepository;

        public BlueprintTestBase(BlueprintWebApplicationFactory<WebHostStartup> factory)
        {
            _factory = factory;
            BlueprintOperations = new(CreateClient());
        }

        private HttpClient CreateClient()
        {
            var path = Assembly.GetAssembly(typeof(WebHostStartup))!.Location;

            if (_client == null)
            {
                _client = _factory.WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            // Mocks
                            _fakeBlueprintRepository = A.Fake<IBlueprintRepository>();
                            A.CallTo(() => _fakeBlueprintRepository.GetBlueprint(A<string>.Ignored)).Returns(new Blueprint());

                            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
                            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
                            _fakeTenantServiceClient = A.Fake<ITenantServiceClient>();

                            services.Replace(ServiceDescriptor.Singleton(_fakeBlueprintRepository));
                            services.Replace(ServiceDescriptor.Singleton(_fakeRegisterServiceClient));
                            services.Replace(ServiceDescriptor.Singleton(_fakeWalletServiceClient));
                            services.Replace(ServiceDescriptor.Singleton(_fakeTenantServiceClient));

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
            }

            return _client;
        }
    }
}
