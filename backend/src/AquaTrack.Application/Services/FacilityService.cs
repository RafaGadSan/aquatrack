using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Services;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FacilityService(IFacilityRepository facilityRepository, IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FacilityResponse>> CreateAsync(CreateFacilityRequest request, CancellationToken cancellationToken = default)
    {
        if (await _facilityRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return Result<FacilityResponse>.Failure($"Ya existe una instalación llamada '{request.Name}'.");
        }

        var facility = new Facility(request.Name, request.Type, request.Location);
        await _facilityRepository.AddAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            ? Result<FacilityResponse>.Failure("Instalación no encontrada.")
            : Result<FacilityResponse>.Success(ToResponse(facility));
    }

    public async Task<Result<FacilityResponse>> UpdateStatusAsync(Guid id, UpdateFacilityStatusRequest request, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility is null)
        {
            return Result<FacilityResponse>.Failure("Instalación no encontrada.");
        }

        facility.ChangeStatus(request.Status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
