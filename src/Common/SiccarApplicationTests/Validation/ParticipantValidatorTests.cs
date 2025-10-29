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
using FluentValidation;

namespace SiccarApplicationTests.Validation
{
    public class ParticipantValidatorTests
    {
        IValidator<Disclosure> disclosureValidator;

        TestData testData = new TestData();

        public ParticipantValidatorTests()
        {
            disclosureValidator = new DisclosureValidator();
        }

        [Fact]
        public void Basic_Disclosure_Constraints()
        {
            var _disclosure = new Disclosure();

            var tst1 = disclosureValidator.Validate(_disclosure);

            Assert.False(tst1.IsValid); // doesnt have enough detail

            // Add minimum requirements
            _disclosure.ParticipantAddress = "Test Control";
            _disclosure.DataPointers = new List<string>() { "#/" };

            var tst2 = disclosureValidator.Validate(_disclosure);
            Assert.True(tst2.IsValid);

            // and confirm our test data
            Assert.True(disclosureValidator.Validate(testData.disclosure1()).IsValid);
            Assert.False(disclosureValidator.Validate(testData.disclosureBad()).IsValid);
        }
    }
}
