using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
