using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Repositories;

public class EnvironmentalReadingRepository : IEnvironmentalReadingRepository
{
    private readonly AquaTrackDbContext _context;

    public EnvironmentalReadingRepository(AquaTrackDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EnvironmentalReading reading, CancellationToken cancellationToken = default) =>
        await _context.EnvironmentalReadings.AddAsync(reading, cancellationToken);

    public async Task<IReadOnlyList<EnvironmentalReading>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default) =>
        await _context.EnvironmentalReadings
            .Where(r => r.FacilityId == facilityId)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync(cancellationToken);
}
