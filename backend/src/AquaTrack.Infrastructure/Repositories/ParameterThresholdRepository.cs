using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Repositories;

public class ParameterThresholdRepository : IParameterThresholdRepository
{
    private readonly AquaTrackDbContext _context;

    public ParameterThresholdRepository(AquaTrackDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ParameterThreshold threshold, CancellationToken cancellationToken = default) =>
        await _context.ParameterThresholds.AddAsync(threshold, cancellationToken);

    public async Task<IReadOnlyList<ParameterThreshold>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.ParameterThresholds.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ParameterThreshold>> GetApplicableAsync(Guid facilityId, CancellationToken cancellationToken = default) =>
        await _context.ParameterThresholds
            .Where(t => t.FacilityId == facilityId || t.FacilityId == null)
            .ToListAsync(cancellationToken);
}
