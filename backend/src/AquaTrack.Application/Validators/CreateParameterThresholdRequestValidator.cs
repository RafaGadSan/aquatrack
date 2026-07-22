using AquaTrack.Application.DTOs;
using FluentValidation;

namespace AquaTrack.Application.Validators;

public class CreateParameterThresholdRequestValidator : AbstractValidator<CreateParameterThresholdRequest>
{
    public CreateParameterThresholdRequestValidator()
    {
        RuleFor(x => x.Parameter).IsInEnum().WithName("Parámetro");
        RuleFor(x => x.MaxValue).GreaterThanOrEqualTo(x => x.MinValue)
            .WithMessage("El valor máximo debe ser mayor o igual al valor mínimo.");
    }
}
