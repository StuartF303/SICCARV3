using ActionService.V1.Services;
using Xunit;
using Siccar.Application;
using System.Collections.Generic;
using System.Text.Json;
using ActionService.Exceptions;
using SiccarApplicationTests;

namespace ActionUnitTests.V1.Services
{
    public class ActionResolverTest
    {
        private readonly ActionResolver _underTest;
        private readonly TestData testData = new();
    
        public ActionResolverTest()
        {
            _underTest = new ActionResolver();
        }
        public class ResolveNextAction : ActionResolverTest
        {
            [Fact]
            public void Should_ResolveNextAction()
            {
				var expected = new Siccar.Application.Action { Id = 2, Sender = testData.walletAddress1 };
                var testBlueprint = testData.blueprint1();

                testBlueprint.Actions.Add(testData.action2());
    

                var result = _underTest.ResolveNextAction(expected.Id, testBlueprint);

                Assert.Equal(2, result.Id);
            }


            [Fact]
            public void Should_ThrowNoSuchAction()
            {
                var action = new Action { Id = 1 };
                var expected = new Action { Id = 2, Sender = testData.walletAddress1 };
                var testBlueprint = testData.blueprint1();
                // we dont have a second action currently

                Assert.Throws<ActionResolverException>(() => _underTest.ResolveNextAction(expected.Id, testBlueprint));
            }
        }
        public class IsFinalAction : ActionResolverTest
        {
            [Fact]
            public void Should_Verify_NextAction_NotFinal()
            {
                var testBlueprint = testData.blueprint1();
                testBlueprint.Actions.Add(testData.action2());
                var currentAction = testData.action1();
                var nextAction = testData.action2();

                var result = _underTest.IsFinalAction(currentAction, testBlueprint, testData.emptyDoc, out var nextActionId);

                Assert.False(result); // we have another action
                Assert.Equal(nextAction.Id, nextActionId); // and its number 2
            }

            [Fact]
            public void Should_ReturnTrue_WhenNextActionIsLast()
            {
                var testBlueprint = testData.blueprint1();
                var currentAction = testData.action1();

                var result = _underTest.IsFinalAction(currentAction, testBlueprint, testData.emptyDoc, out var _);

                Assert.True(result);
            }

            [Fact]
            public void Should_ReturnNegative1_WhenNextActionIsLast()
            {
                var testBlueprint = testData.blueprint1();
                var currentAction = testData.action1();

                _underTest.IsFinalAction(currentAction, testBlueprint, testData.emptyDoc, out var nextActionId);
                Assert.Equal(-1, nextActionId);
            }

            [Theory]
            [InlineData("Wheat", false)]
            [InlineData("Linseed", true)]
            public void Should_ReturnBool_BasedOnCondition(string conditional, bool expected)
            {
                Condition condition = new(); 
                var rule = "{\"if\":[{\"==\": [{\"var\": \"product\"}, \"" + conditional + "\"] }, 2,-1]}";
                ((List<string>)condition.Criteria).Add(rule);


				var data = new Dictionary<string, object>
				{
					{ "product", "Wheat" }
				};
				var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(data));

                var participantId = System.Guid.NewGuid().ToString();
                var walletAddress = "ws1walletaddress";
                var action = new Action { Id = 1, Condition = rule };
                var action2 = new Action { Id = 2, Sender = participantId };
                var action3 = new Action { Id = 3, Sender = participantId };
                var blueprint = new Blueprint
                {
                    Participants = new List<Participant> { new Participant { Id = participantId, WalletAddress = walletAddress } },
                    Actions = new List<Action> { action, action2, action3 }
                };

                var result = _underTest.IsFinalAction(action, blueprint, jsonDoc, out var _);

                Assert.Equal(expected, result);
            }

            [Theory]
            [InlineData("Wheat", 2)]
            [InlineData("Linseed", -1)]
            public void Should_ReturnId_BasedOnCondition(string conditional, int expected)
            {
                Condition condition = new(); 
                var rule = "{\"if\":[{\"==\": [{\"var\": \"product\"}, \"" + conditional + "\"] }, 2,-1]}";
                ((List<string>)condition.Criteria).Add(rule);


				var data = new Dictionary<string, object>
				{
					{ "product", "Wheat" }
				};
				var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(data));

                var participantId = System.Guid.NewGuid().ToString();
                var walletAddress = "ws1walletaddress";
                var action = new Action { Id = 1, Condition = rule };
                var action2 = new Action { Id = 2, Sender = participantId };
                var action3 = new Action { Id = 3, Sender = participantId };
                var blueprint = new Blueprint
                {
                    Participants = new List<Participant> { new Participant { Id = participantId, WalletAddress = walletAddress } },
                    Actions = new List<Action> { action, action2, action3 }
                };

                _underTest.IsFinalAction(action, blueprint, jsonDoc, out var nextActionId);
                Assert.Equal(expected, nextActionId);
            }
        }
    }
}
