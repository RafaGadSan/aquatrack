using AquaTrack.Domain.Common;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;

namespace AquaTrack.Domain.Entities;

/// <summary>
/// Acceptable range for a given parameter. A null <see cref="FacilityId"/> is a global default;
/// a facility-specific threshold overrides the default for that facility (see AlertEvaluator).
/// </summary>
public class ParameterThreshold : Entity
{
    public Guid? FacilityId { get; private set; }
    public EnvironmentalParameter Parameter { get; private set; }
    public decimal MinValue { get; private set; }
    public decimal MaxValue { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ParameterThreshold()
    {
    }

    public ParameterThreshold(EnvironmentalParameter parameter, decimal minValue, decimal maxValue, Guid? facilityId = null)
    {
        if (minValue > maxValue)
            throw new DomainException("Minimum value cannot be greater than maximum value.");

        Parameter = parameter;
        MinValue = minValue;
        MaxValue = maxValue;
        FacilityId = facilityId;

        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public bool IsBreachedBy(decimal value) => value < MinValue || value > MaxValue;

    public void UpdateRange(decimal minValue, decimal maxValue)
    {
        if (minValue > maxValue)
            throw new DomainException("Minimum value cannot be greater than maximum value.");

        MinValue = minValue;
        MaxValue = maxValue;
        UpdatedAt = DateTime.UtcNow;
    }
}
