using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
