using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siccar.Platform.Tenants.V1.Controllers;
using Siccar.Platform.Tenants.Repository;
using Xunit;
using Siccar.Platform;
using Siccar.Common.ServiceClients;
using Siccar.Application;
using System.Text.Json;
using System.Linq;
using SiccarApplicationTests;
using System.Linq.Expressions;
using Siccar.Platform.Tenants.Core;
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Siccar.Common.Exceptions;
using TenantRepository;
#nullable enable

namespace TenantService.UnitTests
{
	public class TenantsControllerTest
	{
		private readonly ITenantRepository _fakeTenantRepository;
		private readonly TenantsController _underTest;
		private readonly IWalletServiceClient _fakeWalletServiceClient;
		private readonly IRegisterServiceClient _fakeRegisterServiceClient;
		private readonly IUserRepository _fakeUserRepository;
		private readonly TestData testData = new ();
		private readonly string _expectedOwner = Guid.NewGuid().ToString();
		private readonly string _expectedTenant = Guid.NewGuid().ToString();

		public TenantsControllerTest()
		{
			_fakeTenantRepository = A.Fake<ITenantRepository>();
			_fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
			_fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
			_fakeUserRepository = A.Fake<IUserRepository>();
			_underTest = new TenantsController(_fakeTenantRepository, _fakeWalletServiceClient, _fakeRegisterServiceClient, _fakeUserRepository)
			{
				ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() }
			};

			var claim = new Claim("tenant", _expectedTenant);
			var claim2 = new Claim("sub", _expectedOwner);
			var claims = new ClaimsIdentity(new List<Claim> { claim, claim2 });
			_underTest.HttpContext.User = new ClaimsPrincipal(claims);
		}

