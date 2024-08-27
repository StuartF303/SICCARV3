using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Siccar.Application;
using BlueprintService.V1.Repositories;
using BlueprintService.Configuration;
using Siccar.Common.Adaptors;
using System.Text.Json;
using Dapr.Client;
using BlueprintService.Exceptions;
using System.Net;
using MongoDB.Driver.Core.Authentication;
using System;

namespace BlueprintUnitTests.Repositories
{
    public class DaprBlueprintRepositoryTest
    {
        private readonly ILogger<DaprBlueprintRepository> _fakeLogger;
        private readonly IDaprClientAdaptor _fakeClientAdaptor;
        private readonly DaprBlueprintRepository _underTest;
        private const string blueprintId = "My Blueprint";
        private Blueprint mockBlueprintObject = BuildMockBlueprint(blueprintId);
        private readonly List<string> blueprintKeys = new List<string> { blueprintId, "Another blueprint" };
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public DaprBlueprintRepositoryTest()
        {
            _fakeLogger = A.Fake<ILogger<DaprBlueprintRepository>>();
            _fakeClientAdaptor = A.Fake<IDaprClientAdaptor>();
            _underTest = new DaprBlueprintRepository(_fakeLogger, _fakeClientAdaptor);
        }
        public class BluerpintExists : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_ReturnTrueWhenBlueprintReturned()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(mockBlueprintObject);

                var result = await _underTest.BluerpintExists(mockBlueprintObject.Id);

                Assert.True(result);
            }

            [Fact]
            public async Task Should_ReturnFalseWhenNullReturned()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(null);

                var result = await _underTest.BluerpintExists(blueprintId);

