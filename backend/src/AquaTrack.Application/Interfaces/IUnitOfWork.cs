namespace AquaTrack.Application.Interfaces;

/// <summary>
/// Commits changes tracked across repositories that share the same underlying DbContext. Needed
/// once a use case spans more than one repository (e.g. recording a reading also persists any
/// alerts it triggers) — "call SaveChangesAsync on whichever repository" stops making sense there.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
