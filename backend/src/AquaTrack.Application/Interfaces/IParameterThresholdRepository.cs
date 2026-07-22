using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IParameterThresholdRepository
{
    Task AddAsync(ParameterThreshold threshold, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ParameterThreshold>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Global (FacilityId == null) and facility-specific thresholds applicable to the given facility.</summary>
    Task<IReadOnlyList<ParameterThreshold>> GetApplicableAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
