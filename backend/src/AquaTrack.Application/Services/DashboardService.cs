using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IAlertRepository _alertRepository;

    public DashboardService(IFacilityRepository facilityRepository, IAlertRepository alertRepository)
    {
        _facilityRepository = facilityRepository;
        _alertRepository = alertRepository;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.GetAllAsync(cancellationToken);
        var activeAlerts = await _alertRepository.GetActiveAsync(cancellationToken);

        var facilityNamesById = facilities.ToDictionary(f => f.Id, f => f.Name);

        var facilitiesByStatus = Enum.GetValues<FacilityStatus>()
            .Select(status => new FacilityStatusCount(status, facilities.Count(f => f.Status == status)))
            .ToList();

        var activeAlertResponses = activeAlerts
            .Select(alert => new DashboardAlertResponse(
                alert.Id,
                alert.FacilityId,
                facilityNamesById.GetValueOrDefault(alert.FacilityId, "Unknown facility"),
                alert.Parameter,
                alert.Value,
                alert.ThresholdMin,
                alert.ThresholdMax,
                alert.CreatedAt))
            .ToList();

        return new DashboardSummaryResponse(facilities.Count, facilitiesByStatus, activeAlertResponses.Count, activeAlertResponses);
    }
}
