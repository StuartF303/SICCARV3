using BlueprintService.V1.Controllers;
using BlueprintService.V1.Repositories;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Application.Validation;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using SiccarApplicationTests;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace BlueprintUnitTests.V1.Controllers
{
	public class BlueprintsControllerTest
	{
		private readonly ILogger<BlueprintsController> _fakeLogger;
		private readonly IBlueprintRepository _fakeBlueprintRepository;
		private readonly IWalletServiceClient _fakeWalletServiceClient;
		private readonly IRegisterServiceClient _fakeRegisterServiceClient;
		private readonly ITenantServiceClient _fakeTenantServiceClient;
		private readonly IValidator<Blueprint> _validator;
		private readonly BlueprintsController _underTest;
		private readonly TestData testData = new();
		private readonly TransactionModel _blueprintTrans1;
		private readonly TransactionModel _blueprintTrans2;
		private readonly TransactionModel _blueprintTrans3;
		private readonly List<TransactionModel> bps = [];
		private readonly string _expectedOwner = Guid.NewGuid().ToString();
		private readonly string _expectedTenant = Guid.NewGuid().ToString();

		public BlueprintsControllerTest()
		{
			_fakeLogger = A.Fake<ILogger<BlueprintsController>>();
			_fakeBlueprintRepository = A.Fake<IBlueprintRepository>();
			_fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
			_fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
			_fakeTenantServiceClient = A.Fake<ITenantServiceClient>();
			_validator = new BlueprintValidator();
			_underTest = new BlueprintsController(_fakeLogger, _fakeBlueprintRepository, _fakeWalletServiceClient, _fakeRegisterServiceClient, _fakeTenantServiceClient, _validator)
			{
				ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() }
			};

			var claim = new Claim("tenant", _expectedTenant);
			var claim2 = new Claim("sub", _expectedOwner);
			var claims = new ClaimsIdentity(new List<Claim> { claim, claim2 });
			_underTest.HttpContext.User = new ClaimsPrincipal(claims);

			_blueprintTrans1 = new TransactionModel
			{
				TxId = Guid.NewGuid().ToString(),
				PrevTxId = new string($"").PadLeft(64, '0'),
				TimeStamp = DateTime.Now.AddDays(-2),
				MetaData = new TransactionMetaData { BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5" }
			};
			bps.Add(_blueprintTrans1);

			_blueprintTrans2 = new TransactionModel
			{
				TxId = Guid.NewGuid().ToString(),
				PrevTxId = _blueprintTrans1.Id,
				TimeStamp = DateTime.Now.AddDays(-2),
				MetaData = new TransactionMetaData { BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5" }
			};
			bps.Add(_blueprintTrans2);

			_blueprintTrans3 = new TransactionModel
			{
				TxId = Guid.NewGuid().ToString(),
				PrevTxId = new string("").PadLeft(64, '0'),
				TimeStamp = DateTime.Now.AddDays(-4),
				MetaData = new TransactionMetaData { BlueprintId = "A1234567-3DFE-4154-908B-59D31FB760E5" }
			};
			bps.Add(_blueprintTrans3);
		}

		public class GetAll : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Return_All_Blueprints()
			{
				var expectedBlueprint = new Blueprint { Id = "5b6ac972-17c8-4721-8b62-c065169bb824" };
				var expectedBlueprintList = new List<Blueprint> { expectedBlueprint };
				A.CallTo(() => _fakeBlueprintRepository.GetAll()).Returns(expectedBlueprintList);

				var result = await _underTest.GetAll();
				var okResult = result.Result as OkObjectResult;
				A.CallTo(() => _fakeBlueprintRepository.GetAll()).MustHaveHappened();
				Assert.Equal(expectedBlueprintList, okResult?.Value);
			}
		}

		public class GetBlueprint : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Return_Blueprint()
			{
				var expectedBlueprint = new Blueprint() { Id = "5b6ac972-17c8-4721-8b62-c065169bb824" };
				A.CallTo(() => _fakeBlueprintRepository.GetBlueprint(expectedBlueprint.Title)).Returns(expectedBlueprint);

				var result = await _underTest.Get(expectedBlueprint.Title);
				var okResult = result.Result as OkObjectResult;
				A.CallTo(() => _fakeBlueprintRepository.GetBlueprint(expectedBlueprint.Title)).MustHaveHappened();
				Assert.Equal(expectedBlueprint, okResult?.Value);
			}
		}

		public class GetPublishedBlueprint : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Call_GetPublishedBlueprintTransaction()
			{
				var blueprint = testData.blueprint1();
				var bpBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint));
				A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(A<TransactionModel>.Ignored)).Returns(new byte[][] { bpBytes });
				await _underTest.GetPublishedBlueprint(testData.registerId, blueprint.Id, null);
				A.CallTo(() => _fakeRegisterServiceClient.GetPublishedBlueprintTransaction(testData.registerId, blueprint.Id)).MustHaveHappened(); 
			}
			[Fact]
			public async Task Should_Call_DecryptTransaction_WhenWalletSpecified()
			{
				var walletAddress = "ws1asdfasdflknki21323lkadfa";
				var blueprint = testData.blueprint1();
				var bpBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint));
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { bpBytes });
				await _underTest.GetPublishedBlueprint(testData.registerId, blueprint.Id, walletAddress);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).MustHaveHappenedOnceExactly();
			}
		}

		public class Save : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Create_AndReturnBlueprint()
			{
				var expectedBlueprint = new Blueprint { Id = "5b6ac972-17c8-4721-8b62-c065169bb824" };
				var result = await _underTest.Save(expectedBlueprint);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeBlueprintRepository.SaveBlueprint(expectedBlueprint)).MustHaveHappened();
				Assert.Equal(expectedBlueprint, okResult?.Value);
			}
		}

		public class Publish : BlueprintsControllerTest
		{
			private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

			[Fact]
			public async Task Should_ReturnTxResponse()
			{
				SetupTestForSuccess(testData.registerId);
				var blueprintString = testData.simpleBlueprintStr();
				var blueprintDoc = JsonDocument.Parse(blueprintString);
				var expectedBlueprint = JsonSerializer.Deserialize<Blueprint>(blueprintString, options);
				var result = await _underTest.Publish(testData.walletAddress1, testData.registerId, expectedBlueprint);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeWalletServiceClient.SignAndSendTransaction(A<Transaction>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task Should_RemoveDraftBlueprint()
			{
				SetupTestForSuccess(testData.registerId);
				var blueprintString = testData.simpleBlueprintStr();
				var blueprintDoc = JsonDocument.Parse(blueprintString);
				var expectedBlueprint = JsonSerializer.Deserialize<Blueprint>(blueprintString, options);
				A.CallTo(() => _fakeBlueprintRepository.BluerpintExists(expectedBlueprint!.Id)).Returns(true);
				await _underTest.Publish(testData.walletAddress1, testData.registerId, expectedBlueprint);
				A.CallTo(() => _fakeBlueprintRepository.DeleteBlueprint(expectedBlueprint!.Id)).MustHaveHappenedOnceExactly();
			}
			[Fact]
			public async Task ShouldThrow_WhenUserIsNotAuthorisedForRegister()
			{
				var blueprintString = testData.simpleBlueprintStr();
				var blueprintDoc = JsonDocument.Parse(blueprintString);
				var expectedBlueprint = JsonSerializer.Deserialize<Blueprint>(blueprintString, options);
				await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.Publish(testData.walletAddress1, testData.registerId, expectedBlueprint));
			}

			private void SetupTestForSuccess(string registerId)
			{
				A.CallTo(() => _fakeTenantServiceClient.GetTenantById(A<string>._)) .Returns(new Tenant() { Id = _expectedTenant, Registers = [registerId] });
			}
		}

		public class GetAllPublished : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Call_GetBlueprints_WithRegisterId()
			{
				var registerId = "siccarextregister";
				var walletAddress = "ws1asdfasdflknki21323lkadfa";
				var result = await _underTest.GetAllPublished(registerId, walletAddress);
				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(registerId)).MustHaveHappenedOnceExactly();
			}

			[Theory]
			[InlineData(null)]
			[InlineData("")]
			public async Task Should_Call_GetAccessiblePayload_WhenWalletNotSpecified(string? walletAddress)
			{
				var registerId = "siccarextregister";
				var blueprint = new Blueprint();
				var bpBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint));
				var blueprintTrans1 = new TransactionModel
				{
					TxId = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData()
				};
				blueprintTrans1.MetaData.BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5";
				blueprintTrans1.PrevTxId = new string("").PadLeft(64, '0'); ;

				var blueprintTrans2 = new TransactionModel
				{
					TxId = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData()
				};
				blueprintTrans2.MetaData.BlueprintId = "A1234567-3DFE-4154-908B-59D31FB760E5";
				blueprintTrans2.PrevTxId = new string("").PadLeft(64, '0');

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(registerId)).Returns([blueprintTrans1, blueprintTrans2]);
				A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(A<TransactionModel>.Ignored)).Returns(new byte[][] { bpBytes });
				var result = await _underTest.GetAllPublished(registerId, walletAddress);
				A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(blueprintTrans1)).MustHaveHappenedOnceExactly()
					.Then(A.CallTo(() => _fakeWalletServiceClient.GetAccessiblePayloads(blueprintTrans2)).MustHaveHappenedOnceExactly());
			}

			[Fact]
			public async Task ShouldRetrieveLatestBlueprintsOnly()
			{
				var registerId = "siccarextregister";
				var walletAddress = "ws1asdfasdflknki21323lkadfa";
				var blueprint1 = new Blueprint
				{
					Id = "C65F6542-3DFE-4154-908B-59D31FB760E5",
					Version = 2
				};
				var bpBytes1 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint1));

				var blueprint2 = new Blueprint
				{
					Id = "A1234567-3DFE-4154-908B-59D31FB760E5",
					Version = 2
				};
				var bpBytes2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint2));

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(registerId)).Returns(bps);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { bpBytes1 }).Once();
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { bpBytes2 }).Once();

				var actionResult = await _underTest.GetAllPublished(registerId, walletAddress);
				var result = actionResult.Result as OkObjectResult;
				var returnValue = Assert.IsType<List<Blueprint>>(result!.Value);
				Assert.NotNull(actionResult);
				Assert.Equal(2, returnValue.Count);
				returnValue.Should().Contain(blueprint1);
				returnValue.Should().Contain(blueprint2);
			}
			[Fact]
			public async Task Should_CallWalletServiceDecrypt_ForEachBlueprint()
			{
				var registerId = "siccarextregister";
				var walletAddress = "ws1asdfasdflknki21323lkadfa";
				var blueprint = new Blueprint();
				var bpBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprint));
				var blueprintTrans1 = new TransactionModel
				{
					TxId = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData()
				};
				blueprintTrans1.MetaData.BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5";
				blueprintTrans1.PrevTxId = new string("").PadLeft(64, '0'); ;

				var blueprintTrans2 = new TransactionModel
				{
					TxId = Guid.NewGuid().ToString(),
					MetaData = new TransactionMetaData()
				};
				blueprintTrans2.MetaData.BlueprintId = "A1234567-3DFE-4154-908B-59D31FB760E5";
				blueprintTrans2.PrevTxId = new string("").PadLeft(64, '0'); ;

				A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(registerId)).Returns([blueprintTrans1, blueprintTrans2]);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(A<TransactionModel>.Ignored, walletAddress)).Returns(new byte[][] { bpBytes });
				var result = await _underTest.GetAllPublished(registerId, walletAddress);
				A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(blueprintTrans1, walletAddress)).MustHaveHappenedOnceExactly()
					.Then(A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(blueprintTrans2, walletAddress)).MustHaveHappenedOnceExactly());
			}

			//ToDo Get this test to pass. Complexity with parallel loop.
			//[Fact]
			//public async Task Should_Return_EachBlueprint()
			//{
			//    var registerId = "siccarextregister";
			//    var walletAddress = "ws1asdfasdflknki21323lkadfa";
			//    var blueprintTx1 = new Transaction() { TxId = Guid.NewGuid().ToString() };
			//    var blueprintTx2 = new Transaction() { TxId = Guid.NewGuid().ToString() };
			//    A.CallTo(() => _fakeRegisterServiceClient.GetBlueprintTransactions(registerId)).Returns(new List<Transaction> { blueprintTx1, blueprintTx2 });

			//    var blueprintTrans1 = new Blueprint() { Id = Guid.NewGuid().ToString() };
			//    var blueprintTrans2 = new Blueprint() { Id = Guid.NewGuid().ToString() };
			//    var bpBytes1 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprintTrans1));
			//    var bpBytes2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blueprintTrans1));
			//    A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(blueprintTx1, walletAddress)).Returns(new byte[][] { bpBytes1 });
			//    A.CallTo(() => _fakeWalletServiceClient.DecryptTransaction(blueprintTx2, walletAddress)).Returns(new byte[][] { bpBytes2 });
			//    var result = await _underTest.GetAllPublished(walletAddress, registerId);
			//    var okResult = result.Result as OkObjectResult;
			//    var returnValue = Assert.IsType<List<Blueprint>>(okResult.Value);
			//    Assert.Collection(returnValue, bp => Assert.Contains(blueprintTrans1.Id, bp.Id), bp => Assert.Contains(blueprintTrans2.Id, bp.Id));
			//}
		}
		public class Update : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Update_Blueprint()
			{
				var expectedBlueprint = new Blueprint { Id = "5b6ac972-17c8-4721-8b62-c065169bb824" };
				var result = await _underTest.Update(expectedBlueprint.Id, expectedBlueprint);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeBlueprintRepository.UpdateBlueprint(expectedBlueprint)).MustHaveHappened();
				Assert.Equal(expectedBlueprint, okResult?.Value);
			}
		}

		public class Delete : BlueprintsControllerTest
		{
			[Fact]
			public async Task Should_Delete_Blueprint()
			{
				var expectedBlueprint = new Blueprint { Id = "5b6ac972-17c8-4721-8b62-c065169bb824" };
				var result = await _underTest.Delete(expectedBlueprint.Id);
				var okResult = result as AcceptedResult;
				A.CallTo(() => _fakeBlueprintRepository.DeleteBlueprint(expectedBlueprint.Id)).MustHaveHappened();
			}
		}
	}
}
