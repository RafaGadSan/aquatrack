using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class EnvironmentalReadingServiceTests
{
    private readonly Mock<IEnvironmentalReadingRepository> _readingRepository = new();
    private readonly Mock<IParameterThresholdRepository> _thresholdRepository = new();
    private readonly Mock<IAlertRepository> _alertRepository = new();
    private readonly Mock<IFacilityRepository> _facilityRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly EnvironmentalReadingService _sut;

    public EnvironmentalReadingServiceTests()
    {
        _sut = new EnvironmentalReadingService(
            _readingRepository.Object,
            _thresholdRepository.Object,
            _alertRepository.Object,
            _facilityRepository.Object,
            _unitOfWork.Object);
    }

    private void SetUpExistingFacility(Guid facilityId)
    {
        var facility = new Facility("Cage 1", FacilityType.Cage);
        _facilityRepository.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>())).ReturnsAsync(facility);
    }

    [Fact]
    public async Task RecordAsync_ReturnsFailure_WhenFacilityDoesNotExist()
    {
        _facilityRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);

        var result = await _sut.RecordAsync(Guid.NewGuid(), Guid.NewGuid(), new CreateEnvironmentalReadingRequest(18, 7, 30, 7.5m));

        Assert.False(result.IsSuccess);
        _readingRepository.Verify(r => r.AddAsync(It.IsAny<EnvironmentalReading>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RecordAsync_PersistsReading_AndReturnsNoAlerts_WhenWithinThresholds()
    {
        var facilityId = Guid.NewGuid();
        SetUpExistingFacility(facilityId);
        _thresholdRepository.Setup(r => r.GetApplicableAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22) });

        var result = await _sut.RecordAsync(facilityId, Guid.NewGuid(), new CreateEnvironmentalReadingRequest(18, 7, 30, 7.5m));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.TriggeredAlerts);
        _readingRepository.Verify(r => r.AddAsync(It.IsAny<EnvironmentalReading>(), It.IsAny<CancellationToken>()), Times.Once);
        _alertRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Alert>>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecordAsync_PersistsTriggeredAlerts_WhenReadingBreachesThreshold()
    {
        var facilityId = Guid.NewGuid();
        SetUpExistingFacility(facilityId);
        _thresholdRepository.Setup(r => r.GetApplicableAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ParameterThreshold(EnvironmentalParameter.Temperature, 10, 22) });

        var result = await _sut.RecordAsync(facilityId, Guid.NewGuid(), new CreateEnvironmentalReadingRequest(28, 7, 30, 7.5m));

        Assert.True(result.IsSuccess);
        var alert = Assert.Single(result.Value!.TriggeredAlerts);
        Assert.Equal(EnvironmentalParameter.Temperature, alert.Parameter);
        Assert.Equal(28m, alert.Value);
        _alertRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Alert>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByFacilityAsync_ReturnsFailure_WhenFacilityDoesNotExist()
    {
        _facilityRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);

        var result = await _sut.GetByFacilityAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }
}
