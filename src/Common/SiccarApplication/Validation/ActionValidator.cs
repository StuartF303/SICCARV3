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