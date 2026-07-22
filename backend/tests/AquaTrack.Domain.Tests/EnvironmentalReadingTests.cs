using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class EnvironmentalReadingTests
{
    private static readonly Guid FacilityId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Constructor_Throws_WhenFacilityIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new EnvironmentalReading(Guid.Empty, UserId, DateTime.UtcNow, 18, 7, 30, 7.5m));
    }

    [Fact]
    public void Constructor_Throws_WhenPhIsOutOfMeasurableScale()
    {
        Assert.Throws<DomainException>(() =>
            new EnvironmentalReading(FacilityId, UserId, DateTime.UtcNow, 18, 7, 30, 15m));
    }

    [Fact]
    public void Constructor_Throws_WhenDissolvedOxygenIsNegative()
    {
        Assert.Throws<DomainException>(() =>
            new EnvironmentalReading(FacilityId, UserId, DateTime.UtcNow, 18, -1, 30, 7.5m));
    }

    [Fact]
    public void Constructor_AllowsOutOfRangeButBiologicallyMeasurableValues()
    {
        // A dangerously high temperature is still a valid *measurement* — it's ParameterThreshold's
        // job to flag it as an alert, not the entity's job to reject it.
        var reading = new EnvironmentalReading(FacilityId, UserId, DateTime.UtcNow, 40, 7, 30, 7.5m);

        Assert.Equal(40m, reading.Temperature);
    }

    [Fact]
    public void Values_MapsEachPropertyToItsParameter()
    {
        var reading = new EnvironmentalReading(FacilityId, UserId, DateTime.UtcNow, 18, 7, 30, 7.5m);

        Assert.Equal(18m, reading.Values[EnvironmentalParameter.Temperature]);
        Assert.Equal(7m, reading.Values[EnvironmentalParameter.DissolvedOxygen]);
        Assert.Equal(30m, reading.Values[EnvironmentalParameter.Salinity]);
        Assert.Equal(7.5m, reading.Values[EnvironmentalParameter.PH]);
    }
}
