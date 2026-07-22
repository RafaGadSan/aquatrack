using AquaTrack.Application.DTOs;
using AquaTrack.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/facilities")]
public class FacilitiesController : ControllerBase
{
    private readonly IFacilityService _facilityService;
    private readonly IValidator<CreateFacilityRequest> _createValidator;

    public FacilitiesController(IFacilityService facilityService, IValidator<CreateFacilityRequest> createValidator)
    {
        _facilityService = facilityService;
        _createValidator = createValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FacilityResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _facilityService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FacilityResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _facilityService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<FacilityResponse>> Create(CreateFacilityRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _facilityService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : Conflict(new { error = result.Error });
    }

    [Authorize(Roles = "Admin,ShiftLead")]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<FacilityResponse>> UpdateStatus(Guid id, UpdateFacilityStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _facilityService.UpdateStatusAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }
}
