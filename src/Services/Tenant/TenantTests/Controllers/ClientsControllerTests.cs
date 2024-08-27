using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Siccar.Platform.Tenants.Core;
using Xunit;
using TenantService.V1.Controllers;
using Siccar.Platform.Tenants.Repository;
using System.Linq.Expressions;

namespace TenantUnitTests.Controllers
{
    public class ClientsControllerTests
    {
        private readonly ITenantRepository _fakeTenantRepository;
        private readonly ClientsController _underTest;

        public ClientsControllerTests()
        {
            _fakeTenantRepository = A.Fake<ITenantRepository>();
            _underTest = new ClientsController(_fakeTenantRepository);
        }

        public class UpdateClient : ClientsControllerTests
        {
            [Fact]
            public async Task Should_Call_Update_Client()
            {
                var expectedClient = new Client();

                var result = await _underTest.UpdateClient(expectedClient.ClientId, expectedClient);
                var okResult = result as OkObjectResult;

                A.CallTo(() => _fakeTenantRepository.UpdateClient(expectedClient)).MustHaveHappened();
                Assert.Equal(expectedClient, okResult!.Value);
            }
        }

        public class GetClient : ClientsControllerTests
        {
            [Fact]
            public async Task Should_Return_Client()
            {
                var expectedClient = new Client();
                A.CallTo(() => _fakeTenantRepository.Single(A<Expression<System.Func<Client, bool>>>.Ignored)).Returns(expectedClient);

                var result = await _underTest.GetClient("test-tenant", "test-client-id");
                var okResult = result.Result as OkObjectResult;
                A.CallTo(() => _fakeTenantRepository.Single(A<Expression<System.Func<Client, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
                Assert.Equal(expectedClient, okResult!.Value);
            }
        }

        public class CreateClient : ClientsControllerTests
        {
            [Fact]
            public async Task Valid_Client_Should_Create_Client()
            {
                var expectedClient = new Client
                {
                    ClientId = "test-client-id",
                    TenantId = "test-tenant"
                };

                await _underTest.CreateClient("test-tenant", expectedClient);

                A.CallTo(() => _fakeTenantRepository.Add(expectedClient))
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Invalid_Client_Should_Not_Create_Client_And_Return_Validation_Errors()
            {
                var expectedClient = new Client
                {
                    ClientId = "",
                    TenantId = "test-tenant"
                };

                var result = await _underTest.CreateClient("test-tenant", expectedClient);

                A.CallTo(() => _fakeTenantRepository.Add(expectedClient))
                    .MustNotHaveHappened();

                var validationProblemDetails = ((BadRequestObjectResult)result).Value as ValidationProblemDetails;

                Assert.Equal(typeof(BadRequestObjectResult), result.GetType());
                Assert.Equal("'Client Id' must not be empty.", validationProblemDetails!.Errors.First().Value.First());
            }
        }
    }
}
