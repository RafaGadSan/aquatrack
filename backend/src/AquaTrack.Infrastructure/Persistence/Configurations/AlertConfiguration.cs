using AquaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaTrack.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FacilityId).IsRequired();
        builder.Property(a => a.EnvironmentalReadingId).IsRequired();
        builder.Property(a => a.Parameter).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(a => a.Value).HasPrecision(6, 2).IsRequired();
        builder.Property(a => a.ThresholdMin).HasPrecision(6, 2).IsRequired();
        builder.Property(a => a.ThresholdMax).HasPrecision(6, 2).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasIndex(a => a.FacilityId);

        builder.HasOne<Facility>().WithMany().HasForeignKey(a => a.FacilityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<EnvironmentalReading>().WithMany().HasForeignKey(a => a.EnvironmentalReadingId).OnDelete(DeleteBehavior.Restrict);
    }
}
