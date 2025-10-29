// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

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
