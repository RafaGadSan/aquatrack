using AquaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Persistence;

public class AquaTrackDbContext : DbContext
{
    public AquaTrackDbContext(DbContextOptions<AquaTrackDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AquaTrackDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
