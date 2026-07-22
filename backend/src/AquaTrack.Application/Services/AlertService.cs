using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IFacilityRepository _facilityRepository;

    public AlertService(IAlertRepository alertRepository, IFacilityRepository facilityRepository)
    {
        _alertRepository = alertRepository;
        _facilityRepository = facilityRepository;
    }

    public async Task<Result<IReadOnlyList<AlertResponse>>> GetByFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        if (await _facilityRepository.GetByIdAsync(facilityId, cancellationToken) is null)
        {
            return Result<IReadOnlyList<AlertResponse>>.Failure("Facility not found.");
        }

        var alerts = await _alertRepository.GetByFacilityIdAsync(facilityId, cancellationToken);
        return Result<IReadOnlyList<AlertResponse>>.Success(alerts.Select(ToResponse).ToList());
    }

    private static AlertResponse ToResponse(Alert alert) => new(
        alert.Id,
        alert.FacilityId,
        alert.EnvironmentalReadingId,
        alert.Parameter,
        alert.Value,
        alert.ThresholdMin,
        alert.ThresholdMax,
        alert.Status,
        alert.CreatedAt,
        alert.ResolvedAt);
}
