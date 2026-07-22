using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class AlertTests
{
    private static Alert CreateAlert() =>
        new(Guid.NewGuid(), Guid.NewGuid(), EnvironmentalParameter.Temperature, 28m, 10m, 22m);

    [Fact]
    public void Constructor_StartsActiveWithNoResolvedAt()
    {
        var alert = CreateAlert();

        Assert.Equal(AlertStatus.Active, alert.Status);
        Assert.Null(alert.ResolvedAt);
    }

    [Fact]
    public void Resolve_SetsStatusAndResolvedAt()
    {
        var alert = CreateAlert();

        alert.Resolve();

        Assert.Equal(AlertStatus.Resolved, alert.Status);
        Assert.NotNull(alert.ResolvedAt);
    }

    [Fact]
    public void Resolve_Throws_WhenAlreadyResolved()
    {
        var alert = CreateAlert();
        alert.Resolve();

        Assert.Throws<DomainException>(() => alert.Resolve());
    }
}
