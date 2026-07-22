using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IAlertService
{
    Task<Result<IReadOnlyList<AlertResponse>>> GetByFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
