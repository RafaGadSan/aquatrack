using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AquaTrack.Application.DTOs;
using Xunit;

namespace AquaTrack.Api.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<AquaTrackWebApplicationFactory>
{
    // Mirrors the server's JsonStringEnumConverter (Program.cs) so DTOs with enum properties
    // (like AuthResponse.Role) deserialize correctly on the client side of this in-memory test too.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client;

    public AuthEndpointsTests(AquaTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsToken_ForSeededUser()
    {
        var request = new LoginRequest(AquaTrackWebApplicationFactory.OperatorEmail, AquaTrackWebApplicationFactory.TestPassword);

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.Equal(AquaTrackWebApplicationFactory.OperatorEmail, body.Email);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForWrongPassword()
    {
        var request = new LoginRequest(AquaTrackWebApplicationFactory.OperatorEmail, "wrong-password");

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_ForInvalidEmail()
    {
        var request = new LoginRequest("not-an-email", "whatever");

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
