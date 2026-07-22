using System.Text;
using AquaTrack.Application.Interfaces;
using AquaTrack.Infrastructure.Persistence;
using AquaTrack.Infrastructure.Repositories;
using AquaTrack.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AquaTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AquaTrackDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // HS256 needs a key of at least 256 bits; failing fast at startup beats a cryptic
        // IDX10720 exception the first time someone logs in.
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(
                s => Encoding.UTF8.GetByteCount(s.Secret) >= 32,
                "Jwt:Secret must be at least 32 bytes (256 bits) long for HS256 signing.")
            .ValidateOnStart();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
