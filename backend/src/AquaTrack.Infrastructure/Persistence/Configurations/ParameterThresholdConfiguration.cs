using AquaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaTrack.Infrastructure.Persistence.Configurations;

public class ParameterThresholdConfiguration : IEntityTypeConfiguration<ParameterThreshold>
{
    public void Configure(EntityTypeBuilder<ParameterThreshold> builder)
    {
        builder.ToTable("ParameterThresholds");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Parameter).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(t => t.MinValue).HasPrecision(6, 2).IsRequired();
        builder.Property(t => t.MaxValue).HasPrecision(6, 2).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();

        builder.HasOne<Facility>().WithMany().HasForeignKey(t => t.FacilityId).OnDelete(DeleteBehavior.Restrict);
    }
}
