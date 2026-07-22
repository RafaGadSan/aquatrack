using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.DTOs;

public record AlertResponse(
    Guid Id,
    Guid FacilityId,
    Guid EnvironmentalReadingId,
    EnvironmentalParameter Parameter,
    decimal Value,
    decimal ThresholdMin,
    decimal ThresholdMax,
    AlertStatus Status,
    DateTime CreatedAt,
    DateTime? ResolvedAt);
