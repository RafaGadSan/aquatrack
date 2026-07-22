using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;

namespace AquaTrack.Domain.Services;

/// <summary>
/// Compares a reading's values against the applicable thresholds and raises an <see cref="Alert"/>
/// for each parameter that falls outside its range. A facility-specific threshold takes precedence
/// over the global default for the same parameter; a parameter with no configured threshold at all
/// never raises an alert.
/// </summary>
public static class AlertEvaluator
{
    public static IReadOnlyList<Alert> Evaluate(EnvironmentalReading reading, IEnumerable<ParameterThreshold> thresholds)
    {
        ArgumentNullException.ThrowIfNull(reading);

        var thresholdList = thresholds as IReadOnlyCollection<ParameterThreshold> ?? thresholds.ToList();
        var alerts = new List<Alert>();

        foreach (var (parameter, value) in reading.Values)
        {
            var threshold = ResolveThreshold(thresholdList, reading.FacilityId, parameter);
            if (threshold is not null && threshold.IsBreachedBy(value))
            {
                alerts.Add(new Alert(reading.FacilityId, reading.Id, parameter, value, threshold.MinValue, threshold.MaxValue));
            }
        }

        return alerts;
    }

    private static ParameterThreshold? ResolveThreshold(
        IReadOnlyCollection<ParameterThreshold> thresholds,
        Guid facilityId,
        EnvironmentalParameter parameter)
    {
        return thresholds.FirstOrDefault(t => t.Parameter == parameter && t.FacilityId == facilityId)
            ?? thresholds.FirstOrDefault(t => t.Parameter == parameter && t.FacilityId == null);
    }
}
