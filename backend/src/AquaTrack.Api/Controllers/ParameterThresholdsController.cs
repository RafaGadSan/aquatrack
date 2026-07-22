using AquaTrack.Application.DTOs;
using AquaTrack.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/parameter-thresholds")]
public class ParameterThresholdsController : ControllerBase
{
    private readonly IParameterThresholdService _thresholdService;
    private readonly IValidator<CreateParameterThresholdRequest> _createValidator;

    public ParameterThresholdsController(IParameterThresholdService thresholdService, IValidator<CreateParameterThresholdRequest> createValidator)
    {
        _thresholdService = thresholdService;
        _createValidator = createValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ParameterThresholdResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _thresholdService.GetAllAsync(cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ParameterThresholdResponse>> Create(CreateParameterThresholdRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _thresholdService.CreateAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }
}
