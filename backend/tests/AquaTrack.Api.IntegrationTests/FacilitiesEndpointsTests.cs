using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AquaTrack.Application.DTOs;
using AquaTrack.Domain.Enums;
using Xunit;

namespace AquaTrack.Api.IntegrationTests;

public class FacilitiesEndpointsTests : IClassFixture<AquaTrackWebApplicationFactory>
{
    // Mirrors the server's JsonStringEnumConverter (Program.cs) so FacilityResponse's enum
    // properties (Type, Status) deserialize correctly on the client side of this in-memory test.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly AquaTrackWebApplicationFactory _factory;
    private readonly HttpClient _anonymousClient;

    public FacilitiesEndpointsTests(AquaTrackWebApplicationFactory factory)
    {
        _factory = factory;
        _anonymousClient = factory.CreateClient();
    }

    private async Task<HttpClient> AuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var token = await _factory.LoginAsync(_anonymousClient, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task GetAll_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _anonymousClient.GetAsync("/api/facilities");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated_ForAdmin()
    {
        var client = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var request = new CreateFacilityRequest($"Cage {Guid.NewGuid()}", FacilityType.Cage, "North bay");

        var response = await client.PostAsJsonAsync("/api/facilities", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);
        Assert.Equal(request.Name, body!.Name);
        Assert.Equal(FacilityStatus.Empty, body.Status);
    }

    [Fact]
    public async Task Create_ReturnsForbidden_ForOperator()
    {
        var client = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);
        var request = new CreateFacilityRequest($"Cage {Guid.NewGuid()}", FacilityType.Cage, null);

        var response = await client.PostAsJsonAsync("/api/facilities", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsConflict_ForDuplicateName()
    {
        var client = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var request = new CreateFacilityRequest($"Cage {Guid.NewGuid()}", FacilityType.Cage, null);
        await client.PostAsJsonAsync("/api/facilities", request);

        var response = await client.PostAsJsonAsync("/api/facilities", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_Succeeds_ForShiftLead()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var created = await adminClient.PostAsJsonAsync(
            "/api/facilities", new CreateFacilityRequest($"Tank {Guid.NewGuid()}", FacilityType.Tank, null));
        var facility = await created.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);

        var shiftLeadClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.ShiftLeadEmail);
        var response = await shiftLeadClient.PutAsJsonAsync(
            $"/api/facilities/{facility!.Id}/status", new UpdateFacilityStatusRequest(FacilityStatus.Active));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);
        Assert.Equal(FacilityStatus.Active, body!.Status);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsForbidden_ForOperator()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var created = await adminClient.PostAsJsonAsync(
            "/api/facilities", new CreateFacilityRequest($"Tank {Guid.NewGuid()}", FacilityType.Tank, null));
        var facility = await created.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);

        var operatorClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);
        var response = await operatorClient.PutAsJsonAsync(
            $"/api/facilities/{facility!.Id}/status", new UpdateFacilityStatusRequest(FacilityStatus.Active));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForUnknownId()
    {
        var client = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);

        var response = await client.GetAsync($"/api/facilities/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