                Assert.False(result);
            }
        }

        public class GetAll : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_LogWarning_When_GetAllBlueprintKeysState_is_null()
            {
                SetupTestForFailure();

                await _underTest.GetAll();

                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnEmpty_Array_When_NoKeysExist()
            {
                SetupTestForSuccess();
                var emptyItem = new BulkStateItem("some-id", "", "");

                var bulkStateItemList = new List<BulkStateItem>() { emptyItem };

                A.CallTo(() => _fakeClientAdaptor.GetBulkStateAsync(BlueprintConstants.StoreName, A<List<string>>.Ignored, -1)).Returns(bulkStateItemList);

                var result = await _underTest.GetAll();

                Assert.Empty(result);
            }

            [Fact]
            public async Task Should_Call_GetBulkState_WithAllBlueprintKeys()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey)).Returns(blueprintKeys);

                var result = await _underTest.GetAll();

                A.CallTo(() => _fakeClientAdaptor.GetBulkStateAsync(BlueprintConstants.StoreName, blueprintKeys, -1)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ReturnAllBlueprints_From_GetBulkState()
            {
                SetupTestForSuccess();

                var blueprints = new List<Blueprint> { BuildMockBlueprint(blueprintKeys[0]), BuildMockBlueprint(blueprintKeys[1]) };

                var blueprintItem1 = new BulkStateItem(blueprints[0].Id, JsonSerializer.Serialize(blueprints[0], _serializerOptions), "");
                var blueprintItem2 = new BulkStateItem(blueprints[1].Id, JsonSerializer.Serialize(blueprints[1], _serializerOptions), "");

                var bulkStateItemList = new List<BulkStateItem>() { blueprintItem1, blueprintItem2 };

                A.CallTo(() => _fakeClientAdaptor.GetBulkStateAsync(BlueprintConstants.StoreName, blueprintKeys, -1)).Returns(bulkStateItemList);

                var result = await _underTest.GetAll();

                var isTrue = blueprints[0].Equals(result[0]);
                Assert.Contains(blueprints[0], result);
                Assert.Contains(blueprints[1], result);
            }
        }

        public class GetBlueprint : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_LogWarning_When_GetState_Returns_Null()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(null);

                _ = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(() => _underTest.GetBlueprint(blueprintId));

                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_ThrowNotFound_When_GetState_Returns_Null()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(null);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.GetBlueprint(blueprintId));

                Assert.Equal(HttpStatusCode.NotFound, exception.Status);
            }

            [Fact]
            public async Task Should_Return_Blueprint()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns(mockBlueprintObject);

                var result = await _underTest.GetBlueprint(mockBlueprintObject.Id);

                Assert.Equal(mockBlueprintObject, result);
                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Information)
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class SaveBlueprint : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_ThrowBadRequest_When_BlueprintAlreadyExists()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns(mockBlueprintObject);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.SaveBlueprint(mockBlueprintObject));

                Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
            }

            [Fact]
            public async Task Should_LogWarning_When_BlueprintAlreadyExists()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns(mockBlueprintObject);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.SaveBlueprint(mockBlueprintObject));

                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallSaveState_UpdatingAllBlueprintKeys()
            {
                SetupTestForSuccess();

                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns<Blueprint>(null);

                await _underTest.SaveBlueprint(mockBlueprintObject);

                A.CallTo(() => _fakeClientAdaptor.SaveStateAsync(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey, A<List<string>>.That.Contains(mockBlueprintObject.Id))).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallSaveState_SavingTheNewBlueprint()
            {
                SetupTestForSuccess();

                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns<Blueprint>(null);

                await _underTest.SaveBlueprint(mockBlueprintObject);

                A.CallTo(() => _fakeClientAdaptor.SaveStateAsync(BlueprintConstants.StoreName, mockBlueprintObject.Id, mockBlueprintObject)).MustHaveHappenedOnceExactly();
            }
        }

        public class UpdateBlueprint : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_ThrowBadRequest_When_BlueprintDoesNotExist()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(null);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.UpdateBlueprint(mockBlueprintObject));

                Assert.Equal(HttpStatusCode.NotFound, exception.Status);
            }

            [Fact]
            public async Task Should_LogWarning_When_BlueprintDoesNotExist()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprintId)).Returns<Blueprint>(null);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.UpdateBlueprint(mockBlueprintObject));

                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallSaveState_UpdatingTheExistingBlueprint()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns(mockBlueprintObject);

                await _underTest.UpdateBlueprint(mockBlueprintObject);

                A.CallTo(() => _fakeClientAdaptor.SaveStateAsync(BlueprintConstants.StoreName, mockBlueprintObject.Id, mockBlueprintObject)).MustHaveHappenedOnceExactly();
            }
        }

        public class DeleteBlueprint : DaprBlueprintRepositoryTest
        {
            [Fact]
            public async Task Should_ThrowNotFound_When_BlueprintDoesNotExist()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns<Blueprint>(null);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.DeleteBlueprint(mockBlueprintObject.Id));

                Assert.Equal(HttpStatusCode.NotFound, exception.Status);
            }

            [Fact]
            public async Task Should_LogWarning_When_BlueprintDoesNotExist()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns<Blueprint>(null);

                var exception = await Assert.ThrowsAsync<DaprBlueprintRepositoryException>(async () => await _underTest.DeleteBlueprint(mockBlueprintObject.Id));

                A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallDeleteState_UpdatingAllBlueprintKeys()
            {
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey)).Returns(new List<string> { mockBlueprintObject.Id, "5f63a366-99d2-411c-9b42-4bb5fc7d630d" });
                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns<Blueprint>(mockBlueprintObject);

                await _underTest.DeleteBlueprint(mockBlueprintObject.Id);

                A.CallTo(() => _fakeClientAdaptor.SaveStateAsync(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey, A<List<string>>.That.Not.Contains(mockBlueprintObject.Id))).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task Should_CallDeleteState_DeletingTheExistantBlueprint()
            {
                SetupTestForSuccess();

                A.CallTo(() => _fakeClientAdaptor.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, mockBlueprintObject.Id)).Returns(mockBlueprintObject);

                await _underTest.DeleteBlueprint(mockBlueprintObject.Id);

                A.CallTo(() => _fakeClientAdaptor.DeleteStateAsync(BlueprintConstants.StoreName, mockBlueprintObject.Id)).MustHaveHappenedOnceExactly();
            }
        }


        private void SetupTestForSuccess()
        {
            A.CallTo(() => _fakeClientAdaptor.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey)).Returns(blueprintKeys);
        }

        private void SetupTestForFailure()
        {
            A.CallTo(() => _fakeClientAdaptor.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey)).Returns<List<string>>(null);
        }

        private static Blueprint BuildMockBlueprint(string id)
        {
            return new Blueprint
            {
                Id = id,
                Title = "A blueprint title",
                Description = "Some description",
                Participants = new List<Participant> { new Participant { Name = "Actor 1" } }
            };
        }
    }
}
