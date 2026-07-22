using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AquaTrackDbContext _context;

    public UserRepository(AquaTrackDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }
}
