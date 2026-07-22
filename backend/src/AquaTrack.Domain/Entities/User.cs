using AquaTrack.Domain.Common;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;

namespace AquaTrack.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public Role Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public User(string email, string passwordHash, string fullName, Role role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El correo electrónico es obligatorio.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("El hash de la contraseña es obligatorio.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo es obligatorio.");

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public void ChangeRole(Role role) => Role = role;
}
