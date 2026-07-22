namespace AquaTrack.Application.DTOs;

public record CreateEnvironmentalReadingRequest(decimal Temperature, decimal DissolvedOxygen, decimal Salinity, decimal PH);

public record EnvironmentalReadingResponse(
    Guid Id,
    Guid FacilityId,
    Guid RecordedByUserId,
    DateTime RecordedAt,
    decimal Temperature,
    decimal DissolvedOxygen,
    decimal Salinity,
    decimal PH);

public record RecordReadingResult(EnvironmentalReadingResponse Reading, IReadOnlyList<AlertResponse> TriggeredAlerts);
