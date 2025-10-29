// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    public class ConditionValidator : AbstractValidator<Condition>
    {
        public ConditionValidator()
        {
            RuleFor(c => c.Prinicpal).NotEmpty().WithMessage("Condition: Principal Required.");
            RuleFor(c => c.Prinicpal).MaximumLength(2048).WithMessage("Condition: Max Length 2048.");
            RuleFor(c => c.Criteria).NotEmpty().WithMessage("Condition: Criteria Required.");
         }
    }
}