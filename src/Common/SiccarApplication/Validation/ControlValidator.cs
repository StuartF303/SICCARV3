// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    public class ControlValidator : AbstractValidator<Control>
    {
        public ControlValidator()
        {
            RuleFor(c => c.Title).MaximumLength(100).WithMessage("Control: Title Max Length 100.");
            RuleFor(c => c.Scope).NotEmpty().WithMessage("Control: Scope Required.");
            RuleFor(c => c.Scope).MaximumLength(250).WithMessage("Control: Scope Max Length 250.");
            RuleFor(c => c.ControlType).IsInEnum().WithMessage("Control: Only valid Control Types allowed.");
            RuleFor(c => c.Layout).IsInEnum().WithMessage("Control: Only valid Layouts allowed.");
        }
    }
}