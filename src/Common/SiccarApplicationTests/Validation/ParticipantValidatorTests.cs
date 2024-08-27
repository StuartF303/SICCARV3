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
