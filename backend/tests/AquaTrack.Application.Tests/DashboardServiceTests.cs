using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class DashboardServiceTests
{
    private readonly Mock<IFacilityRepository> _facilityRepository = new();
    private readonly Mock<IAlertRepository> _alertRepository = new();
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _sut = new DashboardService(_facilityRepository.Object, _alertRepository.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_CountsFacilitiesByStatus()
    {
        var active = new Facility("Cage 1", FacilityType.Cage);
        active.ChangeStatus(FacilityStatus.Active);
        var facilities = new[] { active, new Facility("Cage 2", FacilityType.Cage) };
        _facilityRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(facilities);
        _alertRepository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Alert>());

        var result = await _sut.GetSummaryAsync();

        Assert.Equal(2, result.TotalFacilities);
        Assert.Equal(1, result.FacilitiesByStatus.Single(s => s.Status == FacilityStatus.Active).Count);
        Assert.Equal(1, result.FacilitiesByStatus.Single(s => s.Status == FacilityStatus.Empty).Count);
        Assert.Equal(0, result.FacilitiesByStatus.Single(s => s.Status == FacilityStatus.Harvesting).Count);
    }

    [Fact]
    public async Task GetSummaryAsync_ResolvesFacilityNameForActiveAlerts()
    {
        var facility = new Facility("Cage 1", FacilityType.Cage);
        _facilityRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { facility });
        var alert = new Alert(facility.Id, Guid.NewGuid(), EnvironmentalParameter.Temperature, 28m, 10m, 22m);
        _alertRepository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { alert });

        var result = await _sut.GetSummaryAsync();

        Assert.Equal(1, result.ActiveAlertsCount);
        var alertResponse = Assert.Single(result.ActiveAlerts);
        Assert.Equal("Cage 1", alertResponse.FacilityName);
        Assert.Equal(alert.Id, alertResponse.Id);
    }

    [Fact]
    public async Task GetSummaryAsync_FallsBackToPlaceholderName_WhenAlertsFacilityWasNotReturned()
    {
        _facilityRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Facility>());
        var alert = new Alert(Guid.NewGuid(), Guid.NewGuid(), EnvironmentalParameter.Salinity, 40m, 25m, 35m);
        _alertRepository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { alert });

        var result = await _sut.GetSummaryAsync();

        var alertResponse = Assert.Single(result.ActiveAlerts);
        Assert.Equal("Unknown facility", alertResponse.FacilityName);
    }
}
