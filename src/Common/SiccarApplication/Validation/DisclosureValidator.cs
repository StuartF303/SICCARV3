// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    public class DisclosureValidator : AbstractValidator<Disclosure>
    {
        public DisclosureValidator()
        {
            RuleFor(c => c.ParticipantAddress).NotEmpty().WithMessage("Disclosure: Requires Participant Address");
            RuleFor(c => c.ParticipantAddress).MaximumLength(64).WithMessage("Disclosure: Max Length 64.");
            RuleFor(c => c.DataPointers).NotEmpty().WithMessage("Disclosure: Data Pointers Required.");
        }
    }
}