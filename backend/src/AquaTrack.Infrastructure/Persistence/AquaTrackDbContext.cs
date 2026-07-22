using AquaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Persistence;

public class AquaTrackDbContext : DbContext
{
    public AquaTrackDbContext(DbContextOptions<AquaTrackDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<EnvironmentalReading> EnvironmentalReadings => Set<EnvironmentalReading>();
    public DbSet<ParameterThreshold> ParameterThresholds => Set<ParameterThreshold>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AquaTrackDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
