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
