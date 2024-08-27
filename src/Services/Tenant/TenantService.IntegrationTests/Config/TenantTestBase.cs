using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using IdentityServer4;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;
using Siccar.Platform.Tenants.Repository;
using WebMotions.Fake.Authentication.JwtBearer;

namespace TenantService.IntegrationTests.Config
{
    [Collection("Tenant")]
    public class TenantTestBase 
    {
        public readonly TenantOperations TenantOperations;
        private readonly TenantWebApplicationFactory<WebHostStartup> _factory;
        private static HttpClient? _client;
        protected static ITenantRepository? _fakeTenantRepository;
        protected static IWalletServiceClient? _fakeWalletServiceClient;
        protected static IRegisterServiceClient? _fakeRegisterServiceClient;

        public TenantTestBase(TenantWebApplicationFactory<WebHostStartup> factory)
        {
            _factory = factory;
            TenantOperations = new(CreateClient());
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
                            _fakeTenantRepository = A.Fake<ITenantRepository>();
                            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();

                            A.CallTo(() => _fakeTenantRepository.Single(A<Expression<Func<Client, bool>>>
                                .Ignored)).Returns(new Client());

                            A.CallTo(() => _fakeTenantRepository.All<Tenant>()).Returns(new List<Tenant>().AsQueryable());

                            services.Replace(ServiceDescriptor.Singleton(_fakeTenantRepository));
                            services.Replace(ServiceDescriptor.Singleton(_fakeWalletServiceClient));
                            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                                .AddLocalApi(o =>
                                {
                                    o.ExpectedScope = null;
                                }).AddFakeJwtBearer();

                            services.AddAuthorization(options =>
                            {
                                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                                {
                                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme, FakeJwtBearerDefaults.AuthenticationScheme);
                                    policy.RequireAuthenticatedUser();
                                });
                            });
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
