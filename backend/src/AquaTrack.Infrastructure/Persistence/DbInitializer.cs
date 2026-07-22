using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Persistence;

/// <summary>
/// Seeds a handful of demo accounts (one per role) so the API/demo is explorable without a manual
/// sign-up step. Idempotent: no-ops once any user already exists.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(AquaTrackDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = new[]
        {
            new User("admin@aquatrack.dev", passwordHasher.Hash("Admin123!"), "Admin Demo", Role.Admin),
            new User("shiftlead@aquatrack.dev", passwordHasher.Hash("ShiftLead123!"), "Shift Lead Demo", Role.ShiftLead),
            new User("operator@aquatrack.dev", passwordHasher.Hash("Operator123!"), "Operator Demo", Role.Operator),
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
