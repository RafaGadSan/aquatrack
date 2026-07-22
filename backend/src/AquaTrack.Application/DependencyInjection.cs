using AquaTrack.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AquaTrack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IEnvironmentalReadingService, EnvironmentalReadingService>();
        services.AddScoped<IParameterThresholdService, ParameterThresholdService>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
