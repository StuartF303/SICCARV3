// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    /// <summary>
    /// Am not sure about this yet - think a simple just be to try to parse the Rule
    /// aint going to cut it because we have already loaded it from JSON to a Rule
    /// </summary>
    public class RuleValidator : AbstractValidator<string>
    {
        public RuleValidator()
        {
            // so for the Rule Validaotr we will need to try to convert and make sure it doesnt throw
           
        }
    }
}
