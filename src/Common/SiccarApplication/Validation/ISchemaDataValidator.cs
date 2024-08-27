#nullable enable

namespace Siccar.Application.Validation
{
    public interface ISchemaDataValidator
    {
        public (bool isValid, string validationMessage) ValidateSchemaData(string schema, string data);
    }
}
