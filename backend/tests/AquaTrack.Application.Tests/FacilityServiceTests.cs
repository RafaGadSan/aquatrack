using AquaTrack.Application.DTOs;
using AquaTrack.Application.Interfaces;
using AquaTrack.Application.Services;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using Moq;
using Xunit;

namespace AquaTrack.Application.Tests;

public class FacilityServiceTests
{
    private readonly Mock<IFacilityRepository> _repository = new();
    private readonly FacilityService _sut;

    public FacilityServiceTests()
    {
        _sut = new FacilityService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenNameIsUnique()
    {
        var request = new CreateFacilityRequest("Cage 3", FacilityType.Cage, "North bay");
        _repository.Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(request.Name, result.Value!.Name);
        Assert.Equal(FacilityStatus.Empty, result.Value.Status);
        _repository.Verify(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenNameAlreadyExists()
    {
        var request = new CreateFacilityRequest("Cage 3", FacilityType.Cage, null);
        _repository.Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.CreateAsync(request);

        Assert.False(result.IsSuccess);
        _repository.Verify(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFailure_WhenFacilityDoesNotExist()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSuccess_WhenFacilityExists()
    {
        var facility = new Facility("Tank 1", FacilityType.Tank);
        _repository.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>())).ReturnsAsync(facility);

        var result = await _sut.GetByIdAsync(facility.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(facility.Id, result.Value!.Id);
    }

    [Fact]
    public async Task UpdateStatusAsync_ReturnsFailure_WhenFacilityDoesNotExist()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Facility?)null);

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), new UpdateFacilityStatusRequest(FacilityStatus.Active));

        Assert.False(result.IsSuccess);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesAndPersists_WhenFacilityExists()
    {
        var facility = new Facility("Tank 1", FacilityType.Tank);
        _repository.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>())).ReturnsAsync(facility);

        var result = await _sut.UpdateStatusAsync(facility.Id, new UpdateFacilityStatusRequest(FacilityStatus.Active));

        Assert.True(result.IsSuccess);
        Assert.Equal(FacilityStatus.Active, result.Value!.Status);
        Assert.Equal(FacilityStatus.Active, facility.Status);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
