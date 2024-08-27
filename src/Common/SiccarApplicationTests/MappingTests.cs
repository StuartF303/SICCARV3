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
