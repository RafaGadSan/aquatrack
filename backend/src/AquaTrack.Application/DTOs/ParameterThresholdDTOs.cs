using AquaTrack.Domain.Enums;

namespace AquaTrack.Application.DTOs;

public record CreateParameterThresholdRequest(EnvironmentalParameter Parameter, decimal MinValue, decimal MaxValue, Guid? FacilityId);

public record ParameterThresholdResponse(Guid Id, EnvironmentalParameter Parameter, decimal MinValue, decimal MaxValue, Guid? FacilityId);
