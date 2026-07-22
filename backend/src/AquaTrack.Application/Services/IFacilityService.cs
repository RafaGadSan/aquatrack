using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IFacilityService
{
    Task<Result<FacilityResponse>> CreateAsync(CreateFacilityRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FacilityResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<FacilityResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<FacilityResponse>> UpdateStatusAsync(Guid id, UpdateFacilityStatusRequest request, CancellationToken cancellationToken = default);
}
