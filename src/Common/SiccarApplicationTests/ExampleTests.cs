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
using System.IO;
using Siccar.Application.Validation;
using Siccar.Application;
using System.Text.Json;
#nullable enable

namespace SiccarApplicationTests
{
    public class ExampleTests
    {
        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        [Fact]
        public void HeroDemo_Cereals_Test()
        {
            string json = File.ReadAllText("Examples/HeroDemo.json");

            var t = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            var test = new BlueprintValidator().Validate(t!);
             
            foreach(var e in test.Errors)
            {
                Console.WriteLine($" error: {e.ErrorMessage}");
            }

            Assert.True(test.IsValid);
        }

        [Fact]
        public void SimpleBP_Test()
        {
            string json = File.ReadAllText("Examples/SimpleBlueprint.json");

            var t = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            var test = new BlueprintValidator().Validate(t!);

            foreach (var e in test.Errors)
            {
                Console.WriteLine($" error: {e.ErrorMessage}");
            }

            Assert.True(test.IsValid);
        }
    }
}
