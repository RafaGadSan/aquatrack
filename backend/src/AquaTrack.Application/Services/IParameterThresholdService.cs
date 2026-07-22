using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IParameterThresholdService
{
    Task<Result<ParameterThresholdResponse>> CreateAsync(CreateParameterThresholdRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ParameterThresholdResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
