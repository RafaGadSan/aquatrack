using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _tokenGenerator = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository.Object, _passwordHasher.Object, _tokenGenerator.Object);
    }

    private static User CreateUser(bool isActive = true)
    {
        var user = new User("rafael@example.com", "hashed-password", "Rafael Gadea", Role.ShiftLead);
        if (!isActive) user.Deactivate();
        return user;
    }

    [Fact]
    public async Task LoginAsync_ReturnsSuccessWithToken_WhenCredentialsAreValid()
    {
        var user = CreateUser();
        var request = new LoginRequest(user.Email, "correct-password");
        var generatedToken = new GeneratedToken("jwt-token", DateTime.UtcNow.AddHours(8));

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);
        _tokenGenerator.Setup(g => g.GenerateToken(user)).Returns(generatedToken);

        var result = await _sut.LoginAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(generatedToken.Token, result.Value!.Token);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal(user.Role, result.Value.Role);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_WhenUserDoesNotExist()
    {
        var request = new LoginRequest("missing@example.com", "whatever");
        _userRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(request);

        Assert.False(result.IsSuccess);
        _tokenGenerator.Verify(g => g.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_WhenPasswordIsWrong()
    {
        var user = CreateUser();
        var request = new LoginRequest(user.Email, "wrong-password");

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(false);

        var result = await _sut.LoginAsync(request);

        Assert.False(result.IsSuccess);
        _tokenGenerator.Verify(g => g.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_WhenUserIsInactive()
    {
        var user = CreateUser(isActive: false);
        var request = new LoginRequest(user.Email, "correct-password");

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);

        var result = await _sut.LoginAsync(request);

        Assert.False(result.IsSuccess);
        _tokenGenerator.Verify(g => g.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}
