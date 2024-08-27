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

using System;
using System.Collections.Generic;
using Xunit;
using Siccar.Application;
using Siccar.Application.Validation;
using Action = Siccar.Application.Action;
using System.Text.Json;
using System.IO;
using Json.Pointer;
using FluentValidation;
using FluentValidation.TestHelper;
using System.Linq;

namespace SiccarApplicationTests.Validation
{

    public class BlueprintValidatorTests
    {
        IValidator<Blueprint> blueprintValidator;

        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
        private readonly Blueprint blueprint;

        TestData testData = new TestData();

        public BlueprintValidatorTests()
        {
            string json = File.ReadAllText("Examples/ValidBlueprint.json");
            blueprint = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            blueprintValidator = new BlueprintValidator();
        }

        [Fact]
        public void Should_Be_Valid()
        {
            var result = new BlueprintValidator().TestValidate(blueprint!);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Be_InValid_When_RecipientIdsNotInListOfParticipants()
        {
            blueprint.Participants[0].Id = Guid.NewGuid().ToString();

            var result = new BlueprintValidator().Validate(blueprint!);

            Assert.False(result.IsValid);
            Assert.Contains("Action sender or additional recipients could not be found in the list of Blueprint participants", result.Errors.First().ErrorMessage);
        }

        [Fact]
        public void Should_Be_InValid_WhenThereIsLessThanTwoParticipants()
        {
            blueprint.Participants = new List<Participant>() { new Participant() };

            var result = new BlueprintValidator().TestValidate(blueprint!, opt => opt.IncludeAllRuleSets());

            result.ShouldHaveValidationErrorFor(bp => bp.Participants)
                .WithErrorMessage("Blueprint: Requires >=2 Participants");
        }

        [Fact]
        public void Should_Be_InValid_WhenThereAreNoActions()
        {
            blueprint.Actions.Clear();

            var result = new BlueprintValidator().TestValidate(blueprint!, opt => opt.IncludeAllRuleSets());

            result.ShouldHaveValidationErrorFor(bp => bp.Actions)
                .WithErrorMessage("Blueprint: Requires >=1 Actions");
        }

        [Fact]
        public void Basic_Blueprint_Constraints()
        {
            Blueprint _blueprint = new Blueprint();

            var tst1 = blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish"));
            Assert.False(tst1.IsValid); // doesnt have enough detail

            // Add minimum requirements
            _blueprint.Participants.Add(testData.participant1());
            _blueprint.Participants.Add(testData.participant2());

            _blueprint.Actions.Add(testData.action1()); // action 1 has both participants

            var tst2 = blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish"));
            Assert.True(tst2.IsValid);

            // and confirm our test data
            Assert.True(blueprintValidator.Validate(testData.blueprint1(), options => options.IncludeRuleSets("Publish")).IsValid);
            Assert.False(blueprintValidator.Validate(testData.blueprintBad(), options => options.IncludeRuleSets("Publish")).IsValid);
        }


        [Fact]
        public void Full_Blueprint_Invalid_Participants()
        {
            Blueprint _blueprint = new Blueprint()
            {
                Id = "testBlueprintId",
                Title = "A Test Blueprint"
            };

            //var tst1 = blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish"));
            //Assert.False(tst1.IsValid); // doesnt have enough detail

            // Add minimum requirements
            _blueprint.Participants.Add(testData.participant1());
            _blueprint.Participants.Add(testData.participant2());
            _blueprint.Actions.Add(testData.action1());

            //var tst2 = blueprintValidator.Validate(_blueprint);
            //Assert.True(tst2.IsValid); // has enough detail

            _blueprint.Participants.Add(testData.participantBad());
            var tst3 = blueprintValidator.Validate(_blueprint, options => options.IncludeAllRuleSets());
            Assert.False(tst3.IsValid); // participant 3 has heaps of errors
        }

        [Fact]
        public void Full_Blueprint_Valid_Disclosures()
        {
            Blueprint _blueprint = new Blueprint()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Working",
                Description = "A Test Blueprint"
            };

            // Add minimum test requirements
            _blueprint.Participants.Add(testData.participant1());
            _blueprint.Participants.Add(testData.participant2());

            var tst1 = blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish"));
            Assert.NotEmpty(tst1.Errors); // we dont have a valid disclosure at this point

        }

        [Fact]
        public void Full_Blueprint_Invalid_Disclosures()
        {
            Blueprint _blueprint = new Blueprint();

            // Add minimum test requirements
            _blueprint.Participants.Add(testData.participant1());
            _blueprint.Participants.Add(testData.participant2());

            var errors = blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish")).Errors;
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Full_Blueprint_ActionRecipientsNotInParticipantList()
        {
            Blueprint _blueprint = new Blueprint();

            // Add minimum test requirements
            _blueprint.Participants.Add(testData.participant1());
            _blueprint.Participants.Add(testData.participant2());

            Assert.NotEmpty(blueprintValidator.Validate(_blueprint, options => options.IncludeRuleSets("Publish")).Errors);
        }

        [Fact]
        public void Should_BeInvalidWhenFieldInCalculation_NotFoundInCalculationSendersDisclosures()
        {
            blueprint.Actions[1].Disclosures = new List<Disclosure> { new Disclosure() { ParticipantAddress = blueprint.Participants[2].Id, DataPointers = new List<string> { "differentField" } } };

            var result = new BlueprintValidator().TestValidate(blueprint!, opt => opt.IncludeAllRuleSets());

            result.ShouldHaveValidationErrorFor(bp => bp.Actions)
                .WithErrorMessage("Calculations must have data submitted in the current action or a previous action that is disclosed to the calculation sender.");
        }

        [Theory]
        [InlineData(new object[] { "string" })]
        [InlineData(new object[] { "bool" })]
        public void Should_BeInvalidWhenFieldInCalculation_IsNotNumeric(string invalidFieldType)
        {
            var dataSchema = blueprint.Actions[1].DataSchemas.First().RootElement.GetRawText().Replace("integer", invalidFieldType);
            blueprint.Actions[1].DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) };

            var result = new BlueprintValidator().TestValidate(blueprint!, opt => opt.IncludeAllRuleSets());

            result.ShouldHaveValidationErrorFor(bp => bp.Actions)
                .WithErrorMessage("Calculations must have data fields which are of type integer or number.");
        }

        [Theory]
        [InlineData(new object[] { "integer" })]
        [InlineData(new object[] { "number" })]
        public void Should_BeValidWhenFieldInCalculation_IsNumeric(string validFieldType)
        {
            var dataSchema = blueprint.Actions[1].DataSchemas.First().RootElement.GetRawText().Replace("integer", validFieldType);
            blueprint.Actions[1].DataSchemas = new List<JsonDocument> { JsonDocument.Parse(dataSchema) };

            var result = new BlueprintValidator().TestValidate(blueprint!, opt => opt.IncludeAllRuleSets());

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
