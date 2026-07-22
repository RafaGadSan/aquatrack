using AquaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaTrack.Infrastructure.Persistence.Configurations;

public class EnvironmentalReadingConfiguration : IEntityTypeConfiguration<EnvironmentalReading>
{
    public void Configure(EntityTypeBuilder<EnvironmentalReading> builder)
    {
        builder.ToTable("EnvironmentalReadings");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FacilityId).IsRequired();
        builder.Property(r => r.RecordedByUserId).IsRequired();
        builder.Property(r => r.RecordedAt).IsRequired();
        builder.Property(r => r.Temperature).HasPrecision(6, 2).IsRequired();
        builder.Property(r => r.DissolvedOxygen).HasPrecision(6, 2).IsRequired();
        builder.Property(r => r.Salinity).HasPrecision(6, 2).IsRequired();
        builder.Property(r => r.PH).HasPrecision(4, 2).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasIndex(r => r.FacilityId);

        // FK-only association: EF sees these as related via FacilityId/RecordedByUserId, but the
        // domain entities don't hold navigation properties to each other (kept decoupled on purpose).
        builder.HasOne<Facility>().WithMany().HasForeignKey(r => r.FacilityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(r => r.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
