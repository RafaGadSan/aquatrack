using AquaTrack.Application.DTOs;
using AquaTrack.Application.Validators;
using Xunit;

namespace AquaTrack.Application.Tests;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_Succeeds_ForValidRequest()
    {
        var result = _validator.Validate(new LoginRequest("rafael@example.com", "some-password"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("not-an-email", "password")]
    [InlineData("rafael@example.com", "")]
    public void Validate_Fails_ForInvalidRequest(string email, string password)
    {
        var result = _validator.Validate(new LoginRequest(email, password));

        Assert.False(result.IsValid);
    }
}
