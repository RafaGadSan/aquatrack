using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IEnvironmentalReadingService
{
    Task<Result<RecordReadingResult>> RecordAsync(
        Guid facilityId,
        Guid recordedByUserId,
        CreateEnvironmentalReadingRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<EnvironmentalReadingResponse>>> GetByFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
