using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IAlertRepository
{
    Task AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Alert>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
