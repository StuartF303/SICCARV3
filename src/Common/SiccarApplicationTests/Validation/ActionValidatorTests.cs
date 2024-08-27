/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using Siccar.Application.Validation;
using Siccar.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Action = Siccar.Application.Action;
using FluentValidation;
using System.IO;
using FluentValidation.TestHelper;

namespace SiccarApplicationTests.Validation
{
    public class ActionValidatorTests
    {
        ActionValidator actionValidator;
        private readonly Action ValidAction;
        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        TestData testData = new TestData();

        public ActionValidatorTests()
        {
            string json = File.ReadAllText("Examples/ValidBlueprint.json");
            var blueprint = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            ValidAction = blueprint.Actions[0];
            actionValidator = new ActionValidator();
        }

        [Fact]
        public void Should_Be_Valid()
        {
            var result = actionValidator.TestValidate(ValidAction);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_A_Title(string title)
        {
            ValidAction.Title = title;

            var result = actionValidator.TestValidate(ValidAction);

            result.ShouldHaveValidationErrorFor("Title").WithErrorMessage("Action: Requires a Title.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_A_BlueprintId(string id)
        {
            ValidAction.Blueprint = id;

            var result = actionValidator.TestValidate(ValidAction);

            result.ShouldHaveValidationErrorFor("Blueprint").WithErrorMessage("Action: Requires a Blueprint Id.");
        }

        [Fact]
        public void Should_Have_AtleastOneDataSchema()
        {
            ValidAction.DataSchemas = Enumerable.Empty<JsonDocument>();

            var result = actionValidator.TestValidate(ValidAction);

            result.ShouldHaveValidationErrorFor("DataSchemas").WithErrorMessage("Action: Requires at least one data schema.");
        }

        [Fact]
        public void Should_Have_AtleastOneDisclosure()
        {
            ValidAction.Disclosures = Enumerable.Empty<Disclosure>();

            var result = actionValidator.TestValidate(ValidAction);

            result.ShouldHaveValidationErrorFor("Disclosures").WithErrorMessage("Action: Require at least 1 Disclosure.");
        }

        [Fact]
        public void Basic_Action_Constraints()
        {
            Action _action = new Action();

            var tst1 = actionValidator.Validate(_action);
            Assert.False(tst1.IsValid);
            // we could check all the errors are correct but maybe later

            // Add Minimum Requirements
            _action.Id = 1;
            _action.Title = "A Test Action";
            _action.Description = "A test ActionDescription";
            _action.Sender = "previousAddress";
            _action.Blueprint = "blueprintTxId";
            var jsch = new List<JsonDocument>();
            jsch.Add(JsonDocument.Parse("{}"));
            _action.DataSchemas = jsch;
            _action.PreviousTxId = "previousTxId";
            _action.Participants = new List<Condition>();
            var listRule = new List<string>();
            listRule.Add(testData.trueRule);
            ((List<Condition>)_action.Participants).Add(new Condition("participant1", listRule));
            ((List<Condition>)_action.Participants).Add(new Condition("participant2", listRule));

            ((List<Disclosure>)_action.Disclosures).Add(new Disclosure("participant1", new List<string>()
            { "#/item1" }));

            ((List<Disclosure>)_action.Disclosures).Add(new Disclosure("participant2", new List<string>()
            { "#/item1" }));

            // try again
            var tst2 = actionValidator.Validate(_action);
            Assert.True(tst2.IsValid);

            // and confirm our test data
            Assert.True(actionValidator.Validate(testData.action1()).IsValid);
            Assert.False(actionValidator.Validate(testData.actionBad()).IsValid);
        }
    }
}