		public class GetAll : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Return_All_Tenants()
			{
				var expectedTenant = new Tenant();
				var expectedClient = new Client();
				var expectedTenantList = new List<Tenant> { expectedTenant };
				A.CallTo(() => _fakeTenantRepository.All<Tenant>()).Returns(expectedTenantList.AsQueryable());
				var result = await _underTest.GetTenants();
				var okResult = result.Result as OkObjectResult;
				A.CallTo(() => _fakeTenantRepository.All<Tenant>()).MustHaveHappened();
				Assert.Equal(expectedTenantList, okResult?.Value);
			}
			[Fact]
			public async Task Should_CallGetClients_ForEachTenant()
			{
				var expectedTenant = new Tenant();
				var expectedTenant2 = new Tenant();
				var expectedTenantList = new List<Tenant> { expectedTenant, expectedTenant2 };
				A.CallTo(() => _fakeTenantRepository.All<Tenant>()).Returns(expectedTenantList.AsQueryable());
				var result = await _underTest.GetTenants();
				var okResult = result.Result as OkObjectResult;
				A.CallTo(() => _fakeTenantRepository.Where<Client>(A<Expression<System.Func<Client, bool>>>._)).MustHaveHappenedTwiceExactly();
				Assert.Equal(expectedTenantList, okResult?.Value);
			}
			[Fact]
			public async Task Should_ReturnTenantWith_Clients()
			{
				var expectedTenant = new Tenant();
				var expectedClient = new Client();
				var expectedTenantList = new List<Tenant> { expectedTenant };
				var expectedClientList = new List<Client> { expectedClient };
				A.CallTo(() => _fakeTenantRepository.All<Tenant>()).Returns(expectedTenantList.AsQueryable());
				A.CallTo(() => _fakeTenantRepository.Where<Client>(A<Expression<System.Func<Client, bool>>>._)).Returns(expectedClientList.AsQueryable());
				var result = await _underTest.GetTenants();
				var okResult = result.Result as OkObjectResult;
				var listResult = okResult?.Value as List<Tenant>;
				Assert.Equal(expectedClientList, listResult?.First().Clients);
			}
		}

		public class GetTenant : TenantsControllerTest
		{
			[Fact]
			public async Task Should_CallGetTenant_Repository()
			{
				var expectedTenant = testData.tenant1();
				A.CallTo(() => _fakeTenantRepository.Single(A<Expression<System.Func<Tenant, bool>>>.Ignored)).Returns(expectedTenant);
				var result = await _underTest.GetTenantById(testData.tenantId);
				var okResult = result.Result as OkObjectResult;
				A.CallTo(() => _fakeTenantRepository.Single<Tenant>(A<Expression<System.Func<Tenant, bool>>>.Ignored)).MustHaveHappened(); // testc => testc.id == "someTenantId")
				Assert.Equal(expectedTenant, okResult?.Value);
			}
			[Fact]
			public async Task Should_ReturnTenantWith_Clients()
			{
				var expectedTenant = new Tenant();
				var expectedClient = new Client();
				var expectedClientList = new List<Client> { expectedClient };
				A.CallTo(() => _fakeTenantRepository.Single<Tenant>(A<Expression<System.Func<Tenant, bool>>>.Ignored)).Returns(expectedTenant);
				A.CallTo(() => _fakeTenantRepository.Where<Client>(A<Expression<System.Func<Client, bool>>>._)).Returns(expectedClientList.AsQueryable());
				var result = await _underTest.GetTenantById(expectedTenant.Id!);
				var okResult = result.Result as OkObjectResult;
				var listResult = okResult?.Value as Tenant;
				Assert.Equal(expectedClientList, listResult?.Clients);
			}
			[Fact]
			public async Task Tenant_DoesNotExist_ShouldReturn404()
			{
				A.CallTo(() => _fakeTenantRepository.Single(A<Expression<System.Func<Tenant, bool>>>.Ignored))!.Returns(default(Tenant));
				var result = await _underTest.GetTenantById("invalid");
				A.CallTo(() => _fakeTenantRepository.Single(A<Expression<System.Func<Tenant, bool>>>.Ignored)).MustHaveHappened();
				Assert.IsType<NotFoundResult>(result.Result);
			}
		}

		public class CreateTenant : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Create_Tenant_And_ReturnTenant()
			{
				var expectedTenant = new Tenant();
				var result = await _underTest.Post(expectedTenant);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeTenantRepository.Add<Tenant>(expectedTenant)).MustHaveHappened();
				Assert.Equal(expectedTenant, okResult?.Value);
			}
		}

		public class UpdateTenant : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Call_Update_Tenant()
			{
				var expectedTenant = new Tenant();
				var result = await _underTest.UpdateTenant(expectedTenant.Id!, expectedTenant);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeTenantRepository.UpdateTenant<Tenant>(expectedTenant)).MustHaveHappened();
				Assert.Equal(expectedTenant, okResult?.Value);
			}
		}

		public class DeleteTenant : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Call_Delete_Tenant()
			{
				var result = await _underTest.Delete(testData.tenantId);
				A.CallTo(() => _fakeTenantRepository.Delete<Tenant>(A<Expression<System.Func<Tenant, bool>>>.Ignored)).MustHaveHappened();
			}
		}

        public class PublishParticipant : TenantsControllerTest
        {
            [Fact]
            public async Task Should_Call_Publish_Participant()
            {
                var expectedTenant = new Tenant();
                var expectedRegisterId = "registerId1";
                var expectedAddress = "ws10000000";
                A.CallTo(() => _fakeTenantRepository.Single<Tenant>(A<Expression<System.Func<Tenant, bool>>>.Ignored))
                    .Returns(new Tenant() { Id = _expectedTenant, Registers = new List<string>() { expectedRegisterId } });
                var expectedParticipant = new Participant() { Name = "TEST", Organisation = "test" };
                var trackingData = new SortedList<string, string>
                {
                    { "organization", expectedParticipant.Organisation },
                    { "name", expectedParticipant.Name }
                };

                A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(A<Transaction>.Ignored, expectedAddress)).Returns(new TransactionModel());
                var ret = await _underTest.PublishParticipant(expectedRegisterId, expectedAddress, expectedParticipant);
                Assert.True(ret.Result != null);  // need to think of a better test
            }

            [Fact]
            public async Task ShouldThrow_WhenUserIsNotAuthorisedForRegister()
            {
                var expectedRegisterId = "registerId1";
                var expectedAddress = "ws10000000";
                var expectedParticipant = new Participant() { Name = "TEST", Organisation = "test" };
                await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.PublishParticipant(expectedRegisterId, expectedAddress, expectedParticipant));
            }
        }

        public class GetParticipantById : TenantsControllerTest
        {
            readonly Participant p1 = new() { Id = "p1Id", Name = "p1Name", Organisation = "Org", useStealthAddress = true, WalletAddress = "W" };
            byte[] p1payload = Array.Empty<byte>();
            readonly Participant p2 = new() { Id = "p1Id", Name = "p1Name", Organisation = "Org", useStealthAddress = true, WalletAddress = "W" };
            byte[] p2payload = Array.Empty<byte>();

			TransactionModel? p1Trans;
			byte[][] ploads1 = Array.Empty<byte[]>();
			TransactionModel? p2Trans;
			byte[][] ploads2 = Array.Empty<byte[]>();

			private void SetInitials()
			{
				p1payload = JsonSerializer.SerializeToUtf8Bytes(p1);
				p2payload = JsonSerializer.SerializeToUtf8Bytes(p2);
				p1Trans = new()
				{
					MetaData = new TransactionMetaData
					{
						RegisterId = "reg",
						TransactionType = TransactionTypes.Participant,
						TrackingData = new()
						{
							{ "organization", p1.Organisation },
							{ "name", p1.Name },
							{ "participantId", p1.Id }
						}
					}
				};
				ploads1 = new byte[][] { p1payload };
				p2Trans = new()
				{
					MetaData = new TransactionMetaData
					{
						RegisterId = "reg",
						TransactionType = TransactionTypes.Participant,
						TrackingData = new()
						{
							{ "organization", p2.Organisation },
							{ "name", p2.Name },
							{ "participantId", p2.Id }
						}
					}
				};
				ploads2 = new byte[][] { p2payload };
			}
			[Fact]
			public async Task Should_Return_Participant()
			{
				SetInitials();
				A.CallTo(() => _fakeRegisterServiceClient.GetParticipantTransactions(A<string>.Ignored)).Returns(new List<TransactionModel> { p1Trans!, p2Trans! });
				A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(p1Trans)).Returns(ploads1);
				A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(p2Trans)).Returns(ploads2);
				var ret = (await _underTest.GetPublishedParticipantById("reg", p1.Id)).Result as ObjectResult;
				Assert.Equal(ret?.StatusCode, StatusCodes.Status200OK);
				Assert.Equal(ret?.Value, p1);
				ret = (await _underTest.GetPublishedParticipantById("reg", p2.Id)).Result as ObjectResult;
				Assert.Equal(ret?.StatusCode, StatusCodes.Status200OK);
				Assert.Equal(ret?.Value, p2);
			}
			[Fact]
			public async Task Should_Return_Not_Found()
			{
				SetInitials();
				A.CallTo(() => _fakeRegisterServiceClient.GetParticipantTransactions(A<string>.Ignored)).Returns(new List<TransactionModel> { p1Trans!, p2Trans! });
				var ret = (await _underTest.GetPublishedParticipantById("reg", "bla-bla")).Result as ObjectResult;
				Assert.Equal(ret?.StatusCode, StatusCodes.Status404NotFound);
			}
		}

		public class RegisterCreated : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Add_RegisterId_To_Tenant()
			{
				var registerCreated = new Siccar.Platform.Registers.Core.Models.RegisterCreated
				{
					TenantId = Guid.NewGuid().ToString(),
					Id = Guid.NewGuid().ToString()
				};
				var expectedTenant = new Tenant { Id = registerCreated.TenantId };
				A.CallTo(() => _fakeTenantRepository.Single(A<Expression<Func<Tenant, bool>>>.Ignored)).Returns(expectedTenant);
				await _underTest.RegisterCreated(registerCreated);
				A.CallTo(() => _fakeTenantRepository.UpdateTenant(A<Tenant>.That.Matches(tenant => tenant.Id == expectedTenant.Id && tenant.Registers.Contains(registerCreated.Id))));
			}
		}

		public class RegisterDeleted : TenantsControllerTest
		{
			[Fact]
			public async Task Should_Remove_RegisterId_From_Tenant()
			{
				var registerDeleted = new Siccar.Platform.Registers.Core.Models.RegisterDeleted()
				{
					TenantId = Guid.NewGuid().ToString(),
					Id = Guid.NewGuid().ToString()
				};
				var expectedTenant = new Tenant
				{
					Id = registerDeleted.TenantId,
					Registers = new List<string> { registerDeleted.Id }
				};
				A.CallTo(() => _fakeTenantRepository.Single(A<Expression<Func<Tenant, bool>>>.Ignored)).Returns(expectedTenant);
				await _underTest.RegisterDeleted(registerDeleted);
				A.CallTo(() => _fakeTenantRepository.UpdateTenant(A<Tenant>.That.Matches(tenant => tenant.Id == expectedTenant.Id && !tenant.Registers.Contains(registerDeleted.Id))));
			}
		}
	}
}
