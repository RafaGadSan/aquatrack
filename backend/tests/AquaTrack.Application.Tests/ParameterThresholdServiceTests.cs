using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class ParameterThresholdServiceTests
{
    private readonly Mock<IParameterThresholdRepository> _thresholdRepository = new();
    private readonly Mock<IFacilityRepository> _facilityRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ParameterThresholdService _sut;

    public ParameterThresholdServiceTests()
    {
        _sut = new ParameterThresholdService(_thresholdRepository.Object, _facilityRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_CreatesGlobalThreshold_WhenFacilityIdIsNull()
    {
        var request = new CreateParameterThresholdRequest(EnvironmentalParameter.PH, 6.5m, 8.5m, null);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.FacilityId);
        _facilityRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenFacilityIdDoesNotExist()
    {
        var facilityId = Guid.NewGuid();
        _facilityRepository.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);
        var request = new CreateParameterThresholdRequest(EnvironmentalParameter.PH, 6.5m, 8.5m, facilityId);

        var result = await _sut.CreateAsync(request);

        Assert.False(result.IsSuccess);
        _thresholdRepository.Verify(r => r.AddAsync(It.IsAny<ParameterThreshold>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CreatesFacilitySpecificThreshold_WhenFacilityExists()
    {
        var facilityId = Guid.NewGuid();
        _facilityRepository.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>())).ReturnsAsync(new Facility("Cage 1", FacilityType.Cage));
        var request = new CreateParameterThresholdRequest(EnvironmentalParameter.Temperature, 20, 30, facilityId);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(facilityId, result.Value!.FacilityId);
    }
}
