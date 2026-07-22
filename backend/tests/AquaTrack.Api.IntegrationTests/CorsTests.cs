using Xunit;

namespace AquaTrack.Api.IntegrationTests;

// Regression coverage for a real bug: without CORS configured, curl and this very test project's
// HttpClient never notice anything is wrong (neither enforces the browser's same-origin policy),
// but every request from the actual frontend gets silently blocked before reaching a controller.
// Only surfaced via a real (headless) browser hitting the API cross-origin.
public class CorsTests : IClassFixture<AquaTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CorsTests(AquaTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PreflightRequest_FromConfiguredFrontendOrigin_IsAllowed()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Equal("http://localhost:5173", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    [Fact]
    public async Task PreflightRequest_FromUnconfiguredOrigin_IsNotAllowed()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://evil.example.com");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await _client.SendAsync(request);

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
