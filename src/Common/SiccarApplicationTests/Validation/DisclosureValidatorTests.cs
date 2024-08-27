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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using System.IO;
using FluentValidation.TestHelper;

namespace SiccarApplicationTests.Validation
{
    public class DisclosureValidatorTests
    {
        DisclosureValidator disclosureValidator;

        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
        private readonly Disclosure ValidDisclosure;

        TestData testData = new TestData();

        public DisclosureValidatorTests()
        {
            disclosureValidator = new DisclosureValidator();
            string json = File.ReadAllText("Examples/ValidBlueprint.json");
            var blueprint = JsonSerializer.Deserialize<Blueprint>(json, serializerOptions);
            ValidDisclosure = blueprint.Actions[0].Disclosures.First();
        }

        [Fact]
        public void Should_Be_Valid()
        {
            var result = disclosureValidator.TestValidate(ValidDisclosure);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_A_Title(string address)
        {
            ValidDisclosure.ParticipantAddress = address;

            var result = disclosureValidator.TestValidate(ValidDisclosure);

            result.ShouldHaveValidationErrorFor("ParticipantAddress").WithErrorMessage("Disclosure: Requires Participant Address");
        }

        [Fact]
        public void Should_Have_Atleast_One_DataPointer()
        {
            ValidDisclosure.DataPointers.Clear();

            var result = disclosureValidator.TestValidate(ValidDisclosure);

            result.ShouldHaveValidationErrorFor("DataPointers").WithErrorMessage("Disclosure: Data Pointers Required.");
        }

        [Fact]
        public void Should_Have_AddressWithLessThan64Characters()
        {
            ValidDisclosure.ParticipantAddress = "jkyEypeeTERN1FzmQ7fwaFNTy4Nt3kHuBK8UaydBzYTDgIFf8RAr6UprS6VeaVlta";

            var result = disclosureValidator.TestValidate(ValidDisclosure);

            result.ShouldHaveValidationErrorFor("ParticipantAddress").WithErrorMessage("Disclosure: Max Length 64.");
        }
    }
}
