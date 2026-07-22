using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AquaTrack.Application.DTOs;
using AquaTrack.Domain.Enums;
using Xunit;

namespace AquaTrack.Api.IntegrationTests;

public class DashboardEndpointsTests : IClassFixture<AquaTrackWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly AquaTrackWebApplicationFactory _factory;
    private readonly HttpClient _anonymousClient;

    public DashboardEndpointsTests(AquaTrackWebApplicationFactory factory)
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
    public async Task GetSummary_ReturnsUnauthorized_ForAnonymousCaller()
    {
        var response = await _anonymousClient.GetAsync("/api/dashboard/summary");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSummary_IncludesFreshlyRecordedActiveAlert()
    {
        var adminClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.AdminEmail);
        var facilityName = $"Cage {Guid.NewGuid()}";
        var facilityResponse = await adminClient.PostAsJsonAsync(
            "/api/facilities", new CreateFacilityRequest(facilityName, FacilityType.Cage, null));
        var facility = await facilityResponse.Content.ReadFromJsonAsync<FacilityResponse>(JsonOptions);

        await adminClient.PostAsJsonAsync(
            "/api/parameter-thresholds",
            new CreateParameterThresholdRequest(EnvironmentalParameter.Temperature, 10, 22, facility!.Id));

        var operatorClient = await AuthenticatedClientAsync(AquaTrackWebApplicationFactory.OperatorEmail);
        await operatorClient.PostAsJsonAsync(
            $"/api/facilities/{facility.Id}/readings",
            new CreateEnvironmentalReadingRequest(28, 7, 30, 7.5m));

        var summaryResponse = await operatorClient.GetAsync("/api/dashboard/summary");
        Assert.Equal(HttpStatusCode.OK, summaryResponse.StatusCode);
        var summary = await summaryResponse.Content.ReadFromJsonAsync<DashboardSummaryResponse>(JsonOptions);

        Assert.True(summary!.TotalFacilities >= 1);
        Assert.Contains(summary.ActiveAlerts, a => a.FacilityId == facility.Id && a.FacilityName == facilityName);
    }
}
