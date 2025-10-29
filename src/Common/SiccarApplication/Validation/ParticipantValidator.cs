// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    public class ParticipantValidator : AbstractValidator<Participant>
    {
        public ParticipantValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("Participant: Id Required.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Participant: Name Required.");
            RuleFor(p => p.WalletAddress).NotEmpty().WithMessage("Participant: WalletAddress Required.");
        }
    }
}
