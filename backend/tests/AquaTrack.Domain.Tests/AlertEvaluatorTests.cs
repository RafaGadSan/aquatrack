using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Services;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class AlertEvaluatorTests
{
    private static readonly Guid FacilityId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private static EnvironmentalReading CreateReading(
        decimal temperature = 18m,
        decimal dissolvedOxygen = 7m,
        decimal salinity = 30m,
        decimal ph = 7.5m)
    {
        return new EnvironmentalReading(FacilityId, UserId, DateTime.UtcNow, temperature, dissolvedOxygen, salinity, ph);
    }

    [Fact]
    public void Evaluate_ReturnsNoAlerts_WhenAllValuesAreWithinGlobalThresholds()
    {
        var reading = CreateReading();
        var thresholds = new[]
        {
            new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22),
            new ParameterThreshold(EnvironmentalParameter.DissolvedOxygen, 5, 12),
            new ParameterThreshold(EnvironmentalParameter.Salinity, 25, 35),
            new ParameterThreshold(EnvironmentalParameter.PH, 6.5m, 8.5m),
        };

        var alerts = AlertEvaluator.Evaluate(reading, thresholds);

        Assert.Empty(alerts);
    }

    [Fact]
    public void Evaluate_RaisesAlert_WhenValueBreachesGlobalThreshold()
    {
        var reading = CreateReading(temperature: 28m);
        var thresholds = new[] { new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22) };

        var alerts = AlertEvaluator.Evaluate(reading, thresholds);

        var alert = Assert.Single(alerts);
        Assert.Equal(EnvironmentalParameter.Temperature, alert.Parameter);
        Assert.Equal(28m, alert.Value);
        Assert.Equal(10m, alert.ThresholdMin);
        Assert.Equal(22m, alert.ThresholdMax);
        Assert.Equal(reading.FacilityId, alert.FacilityId);
        Assert.Equal(reading.Id, alert.EnvironmentalReadingId);
        Assert.Equal(AlertStatus.Active, alert.Status);
    }

    [Fact]
    public void Evaluate_PrefersFacilitySpecificThreshold_OverGlobalDefault()
    {
        // Global default flags 28 as too hot, but this facility runs warmer water on purpose.
        var reading = CreateReading(temperature: 28m);
        var thresholds = new[]
        {
            new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22),
            new ParameterThreshold(EnvironmentalParameter.Temperature, 20, 30, facilityId: FacilityId),
        };

        var alerts = AlertEvaluator.Evaluate(reading, thresholds);

        Assert.Empty(alerts);
    }

    [Fact]
    public void Evaluate_RaisesNoAlert_WhenParameterHasNoConfiguredThreshold()
    {
        var reading = CreateReading(salinity: 999m);

        var alerts = AlertEvaluator.Evaluate(reading, Array.Empty<ParameterThreshold>());

        Assert.Empty(alerts);
    }

    [Fact]
    public void Evaluate_RaisesOneAlertPerBreachedParameter()
    {
        var reading = CreateReading(temperature: 28m, ph: 9.5m);
        var thresholds = new[]
        {
            new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22),
            new ParameterThreshold(EnvironmentalParameter.DissolvedOxygen, 5, 12),
            new ParameterThreshold(EnvironmentalParameter.Salinity, 25, 35),
            new ParameterThreshold(EnvironmentalParameter.PH, 6.5m, 8.5m),
        };

        var alerts = AlertEvaluator.Evaluate(reading, thresholds);

        Assert.Equal(2, alerts.Count);
        Assert.Contains(alerts, a => a.Parameter == EnvironmentalParameter.Temperature);
        Assert.Contains(alerts, a => a.Parameter == EnvironmentalParameter.PH);
    }
}
