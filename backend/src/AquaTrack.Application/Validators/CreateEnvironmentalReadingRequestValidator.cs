using AquaTrack.Application.DTOs;
using FluentValidation;

namespace AquaTrack.Application.Validators;

public class CreateEnvironmentalReadingRequestValidator : AbstractValidator<CreateEnvironmentalReadingRequest>
{
    public CreateEnvironmentalReadingRequestValidator()
    {
        RuleFor(x => x.DissolvedOxygen).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Salinity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PH).InclusiveBetween(0, 14);
    }
}
