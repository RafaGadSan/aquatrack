using AquaTrack.Application.Common;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Services;

namespace AquaTrack.Application.Services;

public class EnvironmentalReadingService : IEnvironmentalReadingService
{
    private readonly IEnvironmentalReadingRepository _readingRepository;
    private readonly IParameterThresholdRepository _thresholdRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnvironmentalReadingService(
        IEnvironmentalReadingRepository readingRepository,
        IParameterThresholdRepository thresholdRepository,
        IAlertRepository alertRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _readingRepository = readingRepository;
        _thresholdRepository = thresholdRepository;
        _alertRepository = alertRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RecordReadingResult>> RecordAsync(
        Guid facilityId,
        Guid recordedByUserId,
        CreateEnvironmentalReadingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _facilityRepository.GetByIdAsync(facilityId, cancellationToken) is null)
        {
            return Result<RecordReadingResult>.Failure("Instalación no encontrada.");
        }

        var reading = new EnvironmentalReading(
            facilityId,
            recordedByUserId,
            DateTime.UtcNow,
            request.Temperature,
            request.DissolvedOxygen,
            request.Salinity,
            request.PH);
        await _readingRepository.AddAsync(reading, cancellationToken);

        var thresholds = await _thresholdRepository.GetApplicableAsync(facilityId, cancellationToken);
        var alerts = AlertEvaluator.Evaluate(reading, thresholds);
        if (alerts.Count > 0)
        {
            await _alertRepository.AddRangeAsync(alerts, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new RecordReadingResult(ToResponse(reading), alerts.Select(ToResponse).ToList());
        return Result<RecordReadingResult>.Success(result);
    }

    public async Task<Result<IReadOnlyList<EnvironmentalReadingResponse>>> GetByFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        if (await _facilityRepository.GetByIdAsync(facilityId, cancellationToken) is null)
        {
            return Result<IReadOnlyList<EnvironmentalReadingResponse>>.Failure("Instalación no encontrada.");
        }

        var readings = await _readingRepository.GetByFacilityIdAsync(facilityId, cancellationToken);
        return Result<IReadOnlyList<EnvironmentalReadingResponse>>.Success(readings.Select(ToResponse).ToList());
    }

    private static EnvironmentalReadingResponse ToResponse(EnvironmentalReading reading) => new(
        reading.Id,
        reading.FacilityId,
        reading.RecordedByUserId,
        reading.RecordedAt,
        reading.Temperature,
        reading.DissolvedOxygen,
        reading.Salinity,
        reading.PH);

    private static AlertResponse ToResponse(Alert alert) => new(
        alert.Id,
        alert.FacilityId,
        alert.EnvironmentalReadingId,
        alert.Parameter,
        alert.Value,
        alert.ThresholdMin,
        alert.ThresholdMax,
        alert.Status,
        alert.CreatedAt,
        alert.ResolvedAt);
}
