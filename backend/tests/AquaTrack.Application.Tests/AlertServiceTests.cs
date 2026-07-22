using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class AlertServiceTests
{
    private readonly Mock<IAlertRepository> _alertRepository = new();
    private readonly Mock<IFacilityRepository> _facilityRepository = new();
    private readonly AlertService _sut;

    public AlertServiceTests()
    {
        _sut = new AlertService(_alertRepository.Object, _facilityRepository.Object);
    }

    [Fact]
    public async Task GetByFacilityAsync_ReturnsFailure_WhenFacilityDoesNotExist()
    {
        _facilityRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);

        var result = await _sut.GetByFacilityAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByFacilityAsync_ReturnsAlerts_WhenFacilityExists()
    {
        var facilityId = Guid.NewGuid();
        _facilityRepository.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>())).ReturnsAsync(new Facility("Cage 1", FacilityType.Cage));
        var alert = new Alert(facilityId, Guid.NewGuid(), EnvironmentalParameter.Temperature, 28m, 10m, 22m);
        _alertRepository.Setup(r => r.GetByFacilityIdAsync(facilityId, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { alert });

        var result = await _sut.GetByFacilityAsync(facilityId);

        Assert.True(result.IsSuccess);
        var response = Assert.Single(result.Value!);
        Assert.Equal(alert.Id, response.Id);
    }
}
