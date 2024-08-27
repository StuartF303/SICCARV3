using System.Reflection;
using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using WebMotions.Fake.Authentication.JwtBearer;

namespace ActionService.IntegrationTests.Config
{
    [Collection("Action")]
    public class ActionTestBase
    {
        public readonly ActionOperations ActionOperations;
        private readonly ActionWebApplicationFactory<WebHostStartup> _factory;

        public ActionTestBase(ActionWebApplicationFactory<WebHostStartup> factory)
        {
            _factory = factory;
            ActionOperations = new(CreateClient());
        }

        private HttpClient CreateClient()
        {
            var path = Assembly.GetAssembly(typeof(WebHostStartup))!.Location;

            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Mocks
                        var fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
                        var fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
                        A.CallTo(() => fakeRegisterServiceClient.GetTransactionById(A<string>.Ignored, A<string>.Ignored)).Returns(new TransactionModel(){MetaData = new TransactionMetaData()});

                        services.Replace(ServiceDescriptor.Singleton(fakeWalletServiceClient));
                        services.Replace(ServiceDescriptor.Singleton(fakeRegisterServiceClient));
                        
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
