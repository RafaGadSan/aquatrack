using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Services;

public class ParameterThresholdService : IParameterThresholdService
{
    private readonly IParameterThresholdRepository _thresholdRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ParameterThresholdService(
        IParameterThresholdRepository thresholdRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _thresholdRepository = thresholdRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ParameterThresholdResponse>> CreateAsync(CreateParameterThresholdRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FacilityId is { } facilityId && await _facilityRepository.GetByIdAsync(facilityId, cancellationToken) is null)
        {
            return Result<ParameterThresholdResponse>.Failure("Facility not found.");
        }

        var threshold = new ParameterThreshold(request.Parameter, request.MinValue, request.MaxValue, request.FacilityId);

        await _thresholdRepository.AddAsync(threshold, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ParameterThresholdResponse>.Success(ToResponse(threshold));
    }

    public async Task<IReadOnlyList<ParameterThresholdResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var thresholds = await _thresholdRepository.GetAllAsync(cancellationToken);
        return thresholds.Select(ToResponse).ToList();
    }

    private static ParameterThresholdResponse ToResponse(ParameterThreshold threshold) => new(
        threshold.Id,
        threshold.Parameter,
        threshold.MinValue,
        threshold.MaxValue,
        threshold.FacilityId);
}
