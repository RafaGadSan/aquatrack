using AquaTrack.Application.Interfaces;

namespace AquaTrack.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AquaTrackDbContext _context;

    public UnitOfWork(AquaTrackDbContext context)
    {
        _context = context;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
