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

namespace SiccarApplicationTests.Validation
{
    public class ConditionValidatorTests
    {
        ConditionValidator conditionValidator;

        TestData testData = new TestData();

        public ConditionValidatorTests()
        {
            conditionValidator = new ConditionValidator();
        }

        [Fact]
        public void Basic_Condition_Constraints()
        {
            var _condition = new Siccar.Application.Condition();

            var tst1 = conditionValidator.Validate(_condition);

            Assert.False(tst1.IsValid); // doesnt have enough detail

            // Add minimum requirements
            _condition.Prinicpal = "Test Control";
            _condition.Criteria = new List<string>() { testData.trueRule };

            var tst2 = conditionValidator.Validate(_condition);
            Assert.True(tst2.IsValid);

            // and confirm our test data
            Assert.True(conditionValidator.Validate(testData.condition1()).IsValid);
            Assert.False(conditionValidator.Validate(testData.conditionBad()).IsValid);
        }
    }
}
