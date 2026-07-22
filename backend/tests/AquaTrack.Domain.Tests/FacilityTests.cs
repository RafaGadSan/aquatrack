using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class FacilityTests
{
    [Fact]
    public void Constructor_StartsInEmptyStatus()
    {
        var facility = new Facility("Cage 3", FacilityType.Cage);

        Assert.Equal(FacilityStatus.Empty, facility.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenNameIsMissing(string name)
    {
        Assert.Throws<DomainException>(() => new Facility(name, FacilityType.Tank));
    }

    [Fact]
    public void ChangeStatus_UpdatesStatusAndUpdatedAt()
    {
        var facility = new Facility("Tank 1", FacilityType.Tank);
        var updatedAtBeforeChange = facility.UpdatedAt;

        facility.ChangeStatus(FacilityStatus.Active);

        Assert.Equal(FacilityStatus.Active, facility.Status);
        Assert.True(facility.UpdatedAt >= updatedAtBeforeChange);
    }
}
