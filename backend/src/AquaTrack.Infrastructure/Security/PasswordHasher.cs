using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AquaTrack.Infrastructure.Security;

/// <summary>
/// Thin wrapper around ASP.NET Core Identity's battle-tested PBKDF2 hasher. The <c>User</c>
/// argument PasswordHasher&lt;TUser&gt; asks for is a generic type marker only — its default
/// implementation never inspects it, so passing null is the documented, safe idiom here.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}
