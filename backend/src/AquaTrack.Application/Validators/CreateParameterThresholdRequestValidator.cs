using AquaTrack.Application.DTOs;
using FluentValidation;

namespace AquaTrack.Application.Validators;

public class CreateParameterThresholdRequestValidator : AbstractValidator<CreateParameterThresholdRequest>
{
    public CreateParameterThresholdRequestValidator()
    {
        RuleFor(x => x.Parameter).IsInEnum();
        RuleFor(x => x.MaxValue).GreaterThanOrEqualTo(x => x.MinValue)
            .WithMessage("MaxValue must be greater than or equal to MinValue.");
    }
}
