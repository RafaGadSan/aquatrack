using System.Security.Claims;
using AquaTrack.Application.DTOs;
using AquaTrack.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/facilities/{facilityId:guid}")]
public class EnvironmentalReadingsController : ControllerBase
{
    private readonly IEnvironmentalReadingService _readingService;
    private readonly IAlertService _alertService;
    private readonly IValidator<CreateEnvironmentalReadingRequest> _createValidator;

    public EnvironmentalReadingsController(
        IEnvironmentalReadingService readingService,
        IAlertService alertService,
        IValidator<CreateEnvironmentalReadingRequest> createValidator)
    {
        _readingService = readingService;
        _alertService = alertService;
        _createValidator = createValidator;
    }

    [HttpPost("readings")]
    public async Task<ActionResult<RecordReadingResult>> RecordReading(Guid facilityId, CreateEnvironmentalReadingRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _readingService.RecordAsync(facilityId, GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(GetReadings), new { facilityId }, result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("readings")]
    public async Task<ActionResult<IReadOnlyList<EnvironmentalReadingResponse>>> GetReadings(Guid facilityId, CancellationToken cancellationToken)
    {
        var result = await _readingService.GetByFacilityAsync(facilityId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("alerts")]
    public async Task<ActionResult<IReadOnlyList<AlertResponse>>> GetAlerts(Guid facilityId, CancellationToken cancellationToken)
    {
        var result = await _alertService.GetByFacilityAsync(facilityId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
