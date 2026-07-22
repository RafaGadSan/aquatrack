using AquaTrack.Domain.Common;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;

namespace AquaTrack.Domain.Entities;

public class EnvironmentalReading : Entity
{
    public Guid FacilityId { get; private set; }
    public Guid RecordedByUserId { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public decimal Temperature { get; private set; }
    public decimal DissolvedOxygen { get; private set; }
    public decimal Salinity { get; private set; }
    public decimal PH { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private EnvironmentalReading()
    {
    }

    public EnvironmentalReading(
        Guid facilityId,
        Guid recordedByUserId,
        DateTime recordedAt,
        decimal temperature,
        decimal dissolvedOxygen,
        decimal salinity,
        decimal ph)
    {
        if (facilityId == Guid.Empty)
            throw new DomainException("A reading must belong to a facility.");
        if (recordedByUserId == Guid.Empty)
            throw new DomainException("A reading must be attributed to a user.");
        if (dissolvedOxygen < 0)
            throw new DomainException("Dissolved oxygen cannot be negative.");
        if (salinity < 0)
            throw new DomainException("Salinity cannot be negative.");
        if (ph is < 0 or > 14)
            throw new DomainException("pH must be between 0 and 14.");

        FacilityId = facilityId;
        RecordedByUserId = recordedByUserId;
        RecordedAt = recordedAt;
        Temperature = temperature;
        DissolvedOxygen = dissolvedOxygen;
        Salinity = salinity;
        PH = ph;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Per-parameter view used by <c>AlertEvaluator</c> to check each value against its threshold generically.</summary>
    public IReadOnlyDictionary<EnvironmentalParameter, decimal> Values => new Dictionary<EnvironmentalParameter, decimal>
    {
        [EnvironmentalParameter.Temperature] = Temperature,
        [EnvironmentalParameter.DissolvedOxygen] = DissolvedOxygen,
        [EnvironmentalParameter.Salinity] = Salinity,
        [EnvironmentalParameter.PH] = PH,
    };
}
