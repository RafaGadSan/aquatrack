using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IEnvironmentalReadingRepository
{
    Task AddAsync(EnvironmentalReading reading, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EnvironmentalReading>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
