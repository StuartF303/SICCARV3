
using Siccar.Application;
using Siccar.UI.Admin.Models;
using Xunit;

namespace AdminUiTest.Models
{
    public class DataSchemaPropertyTest
    {
        private DataSchemaProperty _underTest;

        public DataSchemaPropertyTest()
        {
            _underTest = new();
        }

        [Theory]
        [InlineData("string", ControlTypes.TextLine)]
        [InlineData("string", ControlTypes.Choice)]
        [InlineData("string", ControlTypes.TextArea)]
        [InlineData("boolean", ControlTypes.Checkbox)]
        [InlineData("string", ControlTypes.DateTime)]
        [InlineData("integer", ControlTypes.Numeric)]
        [InlineData("object", ControlTypes.File)]
        [InlineData("string", ControlTypes.Selection)]
        [InlineData("", ControlTypes.Layout)]
        [InlineData("string", ControlTypes.Label)]
        public void ShouldMapTypeCorrectly(string expected, ControlTypes controlType)
        {
            _underTest.Type = controlType.ToString();

            Assert.Equal(expected, _underTest.PropertyType);
        }
    }
}
