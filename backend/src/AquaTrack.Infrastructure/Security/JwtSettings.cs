namespace AquaTrack.Infrastructure.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>Defaults to an 8-hour work shift — no refresh tokens in the MVP, so this is the whole session length.</summary>
    public int ExpiryMinutes { get; set; } = 480;
}
