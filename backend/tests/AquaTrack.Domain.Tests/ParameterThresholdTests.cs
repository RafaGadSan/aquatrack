using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class ParameterThresholdTests
{
    [Fact]
    public void Constructor_Throws_WhenMinIsGreaterThanMax()
    {
        Assert.Throws<DomainException>(() => new ParameterThreshold(EnvironmentalParameter.PH, 8, 6));
    }

    // decimal isn't a valid [InlineData] argument type in C#, so boundary cases are built at
    // runtime via MemberData instead of attribute constants.
    public static IEnumerable<object[]> BoundaryCases()
    {
        yield return new object[] { 10m, false };
        yield return new object[] { 22m, false };
        yield return new object[] { 15m, false };
        yield return new object[] { 9.99m, true };
        yield return new object[] { 22.01m, true };
    }

    [Theory]
    [MemberData(nameof(BoundaryCases))]
    public void IsBreachedBy_TreatsBoundsAsInclusive(decimal value, bool expectedBreach)
    {
        var threshold = new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22);

        Assert.Equal(expectedBreach, threshold.IsBreachedBy(value));
    }

    [Fact]
    public void UpdateRange_Throws_WhenMinIsGreaterThanMax()
    {
        var threshold = new ParameterThreshold(EnvironmentalParameter.Salinity, 25, 35);

        Assert.Throws<DomainException>(() => threshold.UpdateRange(40, 30));
    }
}
