using AquaTrack.Application.DTOs;
using FluentValidation;

namespace AquaTrack.Application.Validators;

public class CreateFacilityRequestValidator : AbstractValidator<CreateFacilityRequest>
{
    public CreateFacilityRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithName("Nombre");
        RuleFor(x => x.Type).IsInEnum().WithName("Tipo");
        RuleFor(x => x.Location).MaximumLength(200).WithName("Ubicación");
    }
}
