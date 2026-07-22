using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IFacilityRepository
{
    Task<Facility?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Facility>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task AddAsync(Facility facility, CancellationToken cancellationToken = default);
}
