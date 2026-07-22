using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;

namespace AquaTrack.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        var token = _tokenGenerator.GenerateToken(user);

        var response = new AuthResponse(token.Token, token.ExpiresAt, user.Id, user.Email, user.FullName, user.Role);
        return Result<AuthResponse>.Success(response);
    }
}
