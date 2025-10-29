// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

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
    public class ControlValidationTests
    {
        ControlValidator controlValidator;
        Control ValidControl;
        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        TestData testData = new TestData();

        public ControlValidationTests()
        {
            controlValidator = new ControlValidator();
            string json = File.ReadAllText("Examples/ValidBlueprint.json");
            var blueprint = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            ValidControl = blueprint.Actions[0].Form;
        }

        [Fact]
        public void Should_Be_Valid()
        {
            var result = controlValidator.TestValidate(ValidControl);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_A_Scope(string scope)
        {
            ValidControl.Scope = scope;

            var result = controlValidator.TestValidate(ValidControl);

            result.ShouldHaveValidationErrorFor("Scope").WithErrorMessage("Control: Scope Required.");
        }

        [Fact]
        public void Basic_Control_Constraints()
        {
            var _control = new Siccar.Application.Control();

            var tst1 = controlValidator.Validate(_control);

            Assert.False(tst1.IsValid); // doesnt have enough detail

            // Add minimum requirements
            _control.Title = "Test Control";
            _control.Scope = "Scope_Path";

            var tst2 = controlValidator.Validate(_control);
            Assert.True(tst2.IsValid);

            // and confirm our test data
            Assert.True(controlValidator.Validate(testData.control1()).IsValid);
            Assert.False(controlValidator.Validate(testData.controlBad()).IsValid);
        }
    }
}
