using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquaTrack.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AquaTrackDbContext _context;

    public AlertRepository(AquaTrackDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default) =>
        await _context.Alerts.AddRangeAsync(alerts, cancellationToken);

    public async Task<IReadOnlyList<Alert>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default) =>
        await _context.Alerts
            .Where(a => a.FacilityId == facilityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
}
