using AquaTrack.Domain.Common;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;

namespace AquaTrack.Domain.Entities;

public class Facility : Entity
{
    public string Name { get; private set; } = null!;
    public FacilityType Type { get; private set; }
    public FacilityStatus Status { get; private set; }
    public string? Location { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Facility()
    {
    }

    public Facility(string name, FacilityType type, string? location = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la instalación es obligatorio.");

        Name = name.Trim();
        Type = type;
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        Status = FacilityStatus.Empty;

        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la instalación es obligatorio.");

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(FacilityStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
