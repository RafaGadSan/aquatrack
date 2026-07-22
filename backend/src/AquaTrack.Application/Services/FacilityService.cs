using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Services;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;

    public FacilityService(IFacilityRepository facilityRepository)
    {
        _facilityRepository = facilityRepository;
    }

    public async Task<Result<FacilityResponse>> CreateAsync(CreateFacilityRequest request, CancellationToken cancellationToken = default)
    {
        if (await _facilityRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return Result<FacilityResponse>.Failure($"A facility named '{request.Name}' already exists.");
        }

        var facility = new Facility(request.Name, request.Type, request.Location);
        await _facilityRepository.AddAsync(facility, cancellationToken);
        await _facilityRepository.SaveChangesAsync(cancellationToken);

        return Result<FacilityResponse>.Success(ToResponse(facility));
    }

    public async Task<IReadOnlyList<FacilityResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.GetAllAsync(cancellationToken);
        return facilities.Select(ToResponse).ToList();
    }

    public async Task<Result<FacilityResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        return facility is null
            ? Result<FacilityResponse>.Failure("Facility not found.")
            : Result<FacilityResponse>.Success(ToResponse(facility));
    }

    public async Task<Result<FacilityResponse>> UpdateStatusAsync(Guid id, UpdateFacilityStatusRequest request, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility is null)
        {
            return Result<FacilityResponse>.Failure("Facility not found.");
        }

        facility.ChangeStatus(request.Status);
        await _facilityRepository.SaveChangesAsync(cancellationToken);

        return Result<FacilityResponse>.Success(ToResponse(facility));
    }

    private static FacilityResponse ToResponse(Facility facility) => new(
        facility.Id,
        facility.Name,
        facility.Type,
        facility.Status,
        facility.Location,
        facility.CreatedAt,
        facility.UpdatedAt);
}
