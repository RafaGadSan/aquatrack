using AquaTrack.Domain.Common;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;

namespace AquaTrack.Domain.Entities;

public class Alert : Entity
{
    public Guid FacilityId { get; private set; }
    public Guid EnvironmentalReadingId { get; private set; }
    public EnvironmentalParameter Parameter { get; private set; }
    public decimal Value { get; private set; }
    public decimal ThresholdMin { get; private set; }
    public decimal ThresholdMax { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private Alert()
    {
    }

    public Alert(
        Guid facilityId,
        Guid environmentalReadingId,
        EnvironmentalParameter parameter,
        decimal value,
        decimal thresholdMin,
        decimal thresholdMax)
    {
        FacilityId = facilityId;
        EnvironmentalReadingId = environmentalReadingId;
        Parameter = parameter;
        Value = value;
        ThresholdMin = thresholdMin;
        ThresholdMax = thresholdMax;
        Status = AlertStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        if (Status == AlertStatus.Resolved)
            throw new DomainException("La alerta ya está resuelta.");

        Status = AlertStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
