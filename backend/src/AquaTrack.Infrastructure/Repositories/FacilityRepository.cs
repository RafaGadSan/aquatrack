using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Repositories;

public class FacilityRepository : IFacilityRepository
{
    private readonly AquaTrackDbContext _context;

    public FacilityRepository(AquaTrackDbContext context)
    {
        _context = context;
    }

    public Task<Facility?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Facilities.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Facility>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Facilities.OrderBy(f => f.Name).ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.Facilities.AnyAsync(f => f.Name == name.Trim(), cancellationToken);

    public async Task AddAsync(Facility facility, CancellationToken cancellationToken = default) =>
        await _context.Facilities.AddAsync(facility, cancellationToken);
}
