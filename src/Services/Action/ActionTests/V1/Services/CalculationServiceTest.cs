using ActionService.V1.Services;
using FakeItEasy;
using Json.More;
using Siccar.Application;
using Siccar.Common.Exceptions;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace ActionUnitTests.V1.Services
{
    public class CalculationServiceTest
    {
        private readonly IRegisterServiceClient _fakeRegisterServiceClient;
        private readonly IWalletServiceClient _fakeWalletServiceClient;
        private readonly IPayloadResolver _fakePayloadResolver;
        private readonly ICalculationService _underTest;

        public CalculationServiceTest()
        {
            _fakePayloadResolver = A.Fake<IPayloadResolver>();
            _fakeRegisterServiceClient = A.Fake<IRegisterServiceClient>();
            _fakeWalletServiceClient = A.Fake<IWalletServiceClient>();
            _underTest = new CalculationService(_fakeRegisterServiceClient, _fakePayloadResolver, _fakeWalletServiceClient);
        }

        [Fact]
        public async Task Should_ReturnUnmodifiedSubmissionData_WhenThereAreNoCalculations()
        {
            var action = new Siccar.Application.Action();
            var data = JsonDocument.Parse("""{"litresofOil": 10}""");
            var submission = new ActionSubmission { Data = data };
            var instanceId = Guid.NewGuid().ToString();

            var result = await _underTest.RunActionCalculationsAsync(action, submission, instanceId);

            Assert.True(result.IsEquivalentTo(data));
        }

        [Fact]
        public async Task Should_ThrowWhenCalculationResultIsNotANumber()
        {
            var calculation = JsonNode.Parse("""
                {
                    "*": [
                        {
                            "var": "litresofOil"
                        },
                        {
                            "var": "conversionFactor"
                        }
                    ]
                }
                """);
            var action = new Siccar.Application.Action() { Calculations = new Dictionary<string, JsonNode>() { { "conversionResult", calculation } } };
            var data = JsonDocument.Parse("""{"litresofOil": "string", "conversionFactor": 10}""");
            var submission = new ActionSubmission { Data = data };
            var instanceId = Guid.NewGuid().ToString();
            var expected = JsonDocument.Parse("""{"conversionResult":100, "litresofOil": 10,"conversionFactor": 10}""");

            await Assert.ThrowsAsync<HttpStatusException>(() => _underTest.RunActionCalculationsAsync(action, submission, instanceId));
        }

        [Fact]
        public async Task Should_ReturnDataResultFromCalculation()
        {
            var calculation = JsonNode.Parse("""
                {
                    "*": [
                        {
                            "var": "litresofOil"
                        },
                        {
                            "var": "conversionFactor"
                        }
                    ]
                }
                """);
            var action = new Siccar.Application.Action() { Calculations = new Dictionary<string, JsonNode>() { { "conversionResult", calculation } } };
            var data = JsonDocument.Parse("""{"litresofOil": 10, "conversionFactor": 10}""");
            var submission = new ActionSubmission { Data = data };
            var instanceId = Guid.NewGuid().ToString();
            var expected = JsonDocument.Parse("""{"conversionResult":100, "litresofOil": 10,"conversionFactor": 10}""");

            var result = await _underTest.RunActionCalculationsAsync(action, submission, instanceId);

            Assert.True(result.IsEquivalentTo(expected));
        }

        [Fact]
        public async Task Should_ReturnDataForCalculationsFromSubmittedDataAndPreviousData()
        {
            var calculation = JsonNode.Parse("""
                {
                    "*": [
                        {
                            "var": "litresofOil"
                        },
                        {
                            "var": "conversionFactor"
                        }
                    ]
                }
                """);
            var action = new Siccar.Application.Action() { Calculations = new Dictionary<string, JsonNode>() { { "conversionResult", calculation } } };
            var data = JsonDocument.Parse("""{"conversionFactor": 10}""");
            var submission = new ActionSubmission { Data = data };
            var instanceId = Guid.NewGuid().ToString();
            var previousData = new Dictionary<string, object>() { { "litresofOil", 10 } };
            var expected = JsonDocument.Parse("""{"conversionResult":100,"conversionFactor": 10}""");

            A.CallTo(() => _fakePayloadResolver.GetAllPreviousPayloadsForWalletAsync(submission.WalletAddress, A<List<TransactionModel>>._, _fakeWalletServiceClient)).Returns(previousData);

            var result = await _underTest.RunActionCalculationsAsync(action, submission, instanceId);

            Assert.True(result.IsEquivalentTo(expected));
        }

        [Fact]
        public async Task Should_ReturnCompoundedCalculation()
        {
            var calculation1 = JsonNode.Parse("""
                {
                    "*": [
                        {
                            "var": "litresofOil"
                        },
                        {
                            "var": "conversionFactor"
                        }
                    ]
                }
                """);
            var calculation2 = JsonNode.Parse("""
                {
                    "+": [
                        {
                            "var": "conversionResult"
                        },
                        {
                            "var": "conversionFactor"
                        }
                    ]
                }
                """);
            var action = new Siccar.Application.Action() { Calculations = new Dictionary<string, JsonNode>() { { "conversionResult", calculation1 }, { "conversionAndadditionResult", calculation2 } } };
            var data = JsonDocument.Parse("""{"litresofOil": 10, "conversionFactor": 10}""");
            var submission = new ActionSubmission { Data = data };
            var instanceId = Guid.NewGuid().ToString();
            var expected = JsonDocument.Parse("""{"conversionResult": 100, "litresofOil": 10,"conversionFactor": 10,"conversionAndadditionResult": 110}""");

            var result = await _underTest.RunActionCalculationsAsync(action, submission, instanceId);

            Assert.True(result.IsEquivalentTo(expected));
        }
    }
}
