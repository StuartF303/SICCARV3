// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

#nullable enable

namespace Siccar.Application.Validation
{
    public interface ISchemaDataValidator
    {
        public (bool isValid, string validationMessage) ValidateSchemaData(string schema, string data);
    }
}
