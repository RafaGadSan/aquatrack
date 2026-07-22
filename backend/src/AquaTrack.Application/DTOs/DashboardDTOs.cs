using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.DTOs;

public record FacilityStatusCount(FacilityStatus Status, int Count);

public record DashboardAlertResponse(
    Guid Id,
    Guid FacilityId,
    string FacilityName,
    EnvironmentalParameter Parameter,
    decimal Value,
    decimal ThresholdMin,
    decimal ThresholdMax,
    DateTime CreatedAt);

public record DashboardSummaryResponse(
    int TotalFacilities,
    IReadOnlyList<FacilityStatusCount> FacilitiesByStatus,
    int ActiveAlertsCount,
    IReadOnlyList<DashboardAlertResponse> ActiveAlerts);
