using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.DTOs;

public record AuthResponse(string Token, DateTime ExpiresAt, Guid UserId, string Email, string FullName, Role Role);
