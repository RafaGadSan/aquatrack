using AquaTrack.Application.DTOs;
using FluentValidation;

namespace AquaTrack.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithName("Correo electrónico");
        RuleFor(x => x.Password).NotEmpty().WithName("Contraseña");
    }
}
