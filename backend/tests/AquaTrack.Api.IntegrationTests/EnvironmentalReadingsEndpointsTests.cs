using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AquaTrack.Application.DTOs;
using AquaTrack.Domain.Enums;
using Xunit;

namespace AquaTrack.Api.IntegrationTests;

public class EnvironmentalReadingsEndpointsTests : IClassFixture<AquaTrackWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly AquaTrackWebApplicationFactory _factory;
    private readonly HttpClient _anonymousClient;

    public EnvironmentalReadingsEndpointsTests(AquaTrackWebApplicationFactory factory)
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

    private async Task<Guid> CreateFacilityAsync(HttpClient adminClient)
    {
        var response = await adminClient.PostAsJsonAsync(
            "/api/facilities", new CreateFacilityRequest($"Cage {Guid.NewGuid()}", FacilityType.Cage, null));
        var facility = await response.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);
        return facility!.Id;
    }

    [Fact]
    public async Task RecordReading_TriggersNoAlert_WhenWithinConfiguredThreshold()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var facilityId = await CreateFacilityAsync(adminClient);
        await adminClient.PostAsJsonAsync(
            "/api/parameter-thresholds",
            new CreateParameterThresholdRequest(EnvironmentalParameter.Temperature, 10, 22, facilityId));

        var operatorClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);
        var response = await operatorClient.PostAsJsonAsync(
            $"/api/facilities/{facilityId}/readings",
            new CreateEnvironmentalReadingRequest(18, 7, 30, 7.5m));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RecordReadingResult>(JsonOptions);
        Assert.Empty(body!.TriggeredAlerts);
    }

    [Fact]
    public async Task RecordReading_TriggersAlert_WhenOutsideConfiguredThreshold()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var facilityId = await CreateFacilityAsync(adminClient);
        await adminClient.PostAsJsonAsync(
            "/api/parameter-thresholds",
            new CreateParameterThresholdRequest(EnvironmentalParameter.Temperature, 10, 22, facilityId));

        var operatorClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);
        var response = await operatorClient.PostAsJsonAsync(
            $"/api/facilities/{facilityId}/readings",
            new CreateEnvironmentalReadingRequest(28, 7, 30, 7.5m));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RecordReadingResult>(JsonOptions);
        var alert = Assert.Single(body!.TriggeredAlerts);
        Assert.Equal(EnvironmentalParameter.Temperature, alert.Parameter);
        Assert.Equal(AlertStatus.Active, alert.Status);

        var alertsResponse = await operatorClient.GetAsync($"/api/facilities/{facilityId}/alerts");
        var alerts = await alertsResponse.Content.ReadFromJsonAsync<List<AlertResponse>>(JsonOptions);
        Assert.Single(alerts!);
    }

    [Fact]
    public async Task RecordReading_ReturnsBadRequest_ForInvalidPh()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var facilityId = await CreateFacilityAsync(adminClient);

        var response = await adminClient.PostAsJsonAsync(
            $"/api/facilities/{facilityId}/readings",
            new CreateEnvironmentalReadingRequest(18, 7, 30, 15m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RecordReading_ReturnsNotFound_ForUnknownFacility()
    {
        var operatorClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);

        var response = await operatorClient.PostAsJsonAsync(
            $"/api/facilities/{Guid.NewGuid()}/readings",
            new CreateEnvironmentalReadingRequest(18, 7, 30, 7.5m));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateThreshold_ReturnsForbidden_ForNonAdmin()
    {
        var shiftLeadClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.ShiftLeadEmail);

        var response = await shiftLeadClient.PostAsJsonAsync(
            "/api/parameter-thresholds",
            new CreateParameterThresholdRequest(EnvironmentalParameter.Salinity, 25, 35, null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
