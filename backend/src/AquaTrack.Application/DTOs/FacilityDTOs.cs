using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.DTOs;

public record CreateFacilityRequest(string Name, FacilityType Type, string? Location);

public record UpdateFacilityStatusRequest(FacilityStatus Status);

public record FacilityResponse(
    Guid Id,
    string Name,
    FacilityType Type,
    FacilityStatus Status,
    string? Location,
    DateTime CreatedAt,
    DateTime UpdatedAt);
