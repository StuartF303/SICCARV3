using System;
using Xunit;
using Siccar.Application;
using Siccar.Application.Validation;

namespace SiccarApplicationTests
{

    /// <summary>
    /// Ensure the constraints are valid
    /// </summary>
    public class MappingTests
    {
        BlueprintValidator blueprintValidator;

        [Fact]
        public void BluePrint_Errors_Test()
        {
            blueprintValidator = new BlueprintValidator();
            var _underTest = ErrorRiddenBluePrint();

            var results = blueprintValidator.Validate(_underTest);

            // Now we should check for all the errors we made
            Assert.NotNull(_underTest);

        }

        public Blueprint ErrorRiddenBluePrint()
        {
            return new Blueprint() {
                
            };
        }

    }
}
