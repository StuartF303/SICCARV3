/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using FluentValidation;
#nullable enable

namespace Siccar.Application.Validation
{
    public class ActionValidator : AbstractValidator<Action>
    {
        public ActionValidator()
        {
            RuleFor(a => a.Id).NotEmpty().WithMessage("Action: Requires Id.");
            RuleFor(a => a.Title).NotEmpty().WithMessage("Action: Requires a Title."); 
            RuleFor(a => a.Blueprint).NotEmpty().WithMessage("Action: Requires a Blueprint Id.");
            RuleFor(a => a.DataSchemas).NotEmpty().WithMessage("Action: Requires at least one data schema.");
            RuleFor(a => a.Disclosures).NotEmpty().WithMessage("Action: Require at least 1 Disclosure.");
            RuleFor(a => a.Disclosures).ForEach(p => p.SetValidator(new DisclosureValidator()));
        }
    }
}